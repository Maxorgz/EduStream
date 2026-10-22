using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(30)]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(30)]
        public string Rol { get; set; } // 'Administrador', 'Profesor', 'Estudiante'

        [StringLength(30)]
        public string? AvatarUrl { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [NotMapped]
        public IFormFile? AvatarFile { get; set; }

        // Navegación
        public virtual ICollection<Curso>? CursosDictados { get; set; }
        public virtual ICollection<Inscripcion>? Inscripciones { get; set; }
    }
}