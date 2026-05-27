using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda.API.Models
{
    [Table("MaestroTabla")]
    public class MaestroTabla
    {
        [Key]
        public int MaestroTablaID { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty; // Ejemplo: 'TIPO_DOC'

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty; // Ejemplo: 'Tipos de Documento'

        public bool Activo { get; set; } = true;

        // Propiedad de navegación (Relación de uno a muchos: Un Maestro tiene muchos Detalles)
        public virtual ICollection<MaestroTablaDetalle> Detalles { get; set; } = new List<MaestroTablaDetalle>();
    }
}