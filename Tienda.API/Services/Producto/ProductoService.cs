using Microsoft.EntityFrameworkCore;
using SIOL.Models;
using Tienda.API.Data;
using Tienda.API.DTOs;
using Tienda.API.DTOs.Producto.Tienda.API.DTOs;
using Tienda.API.Interfaces;
using Tienda.API.Models;

namespace Tienda.API.Services
{
    public class ProductoService : IProductoService
    {
        private readonly TiendaDbContext _context;

        public ProductoService(TiendaDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProductoDto>> GetProductosAsync()
        {
            return await _context.Productos
                .Select(p => new ProductoDto
                {
                    ProductoID = p.ProductoID,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    NombreMarca = p.Marca.Nombre,
                    NombreTipo = p.TipoProducto.Nombre,      // Si "NombreTipo" es el nombre del tipo
                    TipoProducto = p.TipoProducto.Nombre,    // O si usas esta propiedad
                    UnidadMedida = p.UnidadMedida.Nombre,
                    Abreviatura = p.UnidadMedida.Abreviatura,
                    StockMinimo = p.StockMinimo
                })
                .ToListAsync();
        }
        public async Task<ProductoDto?> GetProductoByIdAsync(int id)
        {
            return await _context.Productos
                .Where(p => p.ProductoID == id)
                .Select(p => new ProductoDto
                {
                    ProductoID = p.ProductoID,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    StockMinimo = p.StockMinimo,
                    NombreMarca = p.Marca.Nombre,
                    NombreTipo = p.TipoProducto.Nombre,
                    TipoProducto = p.TipoProducto.Nombre,
                    UnidadMedida = p.UnidadMedida.Nombre,
                    Abreviatura = p.UnidadMedida.Abreviatura,

                    // 🌟 CRÍTICO: Envía las llaves foráneas numéricas al Frontend
                    MarcaID = p.MarcaID,
                    TipoProductoID = p.TipoProductoID,
                    UnidadMedidaID = p.UnidadMedidaID
                })
                .FirstOrDefaultAsync();
        }
        public async Task<Producto> UpsertProductoAsync(Producto producto)
        {
            producto.FechaModificacion = DateTime.UtcNow;

            if (producto.ProductoID == 0)
            {
                _context.Productos.Add(producto);
            }
            else
            {
                _context.Entry(producto).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<List<ProductoBusquedaDto>> SearchProductosAsync(string query)
        {
            query = query?.Trim().ToLower() ?? string.Empty;

            return await _context.Productos

                .Where(p =>
                    !string.IsNullOrEmpty(p.Nombre) &&
                    p.Nombre.ToLower().Contains(query))

                .Select(p => new ProductoBusquedaDto
                {
                    ProductoID = p.ProductoID,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Tamaño =p.UnidadMedida.Abreviatura
                })
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> ActualizarStockAsync(int productoId, int nuevoStock)
        {
            var producto = new Producto { ProductoID = productoId };
            _context.Productos.Attach(producto);
            producto.Stock = nuevoStock;
            _context.Entry(producto).Property(p => p.Stock).IsModified = true;
            var filasAfectadas = await _context.SaveChangesAsync();
            return filasAfectadas > 0;
        }
    }
}