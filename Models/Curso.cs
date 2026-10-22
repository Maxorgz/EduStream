using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Curso
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        public int? CupoMaximo { get; set; }

        [StringLength(255)]
        public string? PortadaUrl { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Activo";

        [Required]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria? Categoria { get; set; }

        [Required]
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