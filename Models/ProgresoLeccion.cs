using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    [Table("ProgresosLecciones")]
    public class ProgresoLeccion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La inscripcion es obligatoria.")]
        [Display(Name = "Inscripción")]
        public int InscripcionId { get; set; }

        [ForeignKey("InscripcionId")]
        public virtual Inscripcion? Inscripcion { get; set; }

        [Required(ErrorMessage = "La leccion es obligatoria.")]
        [Display(Name = "Leccion")]
        public int LeccionId { get; set; }

        [ForeignKey("LeccionId")]
        public virtual Leccion? Leccion { get; set; }

        [Required]
        [Display(Name = "¿Completada?")]
        public bool Completada { get; set; } = false;

        [Required(ErrorMessage = "La fecha del ultimo acceso es obligatoria.")]
        [Display(Name = "Fecha de ultimo Acceso")]
        [DataType(DataType.DateTime)]
        public DateTime FechaUltimoAcceso { get; set; } = DateTime.Now;
    }
}