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
            try
            {
                return await _context.Clientes
                    .Include(c => c.MaestroTablaDetalle)
                    .Where(c => c.Activo && !(c.NombreRazonSocial.ToLower() == "admin" && c.NumeroDocumento == "00000000"))
                    .Select(c => new ClienteDto
                    {
                        ClienteID = c.ClienteID,
                        NombreRazonSocial = c.NombreRazonSocial != null ? c.NombreRazonSocial.ToUpper() : string.Empty,
                        NumeroDocumento = c.NumeroDocumento,

                        // 🚀 Asignamos el valor de la BD al nuevo nombre del DTO
                        TipoDocumentoCode = c.TipoDocumento,
                        TipoDocumentoTexto = (c.MaestroTablaDetalle.Texto ?? c.TipoDocumento).ToUpper(),

                        Email = c.Email != null ? c.Email.ToUpper() : string.Empty,
                        Telefono = c.Telefono
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error en ObtenerTodosAsync]: {ex.Message}");
                throw;
            }
        }

        public async Task<ApiResponse<ClienteDto>> RegistrarOEditarAsync(ClienteDto dto)
        {
            bool esEdicion = dto.ClienteID > 0;
            var existeDuplicado = await _context.Clientes
                .AnyAsync(c => c.NumeroDocumento == dto.NumeroDocumento && (!esEdicion || c.ClienteID != dto.ClienteID));

            if (existeDuplicado)
            {
                return new ApiResponse<ClienteDto>("Ya existe un cliente con el mismo número de documento");
            }

            Models.Cliente? cliente;

            if (esEdicion)
            {
                cliente = await _context.Clientes.FindAsync(dto.ClienteID);
                if (cliente == null)
                {
                    return new ApiResponse<ClienteDto>("El cliente a editar no existe");
                }
                cliente.NombreRazonSocial = dto.NombreRazonSocial ?? string.Empty;
                cliente.NumeroDocumento = dto.NumeroDocumento ?? string.Empty;
                cliente.TipoDocumento = dto.TipoDocumentoCode ?? string.Empty;
                cliente.Email = dto.Email;
                cliente.Telefono = dto.Telefono;

                _context.Clientes.Update(cliente);
            }
            else
            {
                cliente = new Models.Cliente
                {
                    NombreRazonSocial = dto.NombreRazonSocial ?? string.Empty,
                    NumeroDocumento = dto.NumeroDocumento ?? string.Empty,
                    TipoDocumento = dto.TipoDocumentoCode ?? string.Empty,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    Activo = true
                };

                _context.Clientes.Add(cliente);
            }
            await _context.SaveChangesAsync();
            if (!esEdicion)
            {
                dto.ClienteID = cliente.ClienteID;
            }

            string mensajeExito = esEdicion ? "Cliente actualizado correctamente" : "Cliente registrado correctamente";
            return new ApiResponse<ClienteDto>(dto, mensajeExito);
        }
    }
}