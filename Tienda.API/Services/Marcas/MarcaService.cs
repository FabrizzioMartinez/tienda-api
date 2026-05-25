using Microsoft.EntityFrameworkCore;
using Tienda.API.Data;
using Tienda.API.DTOs.Marcas;
using Tienda.API.Interfaces;
using Tienda.API.Models;

namespace Tienda.API.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly TiendaDbContext _context;

        public MarcaService(TiendaDbContext context) => _context = context;

        public async Task<List<MarcaDto>> GetMarcasActivasAsync()
        {
            return await _context.Marcas
                .Where(m => m.Activo)
                .Select(m => new MarcaDto
                {
                    MarcaID = m.MarcaID,
                    Nombre = m.Nombre,
                    Activo = m.Activo,
                })
                .ToListAsync();
        }

        public async Task<Marca> UpsertMarcaAsync(Marca marca)
        {
            if (marca.MarcaID == 0) _context.Marcas.Add(marca);
            else _context.Entry(marca).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return marca;
        }

        public async Task<bool> InactivarMarcaAsync(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null) return false;

            marca.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}