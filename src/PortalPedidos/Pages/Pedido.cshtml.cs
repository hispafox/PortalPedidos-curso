using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class PedidoModel(ServicioPedidos pedidos) : PageModel
{
    public Pedido Pedido { get; private set; } = null!;

    public IReadOnlyList<string> PasanDeLasExistencias { get; private set; } = [];

    public decimal FaltaParaElMinimo { get; private set; }

    [TempData]
    public string? Aviso { get; set; }

    public void OnGet() => Cargar();

    public IActionResult OnPostCambiar(string codigo, string cajas)
    {
        var resultado = int.TryParse(cajas, out var numero)
            ? pedidos.CambiarCajas(User.TiendaId(), codigo, numero)
            : Resultado.Mal("La cantidad tiene que ser un número entero mayor que cero.");
        if (!resultado.Hecho)
        {
            Aviso = resultado.Motivo;
        }

        return RedirectToPage();
    }

    public IActionResult OnPostQuitar(string codigo)
    {
        pedidos.Quitar(User.TiendaId(), codigo);
        return RedirectToPage();
    }

    public IActionResult OnPostConfirmar()
    {
        var resultado = pedidos.Confirmar(User.TiendaId(), DateOnly.FromDateTime(DateTime.Today));
        if (!resultado.Confirmado)
        {
            Aviso = resultado.Motivo;
            return RedirectToPage();
        }

        return RedirectToPage("/Confirmacion", new
        {
            numero = resultado.Numero,
            dia = resultado.DiaEntrega.ToString("yyyy-MM-dd"),
        });
    }

    private void Cargar()
    {
        Pedido = pedidos.PedidoEnCurso(User.TiendaId());
        PasanDeLasExistencias = pedidos.PasanDeLasExistencias(Pedido);
        FaltaParaElMinimo = pedidos.CuantoFaltaParaElMinimo(Pedido);
    }
}
