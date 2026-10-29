using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

// HU-01: entrar en el portal (simulado, sin Identity).
public class AccesoPruebas
{
    private readonly Acceso acceso = new(new Entorno().Datos);

    [Fact(DisplayName = "Con su usuario y su contraseña, la tienda entra")]
    public void ConSuUsuarioYContrasenaEntra()
    {
        var usuario = acceso.Comprobar("plaza", "plaza");

        Assert.NotNull(usuario);
        Assert.Equal(Rol.Tienda, usuario.Rol);
        Assert.Equal(Entorno.Plaza, usuario.TiendaId);
    }

    [Theory(DisplayName = "Con el usuario o la contraseña mal, no entra")]
    [InlineData("plaza", "otra")]
    [InlineData("nadie", "plaza")]
    public void ConDatosMalNoEntra(string nombre, string contrasena)
    {
        Assert.Null(acceso.Comprobar(nombre, contrasena));
    }

    [Fact(DisplayName = "La contraseña no se guarda en claro")]
    public void LaContrasenaNoSeGuardaEnClaro()
    {
        var entorno = new Entorno();

        Assert.All(entorno.Datos.Usuarios, u => Assert.NotEqual(u.Nombre, u.HuellaContrasena));
    }

    [Fact(DisplayName = "El usuario del almacén entra con su papel")]
    public void ElAlmacenEntraConSuPapel()
    {
        Assert.Equal(Rol.Almacen, acceso.Comprobar("almacen", "almacen")!.Rol);
    }
}
