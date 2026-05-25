namespace Tienda.API.Models
{
    public class Marca
    {
        public int MarcaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
