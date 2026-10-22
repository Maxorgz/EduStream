using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class ProgresoLeccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InscripcionId { get; set; }
        [ForeignKey("InscripcionId")]
        public virtual Inscripcion? Inscripcion { get; set; }

        [Required]
        public int LeccionId { get; set; }
        [ForeignKey("LeccionId")]
        public virtual Leccion? Leccion { get; set; }

        public bool Completada { get; set; } = false;

        public DateTime FechaUltimoAcceso { get; set; } = DateTime.Now;
    }
}