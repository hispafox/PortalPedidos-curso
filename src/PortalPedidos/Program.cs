using System.Globalization;
using PortalPedidos.Dominio;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-ES");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("es-ES");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Una sola tienda de pruebas: el pedido en curso y los confirmados viven en memoria.
builder.Services.AddSingleton<Pedido>();
builder.Services.AddSingleton<RegistroPedidos>();
builder.Services.AddSingleton(new ConsultaExistencias(ConsultaExistencias.AlmacenDePrueba));

var app = builder.Build();

app.UseRouting();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
