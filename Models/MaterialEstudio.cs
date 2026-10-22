using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduStream.Models
{
    public class MaterialEstudio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreArchivo { get; set; }

        [Required]
        [StringLength(10)]
        public string Extension { get; set; }

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.Now;

        [Required]
        public int LeccionId { get; set; }
        [ForeignKey("LeccionId")]
        public virtual Leccion? Leccion { get; set; }

        // Archivo que se procesado en el controlador (PDF, Word, ZIP, etc.)
        [NotMapped]
        public IFormFile? ArchivoFisico { get; set; }
    }
}