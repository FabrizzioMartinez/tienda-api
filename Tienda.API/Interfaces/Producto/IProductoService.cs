using SIOL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tienda.API.DTOs;
using Tienda.API.DTOs.Producto.Tienda.API.DTOs;
using Tienda.API.Models;

namespace Tienda.API.Interfaces
{
    /// <summary>
    /// Interfaz que define las operaciones permitidas para la gestión de productos.
    /// </summary>
    public interface IProductoService
    {
        Task<List<ProductoDto>> GetProductosAsync();
        Task<Producto?> GetProductoByIdAsync(int id);
        Task<Producto> UpsertProductoAsync(Producto producto);
        Task<List<ProductoBusquedaDto>> SearchProductosAsync(string query);
    }
}