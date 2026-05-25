namespace Tienda.API.DTOs.Producto
{
    namespace Tienda.API.DTOs
    {
        public class ProductoBusquedaDto
        {
            public int ProductoID { get; set; }
            public string? Nombre { get; set; }
            public decimal Precio { get; set; }
            public int? Stock { get; set; }
            // Solo incluimos lo que el usuario necesita ver en la lista de resultados
        }
    }
}
