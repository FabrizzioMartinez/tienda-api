using Tienda.API.DTOs.MaestroTabla;

namespace Tienda.API.DTOs.MaestrosTabla
{
    public class MaestroTablaDto
    {
        public int MaestroTablaID { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Activo { get; set; }

        // Lista de detalles anidada
        public List<MaestroTablaDetalleDto> Detalles { get; set; } = new List<MaestroTablaDetalleDto>();
    }
}
