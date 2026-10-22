using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Leccion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200)]
        public string Titulo { get; set; }

        public string? Contenido { get; set; }

        [Required]
        public int Orden { get; set; }

        [Required]
        public int CursoId { get; set; }
        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }

        // Navegación
        public virtual ICollection<MaterialEstudio>? Materiales { get; set; }
        public virtual ICollection<ProgresoLeccion>? Progresos { get; set; }
    }
}