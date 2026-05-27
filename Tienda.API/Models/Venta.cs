using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda.API.Models
{
    public class Venta
    {
        [Key]
        public int VentaID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [ForeignKey("ClienteID")]
        public Cliente? Cliente { get; set; }

        [Required]
        [MaxLength(50)]
        public string TipoComprobante { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? NumeroComprobante { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Total { get; set; }

        public bool EsCredito { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool EsEfectivo { get; set; } = false;

        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoEfectivo { get; set; } = 0.00m;

        public bool EsDigital { get; set; } = false;

        [Column(TypeName = "decimal(12,2)")]
        public decimal MontoDigital { get; set; } = 0.00m;

        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}