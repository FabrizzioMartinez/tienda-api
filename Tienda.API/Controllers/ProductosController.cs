using Microsoft.AspNetCore.Mvc;
using SIOL.Models;
using Tienda.API.DTOs;
using Tienda.API.DTOs.Producto.Tienda.API.DTOs;
using Tienda.API.Interfaces;
using Tienda.API.Models;

namespace Tienda.API.Controllers
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: api/productos/listado
        [HttpGet("listado")]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _productoService.GetProductosAsync();
            return Ok(new ApiResponse<List<ProductoDto>>(productos, "Lista de productos obtenida"));
        }

        // GET: api/productos/detalle/{id}
        [HttpGet("detalle/{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            var producto = await _productoService.GetProductoByIdAsync(id);
            if (producto == null)
                return NotFound(new ApiResponse<object>(null, $"Producto con ID {id} no encontrado"));

            return Ok(new ApiResponse<Producto>(producto, "Producto encontrado"));
        }

        // POST: api/productos/guardar
        [HttpPost("guardar")]
        public async Task<IActionResult> PostProducto([FromBody] Producto producto)
        {
            try
            {
                var result = await _productoService.UpsertProductoAsync(producto);
                return Ok(new ApiResponse<Producto>(result, "Producto guardado correctamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(null, $"Error al guardar: {ex.Message}"));
            }
        }

        // GET: api/productos/buscar
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarProductos([FromQuery(Name = "q")] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new ApiResponse<List<ProductoBusquedaDto>>(null, "El término de búsqueda es requerido"));

            var result = await _productoService.SearchProductosAsync(query);
            return Ok(new ApiResponse<List<ProductoBusquedaDto>>(result, "Búsqueda exitosa"));
        }
    }
}