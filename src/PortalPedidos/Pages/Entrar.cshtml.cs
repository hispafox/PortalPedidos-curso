using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PortalPedidos.Datos;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pages;

public class EntrarModel(Acceso acceso, PortalContexto datos) : PageModel
{
    [BindProperty]
    public string Usuario { get; set; } = "";

    [BindProperty]
    public string Contrasena { get; set; } = "";

    public string? Aviso { get; private set; }

    public async Task<IActionResult> OnPostAsync()
    {
        var usuario = acceso.Comprobar(Usuario, Contrasena);
        if (usuario is null)
        {
            // No se dice cuál de los dos falla.
            Aviso = "El usuario o la contraseña no son correctos.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Role, usuario.Rol.ToString()),
        };
        if (usuario.TiendaId is int tiendaId)
        {
            claims.Add(new Claim(UsuarioActual.ClaimTienda, tiendaId.ToString()));
            claims.Add(new Claim(ClaimTypes.GivenName, datos.Tiendas.Find(tiendaId)!.Nombre));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.GivenName, "Almacén"));
        }

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
        return usuario.Rol == Rol.Almacen ? RedirectToPage("/Almacen/Index") : RedirectToPage("/Index");
    }
}
