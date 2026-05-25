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
    }
}