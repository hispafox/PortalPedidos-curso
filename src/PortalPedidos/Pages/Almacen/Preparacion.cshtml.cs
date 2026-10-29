using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages.Almacen;

// La lista de preparación de la aplicación del almacén, simulada.
public class PreparacionModel(AplicacionAlmacen aplicacion, ColaAlmacen cola) : PageModel
{
    public IReadOnlyList<PedidoEnAlmacen> Lista { get; private set; } = [];

    public bool Disponible => cola.Disponible;

    public int EnEspera => cola.EnEspera;

    public void OnGet() => Lista = aplicacion.ListaDePreparacion();

    // Para probar en clase que un pedido no se pierde si la aplicación se cae.
    public IActionResult OnPostCambiarDisponible()
    {
        cola.Disponible = !cola.Disponible;
        aplicacion.PasarPendientes();
        return RedirectToPage();
    }
}
