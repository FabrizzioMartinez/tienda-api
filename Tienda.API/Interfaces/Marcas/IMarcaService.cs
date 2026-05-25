using Tienda.API.DTOs.Marcas;
using Tienda.API.Models;

namespace Tienda.API.Interfaces
{
    public interface IMarcaService
    {
        Task<List<MarcaDto>> GetMarcasActivasAsync();
        Task<Marca> UpsertMarcaAsync(Marca marca);
        Task<bool> InactivarMarcaAsync(int id);
    }
}