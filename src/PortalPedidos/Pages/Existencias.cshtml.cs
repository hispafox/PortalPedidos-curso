using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class ExistenciasModel(ConsultaExistencias consulta) : PageModel
{
    public IReadOnlyList<Producto> Productos => Catalogo.Productos;

    public int CajasDe(Producto producto) => consulta.CajasDe(producto.Codigo);
}
