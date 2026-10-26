using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class ConfirmacionModel(IWebHostEnvironment entorno) : PageModel
{
    public string Texto { get; private set; } = "";

    public void OnGet(int numero, DateOnly dia)
    {
        var ruta = Path.Combine(entorno.ContentRootPath, "Pantallas", "confirmacion.html");
        Texto = PlantillaConfirmacion.Rellenar(System.IO.File.ReadAllText(ruta), numero, dia);
    }
}
