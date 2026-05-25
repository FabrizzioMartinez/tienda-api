using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tienda.API.Models;

namespace SIOL.Models
{
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int StockMinimo { get; set; }
        public string? ImagenUrl { get; set; }
        public int? CategoriaID { get; set; }
        public string? CodigoBarras { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow; // <-- Cambia DateTime.Now por UtcNow
        public int? MarcaID { get; set; }
        [ForeignKey("MarcaID")]
        public virtual Marca? Marca { get; set; }
        public int? TipoProductoID { get; set; }
        [ForeignKey("TipoProductoID")]
        public virtual TipoProducto? TipoProducto { get; set; }
        public int? UnidadMedidaID { get; set; }
        [ForeignKey("UnidadMedidaID")]
        public virtual UnidadMedida? UnidadMedida { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorVolumen { get; set; }
    }
}