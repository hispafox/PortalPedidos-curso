namespace PortalPedidos.Pruebas;

// HU-14, HU-15, HU-16: existencias.
public class ExistenciasPruebas
{
    private readonly Entorno entorno = new();

    [Fact(DisplayName = "Mientras el almacén no rellene las reservadas, se enseñan las totales")]
    public void SinReservadasSeEnsenanLasTotales()
    {
        var existencias = entorno.Catalogo.Existencias(entorno.Producto("ACEITE"));

        Assert.Equal(210, existencias.Cajas);
        Assert.False(existencias.SonDisponibles);
    }

    [Fact(DisplayName = "Con las reservadas rellenas en todos los productos, se enseñan las disponibles")]
    public void ConReservadasSeEnsenanLasDisponibles()
    {
        foreach (var producto in entorno.Datos.Productos)
        {
            producto.CajasReservadas = 0;
        }

        entorno.Producto("ACEITE").CajasReservadas = 60;
        entorno.Datos.SaveChanges();

        var existencias = entorno.Catalogo.Existencias(entorno.Producto("ACEITE"));

        Assert.Equal(150, existencias.Cajas);
        Assert.True(existencias.SonDisponibles);
    }

    [Fact(DisplayName = "Si solo algunos productos tienen reservadas, se siguen enseñando las totales a todos")]
    public void ConReservadasAMediasSeEnsenanLasTotales()
    {
        entorno.Producto("ACEITE").CajasReservadas = 60;
        entorno.Datos.SaveChanges();

        Assert.Equal(210, entorno.Catalogo.Existencias(entorno.Producto("ACEITE")).Cajas);
    }

    [Fact(DisplayName = "Un producto sin existencias sale agotado")]
    public void UnProductoSinExistenciasSaleAgotado()
    {
        Assert.True(entorno.Catalogo.Existencias(entorno.Producto("ZUMO")).Agotado);
    }

    [Fact(DisplayName = "Pedir más cajas de las que hay avisa, pero deja confirmar")]
    public void PedirMasDeLoQueHayAvisaPeroDejaConfirmar()
    {
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "LENTEJAS", 100);
        var pedido = entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas);

        Assert.Equal(["LENTEJAS"], entorno.Pedidos.PasanDeLasExistencias(pedido));
        Assert.True(entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes).Confirmado);
    }
}
