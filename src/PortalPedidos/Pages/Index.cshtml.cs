using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class IndexModel(Pedido pedido) : PageModel
{
    public IReadOnlyList<Producto> Productos => Catalogo.Productos;

    public int CajasEnPedido => pedido.Lineas.Sum(l => l.Cajas);

    public IActionResult OnPost(string codigo)
    {
        var producto = Catalogo.Buscar(codigo);
        if (producto is not null)
        {
            pedido.Anadir(producto);
        }

        return RedirectToPage();
    }
}
