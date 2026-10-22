using System.ComponentModel.DataAnnotations;

namespace EduStream.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoria es obligatoria")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; } = true; // true = Activa, false = Inactiva

        // Navegación
        public virtual ICollection<Curso>? Cursos { get; set; }
    }
}