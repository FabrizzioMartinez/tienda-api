using Microsoft.EntityFrameworkCore;
using Tienda.API.Data;
using Tienda.API.DTOs;
using Tienda.API.Interfaces; // Asegúrate de tener la interfaz IClienteService aquí
using Tienda.API.Interfaces.Cliente;
using Tienda.API.Models;

namespace Tienda.API.Services.Cliente
{
    public class ClienteService : IClienteService
    {
        private readonly TiendaDbContext _context;

        public ClienteService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClienteDto>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .Where(c => c.Activo)
                .Select(c => new ClienteDto
                {
                    ClienteID = c.ClienteID,
                    NombreRazonSocial = c.NombreRazonSocial,
                    NumeroDocumento = c.NumeroDocumento,
                    TipoDocumento = c.TipoDocumento,
                    Email = c.Email,
                    Telefono = c.Telefono
                })
                .ToListAsync();
        }

        public async Task<ApiResponse<ClienteDto>> RegistrarAsync(ClienteDto dto)
        {
            var existe = await _context.Clientes
                .AnyAsync(c => c.NumeroDocumento == dto.NumeroDocumento);

            if (existe)
            {
                return new ApiResponse<ClienteDto>("El cliente ya está registrado");
            }

            var cliente = new Models.Cliente
            {
                NombreRazonSocial = dto.NombreRazonSocial,
                NumeroDocumento = dto.NumeroDocumento,
                TipoDocumento = dto.TipoDocumento,
                Email = dto.Email,
                Telefono = dto.Telefono,
                Activo = true
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            dto.ClienteID = cliente.ClienteID;

            return new ApiResponse<ClienteDto>(dto, "Cliente registrado correctamente");
        }
    }
}