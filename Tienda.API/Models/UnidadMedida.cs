namespace Tienda.API.Models
{
    public class UnidadMedida
    {
        public int UnidadMedidaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}
