namespace Tienda.API.DTOs.UnidadesMedida
{
    public class UnidadMedidaDto
    {
        public int UnidadMedidaID { get; set; }
        public string? Nombre { get; set; }
        public string? Abreviatura { get; set; }
        public bool Activo { get; set; }
    }
}
