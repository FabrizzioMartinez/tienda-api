using Microsoft.AspNetCore.Mvc;
using Tienda.API.DTOs;
using Tienda.API.Interfaces.Venta;

namespace Tienda.API.Controllers
{
    [Route("api/ventas")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // POST: api/ventas/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaCreateDto ventaDto)
        {
            if (ventaDto == null || ventaDto.Detalles.Count == 0)
            {
                return BadRequest("La venta debe tener al menos un producto.");
            }

            try
            {
                var resultado = await _ventaService.RegistrarVentaAsync(ventaDto);

                if (resultado)
                {
                    return Ok(new { mensaje = "Venta registrada exitosamente" });
                }

                return StatusCode(500, "Ocurrió un error al procesar la venta en la base de datos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/ventas/por-fecha?fecha=2026-05-25
        [HttpGet("por-fecha")]
        public async Task<IActionResult> GetVentasPorFecha([FromQuery] DateTime fecha)
        {
            try
            {
                var ventas = await _ventaService.ObtenerVentasPorFechaAsync(fecha);

                // Retornamos el objeto con la propiedad 'data' para que coincida con tu servicio
                return Ok(new { data = ventas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las ventas: {ex.Message}");
            }
        }
        [HttpGet("filtrar")]
        public async Task<IActionResult> GetVentasFiltro( [FromQuery] DateTime? fecha = null,[FromQuery] DateTime? fechaHasta = null,[FromQuery] int? productoId = null)
        {
            try
            {
                var ventas = await _ventaService.ObtenerVentasFiltroAsync(fecha, fechaHasta, productoId);

                return Ok(new { data = ventas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las ventas: {ex.Message}");
            }
        }

        [HttpDelete("anular/{id}")]
        public async Task<IActionResult> AnularVenta(int id)
        {
            var resultado = await _ventaService.AnularVentaAsync(id);
            if (!resultado)
            {
                return BadRequest(new { mensaje = "No se pudo anular la venta o no fue encontrada." });
            }
            return Ok(new { mensaje = "Venta anulada con éxito y stock restaurado." });
        }

    }
}