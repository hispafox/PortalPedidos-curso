using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class PedidoModel(Pedido pedido, RegistroPedidos registro) : PageModel
{
    public Pedido Pedido => pedido;

    public string? Aviso { get; private set; }

    public IActionResult OnPost()
    {
        var resultado = registro.Confirmar(pedido, DateOnly.FromDateTime(DateTime.Today));
        if (!resultado.Confirmado)
        {
            Aviso = resultado.Motivo;
            return Page();
        }

        pedido.Vaciar();
        return RedirectToPage("/Confirmacion", new
        {
            numero = resultado.Numero,
            dia = resultado.DiaEntrega.ToString("yyyy-MM-dd"),
        });
    }
}
