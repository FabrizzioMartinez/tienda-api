using Microsoft.AspNetCore.Mvc;
using Tienda.API.DTOs;
using Tienda.API.DTOs.UnidadesMedida;
using Tienda.API.Interfaces;
using Tienda.API.Models;

[ApiController]
[Route("api/unidadesmedida")]
public class UnidadesMedidaController : ControllerBase
{
    private readonly IUnidadMedidaService _service;

    public UnidadesMedidaController(IUnidadMedidaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> ObtenerUnidades()
    {
        var lista = await _service.GetUnidadesAsync();
        return Ok(new ApiResponse<List<UnidadMedidaDto>>(lista, "Unidades recuperadas."));
    }

    [HttpPost("guardar")]
    public async Task<IActionResult> RegistrarOActualizarUnidad([FromBody] UnidadMedida unidad)
    {
        if (string.IsNullOrWhiteSpace(unidad.Nombre) || string.IsNullOrWhiteSpace(unidad.Abreviatura))
            return BadRequest(new ApiResponse<UnidadMedida>(null, "Nombre y Abreviatura son obligatorios."));

        var result = await _service.UpsertUnidadAsync(unidad);
        return Ok(new ApiResponse<UnidadMedida>(result, "Unidad guardada correctamente."));
    }
}