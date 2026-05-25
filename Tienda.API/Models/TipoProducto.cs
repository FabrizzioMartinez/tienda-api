namespace Tienda.API.Models
{
    public class TipoProducto
    {
        public int TipoProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
