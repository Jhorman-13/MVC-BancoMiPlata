using Microsoft.EntityFrameworkCore;
using MiPlata.Models;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddControllersWithViews();

// Configuraci�n de la base de datos
builder.Services.AddDbContext<BancoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));

// A�adir servicios de sesi�n
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Configura el tiempo de inactividad
    options.Cookie.HttpOnly = true; // Hace que la cookie sea solo HTTP
    options.Cookie.IsEssential = true; // Asegura que la cookie es esencial
});

var app = builder.Build();

// Configurar el canal de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Si usas autenticaci�n, aseg�rate de incluir este middleware
app.UseAuthorization();

// A�adir el middleware de sesi�n
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
