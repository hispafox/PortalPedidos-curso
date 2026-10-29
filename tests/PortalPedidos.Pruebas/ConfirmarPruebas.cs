using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

// HU-05, HU-13: confirmar el pedido.
public class ConfirmarPruebas
{
    [Fact(DisplayName = "Al confirmar, la tienda recibe un número de pedido")]
    public void AlConfirmarRecibeUnNumero()
    {
        var entorno = new Entorno();
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 12);

        var resultado = entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);

        Assert.True(resultado.Confirmado);
        Assert.Equal(1005, resultado.Numero); // los datos de prueba ya traen del 1001 al 1004
    }

    [Fact(DisplayName = "Cada pedido confirmado recibe un número nuevo")]
    public void CadaPedidoRecibeUnNumeroNuevo()
    {
        var entorno = new Entorno();
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 1);
        var primero = entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 1);
        var segundo = entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);

        Assert.NotEqual(primero.Numero, segundo.Numero);
    }

    [Fact(DisplayName = "El portal no deja confirmar un pedido vacío")]
    public void NoDejaConfirmarUnPedidoVacio()
    {
        var resultado = new Entorno().Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);

        Assert.False(resultado.Confirmado);
        Assert.Equal("El pedido está vacío.", resultado.Motivo);
    }

    [Fact(DisplayName = "La confirmación trae el día de entrega")]
    public void LaConfirmacionTraeElDiaDeEntrega()
    {
        var entorno = new Entorno();
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 1);

        var resultado = entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);

        Assert.Equal(new DateOnly(2026, 11, 3), resultado.DiaEntrega);
    }

    [Fact(DisplayName = "Tras confirmar, la tienda empieza un pedido nuevo vacío")]
    public void TrasConfirmarEmpiezaUnPedidoNuevo()
    {
        var entorno = new Entorno();
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 1);
        entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);

        Assert.True(entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas).EstaVacio);
    }

    [Fact(DisplayName = "Por debajo del mínimo no deja confirmar y dice cuánto falta")]
    public void PorDebajoDelMinimoNoDejaConfirmar()
    {
        var entorno = new Entorno(pedidoMinimo: 30);
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "AGUA", 2); // 7,80 €

        var resultado = entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes);

        Assert.False(resultado.Confirmado);
        Assert.Equal(22.20m, entorno.Pedidos.CuantoFaltaParaElMinimo(entorno.Pedidos.PedidoEnCurso(Entorno.TiendaDePruebas)));
    }

    [Fact(DisplayName = "Un pedido que llega justo al mínimo se confirma")]
    public void JustoAlMinimoSeConfirma()
    {
        var entorno = new Entorno(pedidoMinimo: 10.80m);
        entorno.Pedidos.Anadir(Entorno.TiendaDePruebas, "LEJIA", 1); // 10,80 €

        Assert.True(entorno.Pedidos.Confirmar(Entorno.TiendaDePruebas, Entorno.Lunes).Confirmado);
    }
}
