using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Inscripcion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EstudianteId { get; set; }
        [ForeignKey("EstudianteId")]
        public virtual Usuario? Estudiante { get; set; }

        [Required]
        public int CursoId { get; set; }
        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Estado { get; set; } = "Activa";

        // Navegación
        public virtual ICollection<Pago>? Pagos { get; set; }
        public virtual ICollection<ProgresoLeccion>? Progresos { get; set; }
    }
}