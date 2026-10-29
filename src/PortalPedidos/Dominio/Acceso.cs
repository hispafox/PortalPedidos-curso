using System.Security.Cryptography;
using System.Text;
using PortalPedidos.Datos;

namespace PortalPedidos.Dominio;

// Entrada simulada: comprueba usuario y contraseña contra los usuarios de prueba.
// Sin ASP.NET Core Identity; la sesión la lleva una cookie (Program.cs).
public class Acceso(PortalContexto datos)
{
    public Usuario? Comprobar(string nombre, string contrasena)
    {
        var usuario = datos.Usuarios.Find(nombre.Trim());
        return usuario is not null && usuario.HuellaContrasena == Huella(contrasena) ? usuario : null;
    }

    public static string Huella(string contrasena) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(contrasena)));
}
