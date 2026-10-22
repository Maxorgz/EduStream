using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    [Table("MaterialesEstudio")]
    public class MaterialEstudio
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El titulo del material es obligatorio.")]
        [StringLength(150, ErrorMessage = "El titulo no puede superar los 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La URL o ruta del archivo es obligatoria.")]
        public string ArchivoUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe especificar el tipo de archivo.")]
        [StringLength(50)]
        public string TipoArchivo { get; set; } = string.Empty; // puede ser PDF, Video, Enlace, Documento

        [Required(ErrorMessage = "El material debe pertenecer a una lección.")]
        [Display(Name = "Lección")]
        public int LeccionId { get; set; }

        [ForeignKey("LeccionId")]
        public virtual Leccion? Leccion { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}