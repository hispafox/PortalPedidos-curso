using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

// HU-11, HU-12, HU-17: pedidos anteriores y repetir.
public class HistorialPruebas
{
    private readonly Entorno entorno = new();

    [Fact(DisplayName = "La tienda ve sus pedidos, el más reciente arriba")]
    public void LaTiendaVeSusPedidosElMasRecienteArriba()
    {
        var numeros = entorno.Pedidos.Historial(Entorno.Plaza).Select(p => p.Numero);

        Assert.Equal([1003, 1002, 1001], numeros);
    }

    [Fact(DisplayName = "Una tienda no ve los pedidos de otra")]
    public void UnaTiendaNoVeLosPedidosDeOtra()
    {
        Assert.DoesNotContain(entorno.Pedidos.Historial(Entorno.TiendaDePruebas), p => p.TiendaId == Entorno.Plaza);
        Assert.Null(entorno.Pedidos.DeLaTienda(Entorno.TiendaDePruebas, 1001));
    }

    [Fact(DisplayName = "Cada pedido del historial tiene su estado y su día de entrega")]
    public void CadaPedidoTieneEstadoYDiaDeEntrega()
    {
        Assert.All(entorno.Pedidos.Historial(Entorno.Plaza), p => Assert.NotNull(p.DiaEntrega));
        Assert.Equal(EstadoPedido.Preparado, entorno.Pedidos.DeLaTienda(Entorno.Plaza, 1002)!.Estado);
    }

    [Fact(DisplayName = "Repetir prepara un pedido nuevo con los mismos productos y cantidades")]
    public void RepetirPreparaUnPedidoNuevoIgual()
    {
        var resultado = entorno.Pedidos.Repetir(Entorno.Plaza, 1002, sustituirElEnCurso: false);

        Assert.True(resultado.Hecho);
        var nuevo = entorno.Pedidos.PedidoEnCurso(Entorno.Plaza);
        Assert.Equal([("AGUA", 12), ("ACEITE", 6), ("LEJIA", 2)], nuevo.LineasEnOrden.Select(l => (l.CodigoProducto, l.Cajas)));
    }

    [Fact(DisplayName = "Al repetir, lo retirado del catálogo se avisa y no entra en el pedido")]
    public void AlRepetirLoRetiradoSeAvisaYNoEntra()
    {
        var resultado = entorno.Pedidos.Repetir(Entorno.Plaza, 1001, sustituirElEnCurso: false);

        Assert.True(resultado.Hecho);
        Assert.Equal(["SUAVIZANTE"], resultado.Retirados.Select(p => p.Codigo));
        Assert.DoesNotContain(entorno.Pedidos.PedidoEnCurso(Entorno.Plaza).Lineas, l => l.CodigoProducto == "SUAVIZANTE");
    }

    [Fact(DisplayName = "Si ningún producto sigue en el catálogo, no prepara pedido")]
    public void SiNingunoSigueNoPreparaPedido()
    {
        foreach (var producto in entorno.Datos.Productos)
        {
            producto.EnCatalogo = false;
        }

        entorno.Datos.SaveChanges();

        var resultado = entorno.Pedidos.Repetir(Entorno.Plaza, 1002, sustituirElEnCurso: false);

        Assert.False(resultado.Hecho);
        Assert.True(entorno.Pedidos.PedidoEnCurso(Entorno.Plaza).EstaVacio);
    }

    [Fact(DisplayName = "Con un pedido en curso, primero pregunta si se sustituye")]
    public void ConUnPedidoEnCursoPreguntaAntes()
    {
        entorno.Pedidos.Anadir(Entorno.Plaza, "ARROZ", 1);

        var pregunta = entorno.Pedidos.Repetir(Entorno.Plaza, 1002, sustituirElEnCurso: false);
        Assert.True(pregunta.HayQuePreguntar);
        Assert.Equal("ARROZ", Assert.Single(entorno.Pedidos.PedidoEnCurso(Entorno.Plaza).Lineas).CodigoProducto);

        var hecho = entorno.Pedidos.Repetir(Entorno.Plaza, 1002, sustituirElEnCurso: true);
        Assert.True(hecho.Hecho);
        Assert.DoesNotContain(entorno.Pedidos.PedidoEnCurso(Entorno.Plaza).Lineas, l => l.CodigoProducto == "ARROZ");
    }
}
