namespace Tienda.API.DTOs.MaestroTabla
{
    public class MaestroTablaDetalleDto
    {
        public int MaestroTablaDetalleID { get; set; }
        public int MaestroTablaID { get; set; }
        public string? Code { get; set; }
        public string Valor { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
