using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

// HU-06, HU-09, HU-10: el almacén recibe los pedidos.
public class AlmacenPruebas
{
    private readonly Entorno entorno = new();

    private int ConfirmarUno()
    {
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 1);
        return entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes).Numero;
    }

    [Fact(DisplayName = "La lista enseña los pedidos sin preparar, el más antiguo arriba")]
    public void LaListaEnsenaLosNuevosElMasAntiguoArriba()
    {
        var numero = ConfirmarUno();

        Assert.Equal([1003, 1004, numero], entorno.Pedidos.PedidosNuevos().Select(p => p.Numero!.Value));
    }

    [Fact(DisplayName = "Al marcarlo preparado sale de la lista y la tienda lo ve preparado")]
    public void AlMarcarloPreparadoSaleDeLaLista()
    {
        var resultado = entorno.Pedidos.MarcarPreparado(1003);

        Assert.True(resultado.Hecho);
        Assert.DoesNotContain(entorno.Pedidos.PedidosNuevos(), p => p.Numero == 1003);
        Assert.Equal(EstadoPedido.Preparado, entorno.Pedidos.DeLaTienda(Entorno.Plaza, 1003)!.Estado);
    }

    [Fact(DisplayName = "Un pedido preparado no se puede volver a marcar")]
    public void UnPreparadoNoSeVuelveAMarcar()
    {
        Assert.False(entorno.Pedidos.MarcarPreparado(1002).Hecho);
    }

    [Fact(DisplayName = "El pedido confirmado llega a la lista de preparación del almacén")]
    public void ElConfirmadoLlegaALaAplicacionDelAlmacen()
    {
        var numero = ConfirmarUno();

        var llegado = Assert.Single(entorno.Almacen.ListaDePreparacion(), p => p.Numero == numero);
        Assert.Equal("Tienda de pruebas", llegado.Tienda);
        Assert.Equal("Agua mineral x1", llegado.Lineas);
    }

    [Fact(DisplayName = "Si la aplicación del almacén está caída, el pedido espera y llega cuando vuelve")]
    public void SiLaAplicacionEstaCaidaElPedidoEspera()
    {
        entorno.Cola.Disponible = false;
        var numero = ConfirmarUno();
        Assert.DoesNotContain(entorno.Almacen.ListaDePreparacion(), p => p.Numero == numero);
        Assert.Equal(1, entorno.Cola.EnEspera);

        entorno.Cola.Disponible = true;
        entorno.Almacen.PasarPendientes();

        Assert.Contains(entorno.Almacen.ListaDePreparacion(), p => p.Numero == numero);
    }

    [Fact(DisplayName = "Un pedido no llega dos veces a la aplicación del almacén")]
    public void UnPedidoNoLlegaDosVeces()
    {
        var numero = ConfirmarUno();
        entorno.Cola.Poner(numero);
        entorno.Almacen.PasarPendientes();

        Assert.Single(entorno.Almacen.ListaDePreparacion(), p => p.Numero == numero);
    }
}
