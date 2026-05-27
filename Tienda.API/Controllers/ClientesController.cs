using Microsoft.AspNetCore.Mvc;
using Tienda.API.DTOs;
using Tienda.API.Interfaces.Cliente;

namespace Tienda.API.Controllers
{
    [Route("api/clientes")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClienteDto>>>> ListarClientes()
        {
            var clientes = await _clienteService.ObtenerTodosAsync();

            return Ok(new ApiResponse<IEnumerable<ClienteDto>>(clientes));
        }

        // POST: api/clientes/registrar
        [HttpPost("registrar")]
        public async Task<IActionResult> Guardar(ClienteDto dto)
        {

            var response = await _clienteService.RegistrarOEditarAsync(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}