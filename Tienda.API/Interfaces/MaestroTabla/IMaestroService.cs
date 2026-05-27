using System.Collections.Generic;
using System.Threading.Tasks;
using Tienda.API.DTOs;
using Tienda.API.DTOs.MaestroTabla;

namespace Tienda.API.Interfaces.MaestroTabla
{
    public interface IMaestroService
    {
        // Obtiene la lista de detalles (DNI, RUC, etc.) activos mediante el código del maestro
        Task<IEnumerable<MaestroTablaDetalleDto>> ObtenerDetallesPorCodigoAsync(string codigoMaestro);
    }
}