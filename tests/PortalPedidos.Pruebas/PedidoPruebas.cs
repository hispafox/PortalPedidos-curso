using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

public class PedidoPruebas
{
    private static readonly DateOnly Lunes = new(2026, 11, 2);

    private static Producto Agua => Catalogo.Buscar("AGUA")!;

    private static Producto Aceite => Catalogo.Buscar("ACEITE")!;

    [Fact(DisplayName = "Pedido de varias cajas se confirma")]
    public void PedidoDeVariasCajasSeConfirma()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua, 12);
        pedido.Anadir(Aceite, 6);

        var resultado = new RegistroPedidos().Confirmar(pedido, Lunes);

        Assert.True(resultado.Confirmado);
    }

    [Fact(DisplayName = "Pedido de una sola caja se confirma")]
    public void PedidoDeUnaSolaCajaSeConfirma()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua);

        var resultado = new RegistroPedidos().Confirmar(pedido, Lunes);

        Assert.True(resultado.Confirmado);
    }

    [Fact(DisplayName = "Pedido sin productos se rechaza")]
    public void PedidoSinProductosSeRechaza()
    {
        var resultado = new RegistroPedidos().Confirmar(new Pedido(), Lunes);

        Assert.False(resultado.Confirmado);
        Assert.Equal("El pedido está vacío.", resultado.Motivo);
    }

    [Fact(DisplayName = "Cada pedido confirmado recibe un número nuevo")]
    public void CadaPedidoConfirmadoRecibeUnNumeroNuevo()
    {
        var registro = new RegistroPedidos();
        var pedido = new Pedido();
        pedido.Anadir(Agua);

        var primero = registro.Confirmar(pedido, Lunes);
        var segundo = registro.Confirmar(pedido, Lunes);

        Assert.NotEqual(primero.Numero, segundo.Numero);
    }

    [Fact(DisplayName = "El importe suma todas las líneas")]
    public void ElImporteSumaTodasLasLineas()
    {
        var pedido = new Pedido();
        pedido.Anadir(Agua, 2);
        pedido.Anadir(Aceite);

        Assert.Equal(Agua.PrecioCaja * 2 + Aceite.PrecioCaja, pedido.Importe);
    }

    [Fact(DisplayName = "No se puede pedir cero cajas")]
    public void NoSePuedePedirCeroCajas()
    {
        var pedido = new Pedido();

        Assert.Throws<ArgumentOutOfRangeException>(() => pedido.Anadir(Agua, 0));
    }
}
