using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

// HU-03, HU-04, HU-07: armar el pedido.
public class PedidoPruebas
{
    private readonly Entorno entorno = new();

    private Producto Agua => entorno.Producto("AGUA");

    private Producto Aceite => entorno.Producto("ACEITE");

    [Fact(DisplayName = "El producto añadido aparece con su cantidad")]
    public void ElProductoAnadidoApareceConSuCantidad()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua, 12);

        var linea = Assert.Single(pedido.Lineas);
        Assert.Equal(12, linea.Cajas);
    }

    [Theory(DisplayName = "Una cantidad que no es un entero mayor que cero no añade nada")]
    [InlineData("0")]
    [InlineData("-3")]
    [InlineData("dos")]
    [InlineData("1,5")]
    public void UnaCantidadQueNoValeNoAnadeNada(string cajas)
    {
        var resultado = int.TryParse(cajas, out var numero)
            ? entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", numero)
            : Resultado.Mal("no es un número");

        Assert.False(resultado.Hecho);
        Assert.True(entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas).EstaVacio);
    }

    [Fact(DisplayName = "Si el producto ya estaba, se suma y no aparece dos veces")]
    public void SiElProductoYaEstabaSeSuma()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua, 6);
        pedido.Anadir(Agua, 6);

        var linea = Assert.Single(pedido.Lineas);
        Assert.Equal(12, linea.Cajas);
    }

    [Fact(DisplayName = "El pedido sigue ahí, sin enviar, al volver a entrar")]
    public void ElPedidoSigueAhiAlVolverAEntrar()
    {
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 2);

        // Otra petición, otro momento: se vuelve a pedir el pedido en curso de la tienda.
        var luego = entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas);

        Assert.Equal(EstadoPedido.EnCurso, luego.Estado);
        Assert.Equal(2, luego.TotalCajas);
    }

    [Fact(DisplayName = "El total suma las cajas de todas las líneas")]
    public void ElTotalSumaLasCajasDeTodasLasLineas()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua, 12);
        pedido.Anadir(Aceite, 6);

        Assert.Equal(18, pedido.TotalCajas);
    }

    [Fact(DisplayName = "Las líneas salen en el orden en que se añadieron")]
    public void LasLineasSalenEnElOrdenEnQueSeAnadieron()
    {
        var pedido = new Pedido();
        pedido.Anadir(Aceite, 1);
        pedido.Anadir(Agua, 1);

        Assert.Equal(["ACEITE", "AGUA"], pedido.LineasEnOrden.Select(l => l.CodigoProducto));
    }

    [Fact(DisplayName = "El importe suma precio por cajas de todas las líneas")]
    public void ElImporteSumaTodasLasLineas()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua, 2);
        pedido.Anadir(Aceite, 1);

        Assert.Equal(Agua.PrecioCaja!.Value * 2 + Aceite.PrecioCaja!.Value, pedido.Importe);
    }

    [Fact(DisplayName = "Se cambia la cantidad de una línea")]
    public void SeCambiaLaCantidadDeUnaLinea()
    {
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 2);

        var resultado = entorno.Pedidos.CambiarCajas(Entorno.TiendaDePruebas, "AGUA", 5);

        Assert.True(resultado.Hecho);
        Assert.Equal(5, entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas).TotalCajas);
    }

    [Fact(DisplayName = "Una cantidad nueva que no vale deja la que había")]
    public void UnaCantidadNuevaQueNoValeDejaLaQueHabia()
    {
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 2);

        var resultado = entorno.Pedidos.CambiarCajas(Entorno.TiendaDePruebas, "AGUA", 0);

        Assert.False(resultado.Hecho);
        Assert.Equal(2, entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas).TotalCajas);
    }

    [Fact(DisplayName = "Quitar una línea la saca del pedido")]
    public void QuitarUnaLineaLaSacaDelPedido()
    {
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 2);
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "ACEITE", 1);

        entorno.Pedidos.Quitar(Entorno.TiendaDePruebas, "AGUA");

        var linea = Assert.Single(entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas).Lineas);
        Assert.Equal("ACEITE", linea.CodigoProducto);
    }

    [Fact(DisplayName = "No se puede añadir un producto que no está en el catálogo")]
    public void NoSePuedeAnadirUnProductoRetirado()
    {
        var resultado = entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "SUAVIZANTE", 1);

        Assert.False(resultado.Hecho);
    }
}
