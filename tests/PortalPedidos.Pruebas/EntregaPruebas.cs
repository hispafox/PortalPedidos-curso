using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

public class EntregaPruebas
{
    [Theory(DisplayName = "El pedido llega el siguiente día de reparto")]
    [InlineData("2026-11-02", "2026-11-03")] // lunes → martes
    [InlineData("2026-11-06", "2026-11-09")] // viernes → lunes
    [InlineData("2026-11-07", "2026-11-09")] // sábado → lunes
    public void ElPedidoLlegaElSiguienteDiaDeReparto(string pedido, string entrega)
    {
        Assert.Equal(DateOnly.Parse(entrega), CalendarioReparto.DiaDeEntrega(DateOnly.Parse(pedido)));
    }

    [Fact(DisplayName = "La confirmación pone el número de pedido")]
    public void LaConfirmacionPoneElNumeroDePedido()
    {
        var texto = PlantillaConfirmacion.Rellenar("Pedido {numero} recibido.", 1001, new DateOnly(2026, 11, 3));

        Assert.Equal("Pedido 1001 recibido.", texto);
    }

    [Fact(DisplayName = "La confirmación pone el día de entrega")]
    public void LaConfirmacionPoneElDiaDeEntrega()
    {
        var texto = PlantillaConfirmacion.Rellenar("Llega el {dia_entrega}.", 1001, new DateOnly(2026, 11, 3));

        Assert.Equal("Llega el martes 3 de noviembre.", texto);
    }
}
