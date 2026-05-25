using Microsoft.AspNetCore.Mvc;
using Tienda.API.DTOs;
using Tienda.API.DTOs.Marcas;
using Tienda.API.Interfaces;
using Tienda.API.Models;

[ApiController]
[Route("api/marcas")]
public class MarcasController : ControllerBase
{
    private readonly IMarcaService _marcaService;

    public MarcasController(IMarcaService marcaService) => _marcaService = marcaService;

    // Obtiene solo las marcas habilitadas para la venta/uso
    [HttpGet("activas")]
    public async Task<IActionResult> ObtenerMarcasActivas()
    {
        var marcas = await _marcaService.GetMarcasActivasAsync();
        return Ok(new ApiResponse<List<MarcaDto>>(marcas, "Listado de marcas activas recuperado exitosamente."));
    }

    // Crea o actualiza una marca (Upsert)
    [HttpPost("guardar")]
    public async Task<IActionResult> RegistrarOActualizarMarca([FromBody] Marca marca)
    {
        if (string.IsNullOrWhiteSpace(marca.Nombre))
            return BadRequest(new ApiResponse<Marca>(null, "El nombre de la marca es obligatorio."));

        var resultado = await _marcaService.UpsertMarcaAsync(marca);
        return Ok(new ApiResponse<Marca>(resultado, "La marca ha sido registrada/actualizada correctamente."));
    }

    // Marca una marca como inactiva (Borrado lógico)
    [HttpPut("{id}/deshabilitar")]
    public async Task<IActionResult> DeshabilitarMarca(int id)
    {
        var fueExitoso = await _marcaService.InactivarMarcaAsync(id);

        if (!fueExitoso)
            return NotFound(new ApiResponse<string>(null, $"No se pudo encontrar la marca con ID {id} para deshabilitar."));

        return Ok(new ApiResponse<string>(null, "La marca ha sido deshabilitada del sistema correctamente."));
    }
}