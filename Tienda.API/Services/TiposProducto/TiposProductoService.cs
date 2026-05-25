using Microsoft.EntityFrameworkCore;
using Tienda.API.Data;
using Tienda.API.DTOs;
using Tienda.API.DTOs.TiposProducto;
using Tienda.API.Interfaces;
using Tienda.API.Models;

namespace Tienda.API.Services
{
    public class TipoProductoService : ITipoProductoService
    {
        private readonly TiendaDbContext _context;

        public TipoProductoService(TiendaDbContext context) => _context = context;

        public async Task<List<TipoProductoDto>> GetTiposActivosAsync() =>
            await _context.TiposProducto
                .Where(t => t.Activo)
                .Select(t => new TipoProductoDto
                {
                    TipoProductoID = t.TipoProductoID,
                    Nombre = t.Nombre,
                    Activo = t.Activo,
                })
                .ToListAsync();

        public async Task<TipoProducto> UpsertTipoAsync(TipoProducto tipo)
        {
            if (tipo.TipoProductoID == 0) _context.TiposProducto.Add(tipo);
            else _context.Entry(tipo).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return tipo;
        }

        public async Task<bool> InactivarTipoAsync(int id)
        {
            var tipo = await _context.TiposProducto.FindAsync(id);
            if (tipo == null) return false;

            tipo.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}