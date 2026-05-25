using System.ComponentModel.DataAnnotations;

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
        [MaxLength(10)]
        public string TipoDocumento { get; set; } = string.Empty; // DNI, RUC

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public bool Activo { get; set; } = true;
    }
}