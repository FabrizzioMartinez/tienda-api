using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.API.Data;
using Tienda.API.DTOs;
using Tienda.API.DTOs.MaestroTabla;
using Tienda.API.Interfaces.MaestroTabla; // 👈 Importamos tu interfaz

namespace Tienda.API.Services.MaestroTabla
{
    public class MaestroService : IMaestroService
    {
        private readonly TiendaDbContext _context;

        public MaestroService(TiendaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaestroTablaDetalleDto>> ObtenerDetallesPorCodigoAsync(string codigoMaestro)
        {
            return await _context.MaestroDetalles
                .Include(d => d.MaestroTabla)
                .Where(d => d.Activo
                         && d.MaestroTabla!.Activo
                         && d.MaestroTabla.Codigo.ToUpper() == codigoMaestro.ToUpper())
                .Select(d => new MaestroTablaDetalleDto
                {
                    MaestroTablaDetalleID = d.MaestroTablaDetalleID,
                    MaestroTablaID = d.MaestroTablaID,
                    Code = d.Code,
                    Valor = d.Valor,
                    Texto = d.Texto.ToUpper(),
                    Activo = d.Activo
                })
                .ToListAsync();
        }
    }
}