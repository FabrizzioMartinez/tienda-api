namespace Tienda.API.DTOs
{
    public class ClienteDto
    {
        public int ClienteID { get; set; }
        public string NombreRazonSocial { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}