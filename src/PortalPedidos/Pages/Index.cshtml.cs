using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class IndexModel(Catalogo catalogo, ServicioPedidos pedidos) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? Familia { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Buscar { get; set; }

    public IReadOnlyList<Familia> Familias { get; private set; } = [];

    public IReadOnlyList<Producto> Productos { get; private set; } = [];

    public int CajasEnPedido { get; private set; }

    public string? Aviso { get; private set; }

    public ExistenciasDe Existencias(Producto producto) => catalogo.Existencias(producto);

    public void OnGet() => Cargar();

    public IActionResult OnPostAnadir(string codigo, string cajas)
    {
        if (!int.TryParse(cajas, out var numero))
        {
            Aviso = "La cantidad tiene que ser un número entero mayor que cero.";
            Cargar();
            return Page();
        }

        var resultado = pedidos.Anadir(User.TiendaId(), codigo, numero);
        if (!resultado.Hecho)
        {
            Aviso = resultado.Motivo;
            Cargar();
            return Page();
        }

        return RedirectToPage(new { Familia, Buscar });
    }

    private void Cargar()
    {
        Familias = catalogo.Familias();
        if (!string.IsNullOrWhiteSpace(Buscar))
        {
            Productos = catalogo.Buscar(Buscar);
            if (Productos.Count == 0)
            {
                Aviso ??= $"No hay ningún producto que contenga «{Buscar}».";
            }
        }
        else
        {
            Familia ??= Familias.FirstOrDefault()?.Id;
            Productos = Familia is int id ? catalogo.ProductosDe(id) : [];
        }

        CajasEnPedido = pedidos.PedidoEnCurso(User.TiendaId()).TotalCajas;
    }
}
