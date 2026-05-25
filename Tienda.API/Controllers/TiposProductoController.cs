using Microsoft.AspNetCore.Mvc;
using Tienda.API.DTOs;
using Tienda.API.DTOs.TiposProducto;
using Tienda.API.Interfaces;
using Tienda.API.Models;

[ApiController]
[Route("api/tiposproducto")]
public class TiposProductoController : ControllerBase
{
    private readonly ITipoProductoService _service;

    public TiposProductoController(ITipoProductoService service) => _service = service;

    [HttpGet("activos")]
    public async Task<IActionResult> ObtenerTiposActivos()
    {
        var lista = await _service.GetTiposActivosAsync();
        return Ok(new ApiResponse<List<TipoProductoDto>>(lista, "Clasificaciones recuperadas exitosamente."));
    }

    [HttpPost("guardar")]
    public async Task<IActionResult> RegistrarOActualizarTipo([FromBody] TipoProducto tipo)
    {
        if (string.IsNullOrWhiteSpace(tipo.Nombre))
            return BadRequest(new ApiResponse<TipoProducto>(null, "El nombre es obligatorio."));

        var result = await _service.UpsertTipoAsync(tipo);
        return Ok(new ApiResponse<TipoProducto>(result, "Clasificación guardada con éxito."));
    }

    [HttpPut("{id}/inactivar")]
    public async Task<IActionResult> InactivarTipo(int id)
    {
        var exito = await _service.InactivarTipoAsync(id);
        return exito ? Ok(new { mensaje = "Clasificación desactivada correctamente." })
                     : NotFound(new { mensaje = "No se encontró la clasificación." });
    }
}