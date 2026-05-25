namespace Tienda.API.DTOs
{

    public class VentaDto
    {
        public int VentaID { get; set; }
        public int ClienteID { get; set; }
        public string ClienteNombre { get; set; } = string.Empty; // Para mostrar el nombre fácilmente
        public string TipoComprobante { get; set; } = string.Empty;
        public string NumeroComprobante { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public bool EsCredito { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Incluimos los detalles para tener la venta completa
        public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
    }
    public class VentaCreateDto
    {
        public int ClienteID { get; set; }
        public string ClienteNombre { get; set; } = string.Empty; // Para mostrar el nombre fácilmente
        public string TipoComprobante { get; set; } = string.Empty;
        public string NumeroComprobante { get; set; } = string.Empty;
        
        public decimal Total { get; set; }
        public bool EsCredito { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
    }

    public class DetalleVentaDto
    {
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}