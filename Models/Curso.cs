using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Curso
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El título del curso es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0, 999999.99, ErrorMessage = "El precio debe ser un valor positivo.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser al menos 1.")]
        public int? CupoMaximo { get; set; }

        [StringLength(255)]
        public string? PortadaUrl { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Activo";

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        [Required(ErrorMessage = "Debe asignar un profesor al curso.")]
        public int ProfesorId { get; set; }
        [ForeignKey("ProfesorId")]
        public virtual Usuario? Profesor { get; set; }

        [NotMapped]
        public IFormFile? PortadaFile { get; set; }

        // Navegación
        public virtual ICollection<Leccion>? Lecciones { get; set; }
        public virtual ICollection<Inscripcion>? Inscripciones { get; set; }
    }
}