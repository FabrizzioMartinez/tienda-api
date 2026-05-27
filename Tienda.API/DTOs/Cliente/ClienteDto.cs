namespace Tienda.API.DTOs
{
    public class ClienteDto
    {
        public int ClienteID { get; set; }
        public string NombreRazonSocial { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;

        // 🔑 Nombres limpios y claros:
        public string TipoDocumentoCode { get; set; } = string.Empty; // Recibe '01', '06'
        public string TipoDocumentoTexto { get; set; } = string.Empty; // Recibe 'DNI', 'RUC'

        public string? Email { get; set; }
        public string? Telefono { get; set; }
    }
}