using System.Security.Claims;

namespace PortalPedidos.Pages;

public static class UsuarioActual
{
    public const string ClaimTienda = "tienda";

    public static int TiendaId(this ClaimsPrincipal usuario) =>
        int.Parse(usuario.FindFirstValue(ClaimTienda) ?? throw new InvalidOperationException("Este usuario no es de una tienda."));

    public static string NombreTienda(this ClaimsPrincipal usuario) =>
        usuario.FindFirstValue(ClaimTypes.GivenName) ?? usuario.Identity?.Name ?? "";
}
