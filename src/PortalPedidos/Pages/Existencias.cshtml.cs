using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Datos;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class ExistenciasModel(Catalogo catalogo, PortalContexto datos) : PageModel
{
    public IReadOnlyList<Producto> Productos =>
        datos.Productos.AsEnumerable().Where(p => p.SeVende).OrderBy(p => p.Nombre).ToList();

    public ExistenciasDe Existencias(Producto producto) => catalogo.Existencias(producto);
}
