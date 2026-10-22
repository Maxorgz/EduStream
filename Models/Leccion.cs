using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Leccion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        public string? Contenido { get; set; }

        [Required(ErrorMessage = "El número de orden es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser un número mayor a cero.")]
        public int Orden { get; set; }

        [Required(ErrorMessage = "La lección debe pertenecer a un curso.")]
        public int CursoId { get; set; }
        [ForeignKey("CursoId")]
        public virtual Curso? Curso { get; set; }

        // Navegación
        public virtual ICollection<MaterialEstudio>? Materiales { get; set; }
        public virtual ICollection<ProgresoLeccion>? Progresos { get; set; }
    }
}