using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PortalPedidos.Datos;
using PortalPedidos.Dominio;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("es-ES");

var builder = WebApplication.CreateBuilder(args);

// Entrada simulada: una cookie con el usuario y su papel, sin ASP.NET Core Identity.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Entrar";
        o.AccessDeniedPath = "/Entrar";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

builder.Services.AddRazorPages(o =>
{
    foreach (var pagina in new[] { "/Index", "/Pedido", "/Confirmacion", "/Historial", "/Existencias" })
    {
        o.Conventions.AuthorizePage(pagina, nameof(Rol.Tienda));
    }

    o.Conventions.AuthorizeFolder("/Almacen", nameof(Rol.Almacen));
    o.Conventions.AllowAnonymousToPage("/Entrar");
    o.Conventions.AllowAnonymousToPage("/Salir");
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(nameof(Rol.Tienda), p => p.RequireRole(nameof(Rol.Tienda)))
    .AddPolicy(nameof(Rol.Almacen), p => p.RequireRole(nameof(Rol.Almacen)));

// Todo en memoria. Para pasar a una base de datos de verdad, se cambia el proveedor aquí.
builder.Services.AddDbContext<PortalContexto>(o => o.UseInMemoryDatabase("PortalPedidos"));
builder.Services.AddSingleton(builder.Configuration.GetSection("Portal").Get<OpcionesPortal>() ?? new OpcionesPortal());
builder.Services.AddSingleton<ColaAlmacen>();
builder.Services.AddScoped<AplicacionAlmacen>();
builder.Services.AddScoped<Catalogo>();
builder.Services.AddScoped<ServicioPedidos>();
builder.Services.AddScoped<Acceso>();

var app = builder.Build();

using (var ambito = app.Services.CreateScope())
{
    DatosDePrueba.Sembrar(ambito.ServiceProvider.GetRequiredService<PortalContexto>());
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
