using Microsoft.AspNetCore.Mvc;
using Tienda.API.DTOs;

namespace Tienda.API.Interfaces.Cliente
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> ObtenerTodosAsync();
        Task<ApiResponse<ClienteDto>> RegistrarOEditarAsync(ClienteDto dto);
    }
}