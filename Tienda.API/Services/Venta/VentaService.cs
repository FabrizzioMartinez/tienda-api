using Microsoft.EntityFrameworkCore;
using Tienda.API.Data;
using Tienda.API.DTOs;
using Tienda.API.Interfaces.Venta;
using Tienda.API.Models;

namespace Tienda.API.Services.Venta
{
    public class VentaService : IVentaService
    {
        private readonly TiendaDbContext _context;

        public VentaService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegistrarVentaAsync(VentaCreateDto dto)
        {
            // 1. Validaciones rápidas de contrato (Fail-Fast)
            if (dto.Detalles == null || !dto.Detalles.Any())
                return false;

            if (dto.Total <= 0)
                return false;

            decimal montoEfectivoEval = dto.MontoEfectivo ?? 0.00m;
            decimal montoDigitalEval = dto.MontoDigital ?? 0.00m;

            if (!dto.EsCredito)
            {
                if ((montoEfectivoEval + montoDigitalEval) != dto.Total)
                {
                    return false;
                }
            }

            // Iniciamos la transacción atómica para proteger el stock y los correlativos
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var zonaPeru = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
                DateTime horaPeruRaw = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaPeru);
                DateTime fechaPeruLimpia = DateTime.SpecifyKind(horaPeruRaw, DateTimeKind.Unspecified);

                // =========================================================================
                // 🔄 GENERACIÓN AUTOMÁTICA DEL CORRELATIVO CON PREFIJO (FC / BV)
                // =========================================================================
                string numeroComprobanteFinal = dto.NumeroComprobante;

                if (string.IsNullOrWhiteSpace(numeroComprobanteFinal) || numeroComprobanteFinal == "00000000")
                {
                    // Determinar el prefijo según el tipo de comprobante recibido (Ignora mayúsculas/minúsculas)
                    string prefijo = "NV"; // 'NV' (Nota de Venta) por defecto si no es ninguno
                    string tipoCompUpper = dto.TipoComprobante?.ToUpper() ?? "";

                    if (tipoCompUpper.Contains("FACTURA") || tipoCompUpper == "FAC" || tipoCompUpper == "01")
                    {
                        prefijo = "FC";
                    }
                    else if (tipoCompUpper.Contains("BOLETA") || tipoCompUpper == "BOL" || tipoCompUpper == "03")
                    {
                        prefijo = "BV";
                    }

                    // Buscar la última venta registrada de ese mismo tipo que ya tenga el prefijo
                    var ultimaVenta = await _context.Ventas
                        .Where(v => v.TipoComprobante == dto.TipoComprobante &&
                                    v.NumeroComprobante != "00000000" &&
                                    v.NumeroComprobante.StartsWith(prefijo))
                        .OrderByDescending(v => v.VentaID)
                        .FirstOrDefaultAsync();

                    int siguienteCorrelativo = 1;

                    if (ultimaVenta != null && !string.IsNullOrWhiteSpace(ultimaVenta.NumeroComprobante))
                    {
                        // Extraemos el número quitando las dos letras iniciales (FC o BV)
                        string numeroLimpio = ultimaVenta.NumeroComprobante.Substring(2);

                        if (int.TryParse(numeroLimpio, out int ultimoNumero))
                        {
                            siguienteCorrelativo = ultimoNumero + 1;
                        }
                    }

                    // Formateamos a 8 dígitos numéricos pegados al prefijo (Ej: FC00000001, BV00000142)
                    numeroComprobanteFinal = $"{prefijo}{siguienteCorrelativo.ToString("D8")}";
                }

                // Validación estricta anti-duplicados por si hay colisión de peticiones simultáneas
                if (numeroComprobanteFinal != "00000000")
                {
                    bool existeComprobante = await _context.Ventas
                        .AnyAsync(v => v.NumeroComprobante == numeroComprobanteFinal && v.TipoComprobante == dto.TipoComprobante);

                    if (existeComprobante)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                // Obtener o crear el cliente usando el número de comprobante definitivo
                var cliente = await ObtenerOCrearClienteAsync(dto.ClienteNombre, numeroComprobanteFinal, dto.TipoDocumento);

                if (cliente == null || cliente.ClienteID <= 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                var venta = new Tienda.API.Models.Venta
                {
                    ClienteID = cliente.ClienteID,
                    TipoComprobante = dto.TipoComprobante,
                    NumeroComprobante = numeroComprobanteFinal, // Guardamos la serie autogenerada
                    Total = dto.EsCredito ? dto.Total : (montoEfectivoEval + montoDigitalEval),
                    EsCredito = dto.EsCredito,
                    FechaRegistro = fechaPeruLimpia,
                    EsEfectivo = dto.EsEfectivo,
                    MontoEfectivo = montoEfectivoEval,
                    EsDigital = dto.EsDigital,
                    MontoDigital = montoDigitalEval
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync(); // Guardamos para obtener el VentaID asignado

                // Procesar los detalles de la venta y rebajar inventario
                foreach (var item in dto.Detalles)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(p => p.ProductoID == item.ProductoID);

                    if (producto == null)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    if (producto.Stock < item.Cantidad)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    // Rebajamos el stock de forma segura
                    producto.Stock -= item.Cantidad;

                    var detalle = new Tienda.API.Models.DetalleVenta
                    {
                        VentaID = venta.VentaID,
                        ProductoID = item.ProductoID,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        Subtotal = item.Subtotal,
                        FechaRegistro = fechaPeruLimpia
                    };

                    _context.DetalleVentas.Add(detalle);
                }

                await _context.SaveChangesAsync();

                // Si la venta se procesa bajo la modalidad de Crédito, creamos la deuda inicial
                if (dto.EsCredito)
                {
                    var cuenta = new CuentaPorCobrar
                    {
                        VentaID = venta.VentaID,
                        MontoTotal = dto.Total,
                        SaldoPendiente = dto.Total,
                        Detalle = "Venta a crédito inicial",
                        FechaRegistro = fechaPeruLimpia,
                        Estado = "PENDIENTE"
                    };

                    _context.CuentasPorCobrar.Add(cuenta);
                    await _context.SaveChangesAsync();
                }

                // Consolidamos definitivamente los cambios en la base de datos
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                // Ante cualquier error imprevisto de base de datos, limpiamos el estado
                await transaction.RollbackAsync();
                return false;
            }
        }

        #region Cliente
        public async Task<ClienteDto> ObtenerOCrearClienteAsync(string nombre, string numeroDocumento, string tipoDocumento)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.NumeroDocumento == numeroDocumento);

            if (cliente != null)
            {
                return new ClienteDto
                {
                    ClienteID = cliente.ClienteID,
                    NombreRazonSocial = cliente.NombreRazonSocial,
                    NumeroDocumento = cliente.NumeroDocumento,
                    TipoDocumentoCode = cliente.TipoDocumento,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono
                };
            }

            var nuevo = new Models.Cliente
            {
                NombreRazonSocial = nombre,
                NumeroDocumento = numeroDocumento,
                TipoDocumento = tipoDocumento,
                Activo = true
            };

            _context.Clientes.Add(nuevo);
            await _context.SaveChangesAsync();

            return new ClienteDto
            {
                ClienteID = nuevo.ClienteID,
                NombreRazonSocial = nuevo.NombreRazonSocial,
                NumeroDocumento = nuevo.NumeroDocumento,
                TipoDocumentoCode = nuevo.TipoDocumento,
                Email = null,
                Telefono = null
            };
        }
        #endregion

        public async Task<List<VentaDto>> ObtenerVentasPorFechaAsync(DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1);

            return await _context.Ventas
                .AsNoTracking() // 🚀 Optimización de rendimiento para lecturas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.FechaRegistro >= fechaInicio && v.FechaRegistro < fechaFin)
                .Select(v => new VentaDto
                {
                    VentaID = v.VentaID,
                    ClienteID = v.ClienteID,
                    ClienteNombre = v.Cliente != null ? v.Cliente.NombreRazonSocial : "Sin Cliente",
                    TipoComprobante = v.TipoComprobante,
                    NumeroComprobante = v.NumeroComprobante ?? "",
                    Total = v.Total,
                    EsCredito = v.EsCredito,
                    FechaRegistro = v.FechaRegistro,
                    EsEfectivo = v.EsEfectivo,
                    MontoEfectivo = v.MontoEfectivo,
                    EsDigital = v.EsDigital,
                    MontoDigital = v.MontoDigital,
                    Detalles = v.Detalles.Select(d => new DetalleVentaDto
                    {
                        ProductoID = d.ProductoID,
                        NombreProducto = d.Producto != null ? d.Producto.Nombre : "Sin Nombre",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .OrderByDescending(v => v.FechaRegistro)
                .ToListAsync();
        }

        public async Task<List<VentaDto>> ObtenerVentasFiltroAsync(DateTime? fecha, DateTime? fechaHasta, int? productoId)
        {
            var query = _context.Ventas
                .AsNoTracking()
                .AsQueryable();
            if (fecha.HasValue && fechaHasta.HasValue)
            {
                var fechaInicio = fecha.Value.Date;
                var fechaFin = fechaHasta.Value.Date.AddDays(1);

                query = query.Where(v => v.FechaRegistro >= fechaInicio && v.FechaRegistro < fechaFin);
            }
            else if (fecha.HasValue)
            {
                var fechaInicio = fecha.Value.Date;
                var fechaFin = fechaInicio.AddDays(1);
                query = query.Where(v => v.FechaRegistro >= fechaInicio && v.FechaRegistro < fechaFin);
            }
            if (productoId.HasValue && productoId.Value > 0)
            {
                query = query.Where(v => v.Detalles.Any(d => d.ProductoID == productoId.Value));
            }
            return await query
                .Select(v => new VentaDto
                {
                    VentaID = v.VentaID,
                    TipoComprobante = v.TipoComprobante,
                    NumeroComprobante = v.NumeroComprobante,
                    ClienteID = v.ClienteID,
                    ClienteNombre = v.Cliente != null ? v.Cliente.NombreRazonSocial : "Sin Cliente",
                    FechaRegistro = v.FechaRegistro,
                    EsEfectivo = v.EsEfectivo,
                    EsDigital = v.EsDigital,
                    EsCredito = v.EsCredito,
                    Total = (productoId.HasValue && productoId.Value > 0)
                        ? v.Detalles.Where(d => d.ProductoID == productoId.Value).Sum(d => d.Subtotal)
                        : v.Total,

                    Detalles = v.Detalles
                        .Where(d => !productoId.HasValue || productoId.Value <= 0 || d.ProductoID == productoId.Value)
                        .Select(d => new DetalleVentaDto
                        {
                            NombreProducto = d.Producto != null ? d.Producto.Nombre : "Producto No Registrado",
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Subtotal = d.Subtotal
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<bool> AnularVentaAsync(int ventaId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venta = await _context.Ventas
                    .Include(v => v.Detalles)
                    .FirstOrDefaultAsync(v => v.VentaID == ventaId);

                if (venta == null) return false;
                foreach (var detalle in venta.Detalles)
                {
                    var producto = await _context.Productos
                        .FirstOrDefaultAsync(p => p.ProductoID == detalle.ProductoID);

                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                    }
                }
                if (venta.EsCredito)
                {
                    var cuenta = await _context.CuentasPorCobrar
                        .FirstOrDefaultAsync(c => c.VentaID == venta.VentaID);

                    if (cuenta != null)
                    {
                        cuenta.Estado = "ANULADO";
                        cuenta.SaldoPendiente = 0;
                        cuenta.Detalle += " (Venta Anulada)";
                    }
                }
                _context.Ventas.Remove(venta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al anular venta: {ex.Message}");
                await transaction.RollbackAsync();
                return false;
            }
        }
        public async Task<VentaDto?> ObtenerVentaPorIdAsync(int ventaId)
        {
            return await _context.Ventas
                .AsNoTracking() // 🚀 Lectura limpia y rápida sin persistencia en memoria
                .Where(v => v.VentaID == ventaId)
                .Select(v => new VentaDto
                {
                    VentaID = v.VentaID,
                    ClienteID = v.ClienteID,
                    ClienteNombre = v.Cliente != null ? v.Cliente.NombreRazonSocial : "Sin Cliente",
                    TipoComprobante = v.TipoComprobante,
                    NumeroComprobante = v.NumeroComprobante ?? "",
                    Total = v.Total,
                    EsCredito = v.EsCredito,
                    FechaRegistro = v.FechaRegistro,
                    EsEfectivo = v.EsEfectivo,
                    MontoEfectivo = v.MontoEfectivo,
                    EsDigital = v.EsDigital,
                    MontoDigital = v.MontoDigital,
                    Detalles = v.Detalles.Select(d => new DetalleVentaDto
                    {
                        ProductoID = d.ProductoID,
                        NombreProducto = d.Producto != null ? d.Producto.Nombre : "Sin Nombre",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                })
                .FirstOrDefaultAsync(); // Devuelve la venta o null si el ID no existe
        }

    }
}