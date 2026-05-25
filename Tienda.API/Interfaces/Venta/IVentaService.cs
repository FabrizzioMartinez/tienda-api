using Tienda.API.DTOs;

namespace Tienda.API.Interfaces.Venta
{
    public interface IVentaService
    {
        // El método devuelve un booleano para indicar éxito o fallo en la transacción
        Task<bool> RegistrarVentaAsync(VentaCreateDto ventaDto);
        Task<ClienteDto> ObtenerOCrearClienteAsync(string nombre, string numeroDocumento, string tipoDocumento);
        Task<List<VentaDto>> ObtenerVentasPorFechaAsync(DateTime fecha);
    }
}