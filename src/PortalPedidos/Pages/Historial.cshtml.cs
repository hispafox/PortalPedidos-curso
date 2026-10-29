using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class HistorialModel(ServicioPedidos pedidos, Catalogo catalogo) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? Numero { get; set; }

    public IReadOnlyList<Pedido> Pedidos { get; private set; } = [];

    public Pedido? Abierto { get; private set; }

    public int? PreguntarPorSustituir { get; private set; }

    public string? Aviso { get; private set; }

    public void OnGet() => Cargar();

    public IActionResult OnPostRepetir(int numero, bool sustituir)
    {
        var resultado = pedidos.Repetir(User.TiendaId(), numero, sustituir);
        if (resultado.HayQuePreguntar)
        {
            PreguntarPorSustituir = numero;
            Aviso = resultado.Motivo;
            Cargar();
            return Page();
        }

        if (!resultado.Hecho)
        {
            Aviso = resultado.Motivo;
            Cargar();
            return Page();
        }

        if (resultado.Retirados.Count > 0)
        {
            var avisos = resultado.Retirados.Select(p =>
            {
                var parecido = catalogo.Parecido(p);
                return parecido is null
                    ? $"{p.Nombre} ya no está en el catálogo y no se ha añadido."
                    : $"{p.Nombre} ya no está en el catálogo y no se ha añadido. Uno parecido: {parecido.Nombre}.";
            });
            TempData["Aviso"] = string.Join(" ", avisos);
        }

        return RedirectToPage("/Pedido");
    }

    private void Cargar()
    {
        Pedidos = pedidos.Historial(User.TiendaId());
        Abierto = Numero is int n ? pedidos.DeLaTienda(User.TiendaId(), n) : null;
    }

    public static string NombreDe(EstadoPedido estado) => estado switch
    {
        EstadoPedido.Confirmado => "Confirmado",
        EstadoPedido.Preparado => "Preparado",
        EstadoPedido.Entregado => "Entregado",
        _ => "En curso",
    };
}
