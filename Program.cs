using Microsoft.EntityFrameworkCore;
using EduStream.Data;
using EduStream.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios 
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// Conectar a MySQL en DBeaver
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EduStreamDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddScoped<IRepositorioInscripcion, RepositorioInscripcion>();
builder.Services.AddScoped<IRepositorioProgresoLeccion, RepositorioProgresoLeccion>();



// Construir 
var app = builder.Build();

// pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();