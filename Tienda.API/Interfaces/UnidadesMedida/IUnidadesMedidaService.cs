using Tienda.API.DTOs;
using Tienda.API.DTOs.UnidadesMedida;
using Tienda.API.Models;

namespace Tienda.API.Interfaces
{
    public interface IUnidadMedidaService
    {
        Task<List<UnidadMedidaDto>> GetUnidadesAsync();
        Task<UnidadMedida> UpsertUnidadAsync(UnidadMedida unidad);
    }
}