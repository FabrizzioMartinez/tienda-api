using SIOL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda.API.Models
{
    // Si tu tabla en BD se llama "detalleventa", puedes forzar el nombre aquí
    [Table("detalleventa")]
    public class DetalleVenta
    {
        [Key]
        [Column("detalleventaid")] // Mapeo exacto a tu columna de BD
        public int DetalleVentaID { get; set; }

        [Column("ventaid")]
        public int VentaID { get; set; }

        [ForeignKey("VentaID")]
        public Venta? Venta { get; set; }

        [Column("productoid")]
        public int ProductoID { get; set; }

        [ForeignKey("ProductoID")]
        public Producto? Producto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("preciounitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal", TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }

        [Column("fecharegistro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}