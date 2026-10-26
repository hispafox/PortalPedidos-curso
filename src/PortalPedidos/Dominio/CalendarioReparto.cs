namespace PortalPedidos.Dominio;

public static class CalendarioReparto
{
    // El camión sale de lunes a viernes: un pedido llega el siguiente día de reparto.
    public static DateOnly DiaDeEntrega(DateOnly diaDelPedido)
    {
        var dia = diaDelPedido.AddDays(1);
        while (dia.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            dia = dia.AddDays(1);
        }

        return dia;
    }
}
