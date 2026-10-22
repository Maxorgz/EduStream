using System.ComponentModel.DataAnnotations;

namespace EduStream.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoria es obligatoria")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true; // true = Activa, false = Inactiva

        // Navegación
        public virtual ICollection<Curso>? Cursos { get; set; }
    }
}