using Tienda.API.DTOs;
using Tienda.API.DTOs.TiposProducto;
using Tienda.API.Models;

namespace Tienda.API.Interfaces
{
    public interface ITipoProductoService
    {
        Task<List<TipoProductoDto>> GetTiposActivosAsync();
        Task<TipoProducto> UpsertTipoAsync(TipoProducto tipo);
        Task<bool> InactivarTipoAsync(int id);
    }
}