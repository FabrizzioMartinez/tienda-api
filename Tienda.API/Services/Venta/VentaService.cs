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
            if (dto.Detalles == null || !dto.Detalles.Any())
                return false;

            if (dto.Total <= 0)
                return false;

            // Evaluamos los montos de entrada de forma segura
            decimal montoEfectivoEval = dto.MontoEfectivo ?? 0.00m;
            decimal montoDigitalEval = dto.MontoDigital ?? 0.00m;

            if (!dto.EsCredito)
            {
                if ((montoEfectivoEval + montoDigitalEval) != dto.Total)
                {
                    return false;
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var nowUtc = DateTime.UtcNow;

                var cliente = await ObtenerOCrearClienteAsync(dto.ClienteNombre, dto.NumeroComprobante, dto.TipoDocumento);

                if (cliente == null || cliente.ClienteID <= 0)
                    return false;

                var venta = new Tienda.API.Models.Venta
                {
                    ClienteID = cliente.ClienteID,
                    TipoComprobante = dto.TipoComprobante,
                    NumeroComprobante = dto.NumeroComprobante,
                    // 🚀 El total de la venta se calcula dinámicamente según los métodos de pago (Efectivo + Digital)
                    // Si es crédito, toma el total enviado; de lo contrario, suma ambos montos pagados.
                    Total = dto.EsCredito ? dto.Total : (montoEfectivoEval + montoDigitalEval),
                    EsCredito = dto.EsCredito,
                    FechaRegistro = nowUtc,
                    EsEfectivo = dto.EsEfectivo,
                    MontoEfectivo = montoEfectivoEval,
                    EsDigital = dto.EsDigital,
                    MontoDigital = montoDigitalEval
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                foreach (var item in dto.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoID);

                    if (producto == null)
                    {
                        Console.WriteLine($"[Error Stock]: El producto con ID {item.ProductoID} no existe.");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    if (producto.Stock < item.Cantidad)
                    {
                        Console.WriteLine($"[Error Stock]: Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    producto.Stock -= item.Cantidad;

                    var detalle = new Tienda.API.Models.DetalleVenta
                    {
                        VentaID = venta.VentaID,
                        ProductoID = item.ProductoID,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        Subtotal = item.Subtotal,
                        FechaRegistro = nowUtc
                    };

                    _context.DetalleVentas.Add(detalle);
                }

                await _context.SaveChangesAsync();

                if (dto.EsCredito)
                {
                    var cuenta = new CuentaPorCobrar
                    {
                        VentaID = venta.VentaID,
                        MontoTotal = dto.Total,
                        SaldoPendiente = dto.Total,
                        Detalle = "Venta a crédito inicial",
                        FechaRegistro = nowUtc,
                        Estado = "PENDIENTE"
                    };

                    _context.CuentasPorCobrar.Add(cuenta);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error al registrar venta]: {ex.Message}");
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

        public async Task<List<VentaDto>> ObtenerVentasFiltroAsync(DateTime fecha, int? productoId)
        {

            var fechaInicio = fecha.Date;
            var fechaFin = fechaInicio.AddDays(1);

            var ventas = await _context.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Where(v => v.FechaRegistro >= fechaInicio && v.FechaRegistro < fechaFin)
                .ToListAsync();


            if (productoId.HasValue && productoId.Value > 0)
            {
                ventas = ventas
                    .Where(v => v.Detalles != null && v.Detalles.Any(d => d.ProductoID == productoId.Value))
                    .ToList();
            }

            return ventas.Select(v => new VentaDto
            {
                VentaID = v.VentaID,
                TipoComprobante = v.TipoComprobante,
                NumeroComprobante = v.NumeroComprobante,
                ClienteID = v.ClienteID,
                ClienteNombre = v.Cliente != null ? v.Cliente.NombreRazonSocial : "Cliente Anónimo",
                FechaRegistro = v.FechaRegistro,
                Total = v.Total,
                EsEfectivo = v.EsEfectivo,
                EsDigital = v.EsDigital,
                EsCredito = v.EsCredito,

                Detalles = v.Detalles != null
                    ? v.Detalles.Select(d => new DetalleVentaDto
                    {
                        NombreProducto = d.Producto != null ? d.Producto.Nombre : "Producto No Registrado",
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                    : new List<DetalleVentaDto>()
            }).ToList();
        }
    }
}