using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tienda.API.DTOs;
using Tienda.API.DTOs.MaestroTabla;
using Tienda.API.Interfaces.MaestroTabla; // 👈 Importamos tu interfaz desde su ruta exacta

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaestrosController : ControllerBase
    {
        private readonly IMaestroService _maestroService;

        // Inyectamos el servicio mediante el constructor
        public MaestrosController(IMaestroService maestroService)
        {
            _maestroService = maestroService;
        }

        /// <summary>
        /// Obtiene los elementos de un catálogo maestro usando su código identificador.
        /// </summary>
        /// <param name="codigo">Ejemplo: TIPO_DOC, ESTADO_VENTA, etc.</param>
        // GET: api/maestros/detalles/TIPO_DOC
        [HttpGet("detalles/{codigo}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MaestroTablaDetalleDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MaestroTablaDetalleDto>>> GetDetalles(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest("El código del maestro es requerido.");
            }

            var detalles = await _maestroService.ObtenerDetallesPorCodigoAsync(codigo);

            return Ok(detalles);
        }
    }
}