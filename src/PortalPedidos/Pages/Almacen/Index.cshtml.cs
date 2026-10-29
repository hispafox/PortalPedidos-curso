using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages.Almacen;

// Solo entra el almacén (Program.cs, carpeta /Almacen).
public class IndexModel(ServicioPedidos pedidos) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? Numero { get; set; }

    public IReadOnlyList<Pedido> Nuevos { get; private set; } = [];

    public Pedido? Abierto { get; private set; }

    [TempData]
    public string? Aviso { get; set; }

    public void OnGet()
    {
        Nuevos = pedidos.PedidosNuevos();
        Abierto = Numero is int n ? pedidos.PorNumero(n) : null;
    }

    public IActionResult OnPostPreparado(int numero)
    {
        var resultado = pedidos.MarcarPreparado(numero);
        Aviso = resultado.Hecho ? $"Pedido {numero} marcado como preparado." : resultado.Motivo;
        return RedirectToPage();
    }
}
