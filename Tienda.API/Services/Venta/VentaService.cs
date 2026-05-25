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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var nowUtc = DateTime.UtcNow;

                // 🔎 VALIDACIÓN: detalles
                if (dto.Detalles == null || !dto.Detalles.Any())
                    return false;

                if (dto.Total <= 0)
                    return false;

                // 🧠 CLIENTE: obtener o crear automáticamente
                var cliente = await ObtenerOCrearClienteAsync(dto.ClienteNombre,dto.NumeroComprobante, dto.TipoDocumento);

                if (cliente == null || cliente.ClienteID <= 0)
                    return false;

                // 1. CABECERA
                var venta = new Tienda.API.Models.Venta
                {
                    ClienteID = cliente.ClienteID,
                    TipoComprobante = dto.TipoComprobante,
                    NumeroComprobante = dto.NumeroComprobante,
                    Total = dto.Total,
                    EsCredito = dto.EsCredito,
                    FechaRegistro = nowUtc
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // 2. DETALLES
                foreach (var item in dto.Detalles)
                {
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

                // 3. CRÉDITO
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
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        #region Cliente
        public async Task<ClienteDto> ObtenerOCrearClienteAsync(string nombre, string numeroDocumento, string tipoDocumento)
        {
            // 🔎 buscar cliente existente
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.NumeroDocumento == numeroDocumento);

            // ✔ si existe, lo devolvemos
            if (cliente != null)
            {
                return new ClienteDto
                {
                    ClienteID = cliente.ClienteID,
                    NombreRazonSocial = cliente.NombreRazonSocial,
                    NumeroDocumento = cliente.NumeroDocumento,
                    TipoDocumento = cliente.TipoDocumento,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono
                };
            }

            // 🆕 si no existe, lo creamos
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
                TipoDocumento = nuevo.TipoDocumento,
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
    }
}