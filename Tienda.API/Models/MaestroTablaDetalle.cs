using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda.API.Models
{
    [Table("MaestroTablaDetalle")]
    public class MaestroTablaDetalle
    {
        [Key]
        public int MaestroTablaDetalleID { get; set; }

        [Required]
        public int MaestroTablaID { get; set; } // Llave foránea hacia MaestroTabla

        [StringLength(20)]
        public string? Code { get; set; } // Ejemplo: 'DNI', 'RUC'

        [Required]
        [StringLength(20)]
        public string Valor { get; set; } = string.Empty; // Ejemplo: '01', '06'

        [Required]
        [StringLength(100)]
        public string Texto { get; set; } = string.Empty; // Ejemplo: 'DNI' o 'Carnet de Extranjería'

        public bool Activo { get; set; } = true;

        // Propiedad de navegación (Indica que este detalle pertenece a un Maestro específico)
        [ForeignKey("MaestroTablaID")]
        public virtual MaestroTabla? MaestroTabla { get; set; }
    }
}