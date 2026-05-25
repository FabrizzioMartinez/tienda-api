namespace Tienda.API.DTOs.Marcas
{
    public class MarcaDto
    {
        public int MarcaID { get; set; }
        public string? Nombre { get; set; }
        public bool Activo { get; set; }
        // Puedes agregar más campos aquí en el futuro sin romper el frontend
    }
}
