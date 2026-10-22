using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InscripcionId { get; set; }
        [ForeignKey("InscripcionId")]
        public virtual Inscripcion? Inscripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } // Ej: Transferencia, Tarjeta

        [StringLength(50)]
        public string Estado { get; set; } = "Completado"; // Pendiente, Completado, Anulado

        [StringLength(255)]
        public string? ComprobanteUrl { get; set; }

        // Para recibir el archivo (foto/pdf del comprobante de transferencia) desde el formulario
        [NotMapped]
        public IFormFile? ComprobanteFile { get; set; }
    }
}