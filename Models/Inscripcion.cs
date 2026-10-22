using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El estudiante es obligatorio.")]
        public int EstudianteId { get; set; }
        [ForeignKey("EstudianteId")]
        public virtual Usuario? Estudiante { get; set; }

        [Required(ErrorMessage = "El curso es obligatorio.")]
        public int CursoId { get; set; }
        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }

        [Required]
        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Activa";

        [Required(ErrorMessage = "El monto del pago es obligatorio.")]
        [Range(0, 999999.99, ErrorMessage = "El monto de pago no puede ser negativo.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MontoPago { get; set; }

        [Required]
        [StringLength(50)]
        public string EstadoPago { get; set; } = "Pendiente";
        // Navegación
        
        public virtual ICollection<ProgresoLeccion>? Progresos { get; set; }
    }
}