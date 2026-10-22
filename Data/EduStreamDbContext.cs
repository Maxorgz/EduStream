using EduStream.Models;
using Microsoft.EntityFrameworkCore;

namespace EduStream.Data
{
    public class EduStreamDbContext : DbContext
    {
        public EduStreamDbContext(DbContextOptions<EduStreamDbContext> options) : base(options)
        {
        }

        // DbSets (Tablas)
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<Leccion> Lecciones { get; set; }
        public DbSet<MaterialEstudio> MaterialesEstudio { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<ProgresoLeccion> ProgresosLecciones { get; set; }

        // Configuraciones adicionales usando Fluent API si fueran necesarias
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}