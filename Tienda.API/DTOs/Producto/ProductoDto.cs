namespace Tienda.API.DTOs
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? NombreMarca { get; set; }
        public string? NombreTipo { get; set; }
        public string? TipoProducto { get; set; }
        public string? UnidadMedida { get; set; }
        public string? Abreviatura { get; set; }
        public int StockMinimo { get; set; }

        // 🌟 AGREGA ESTOS CAMPOS PARA CARGAR LOS COMBOS EN ANGULAR
        public int? MarcaID { get; set; }
        public int? TipoProductoID { get; set; }
        public int? UnidadMedidaID { get; set; }
    }
}