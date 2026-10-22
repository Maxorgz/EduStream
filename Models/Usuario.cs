using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(30, ErrorMessage = "El nombre no puede superar los 30 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(30, ErrorMessage = "El apellido no puede superar los 30 caracteres.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(20, ErrorMessage = "El DNI no puede superar los 20 caracteres.")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.", MinimumLength = 6)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(30)]
        public string Rol { get; set; } = "Estudiante"; // aca agregamos los roles 'Administrador', 'Profesor', 'Estudiante'

        [StringLength(255)]
        public string? AvatarUrl { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [NotMapped]
        [Display(Name = "Imagen de Avatar")]
        public IFormFile? AvatarFile { get; set; }

        public virtual ICollection<Curso> CursosDictados { get; set; } = new List<Curso>();
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}