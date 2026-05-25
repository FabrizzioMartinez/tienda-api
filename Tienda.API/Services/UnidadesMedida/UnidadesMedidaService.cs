using Microsoft.EntityFrameworkCore;
using Tienda.API.Data;
using Tienda.API.DTOs;
using Tienda.API.DTOs.UnidadesMedida;
using Tienda.API.Interfaces;
using Tienda.API.Models;

namespace Tienda.API.Services
{
    public class UnidadMedidaService : IUnidadMedidaService
    {
        private readonly TiendaDbContext _context;

        public UnidadMedidaService(TiendaDbContext context) => _context = context;

        public async Task<List<UnidadMedidaDto>> GetUnidadesAsync() =>
            await _context.UnidadesMedida
                .Select(u => new UnidadMedidaDto
                {
                    UnidadMedidaID = u.UnidadMedidaID,
                    Nombre = u.Nombre,
                    Abreviatura = u.Abreviatura,
                    Activo = u.Activo,
                })
                .ToListAsync();

        public async Task<UnidadMedida> UpsertUnidadAsync(UnidadMedida unidad)
        {
            if (unidad.UnidadMedidaID == 0) _context.UnidadesMedida.Add(unidad);
            else _context.Entry(unidad).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return unidad;
        }
    }
}