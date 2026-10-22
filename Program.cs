using Microsoft.EntityFrameworkCore;
using EduStream.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios al contenedor (ANTES del Build)
builder.Services.AddControllersWithViews();

// Opción A: Si instalaste Pomelo para conectar a MySQL en DBeaver
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EduStreamDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Opción B: Si usas SQL Server, comenta las dos líneas de arriba y descomenta la de abajo
// builder.Services.AddDbContext<EduStreamDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 2. Construir 
var app = builder.Build();

// 3. Configurar el pipeline HTTP
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