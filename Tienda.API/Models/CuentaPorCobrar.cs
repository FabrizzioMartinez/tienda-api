using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda.API.Models
{
    public class CuentaPorCobrar
    {
        [Key]
        public int CuentaCobrarID { get; set; }

        [Required]
        public int VentaID { get; set; }

        [ForeignKey("VentaID")]
        public Venta? Venta { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoTotal { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoPagado { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal SaldoPendiente { get; set; }

        // Campo nuevo: Detalle del crédito
        public string? Detalle { get; set; }

        // Campo nuevo: Registro de cuándo se creó
        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        [MaxLength(20)]
        public string Estado { get; set; } = "PENDIENTE"; // PENDIENTE, PAGADO, VENCIDO
    }
}