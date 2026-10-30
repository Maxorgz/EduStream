using EduStream.Models;

namespace EduStream.Data
{
    public static class DbInitializer
    {
        public static void Initialize(EduStreamDbContext context)
        {
            // Verifica si ya existen usuarios para no duplicarlos cada vez que compilas
            if (context.Usuarios.Any())
            {
                return; 
            }

            var usuarios = new Usuario[]
            {
                new Usuario 
                { 
                    Nombre = "Admin", 
                    Apellido = "Sistema", 
                    Email = "admin@edustream.com", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), 
                    Rol = "Administrador", 
                    Activo = true, 
                    FechaRegistro = DateTime.Now 
                },
                new Usuario 
                { 
                    Nombre = "Juan", 
                    Apellido = "Profesor", 
                    Email = "profe@edustream.com", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Profe123!"), 
                    Rol = "Profesor", 
                    Activo = true, 
                    FechaRegistro = DateTime.Now 
                },
                new Usuario 
                { 
                    Nombre = "Lucas", 
                    Apellido = "Estudiante", 
                    Email = "alumno@edustream.com", 
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Alumno123!"), 
                    Rol = "Estudiante", 
                    Activo = true, 
                    FechaRegistro = DateTime.Now 
                }
            };

            context.Usuarios.AddRange(usuarios);
            context.SaveChanges();
        }
    }
}