using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tienda.API.Models
{
    public class Cliente
    {
        [Key]
        public int ClienteID { get; set; }

        [Required]
        [MaxLength(255)]
        public string NombreRazonSocial { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TipoDocumento { get; set; } = string.Empty; // 👈 SE QUEDA COMO STRING ('01', '06' o 'DNI')

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;

        // Propiedad de navegación por texto
        public virtual MaestroTablaDetalle? MaestroTablaDetalle { get; set; }
    }
}