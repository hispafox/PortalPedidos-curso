namespace PortalPedidos.Dominio;

public record ResultadoConfirmacion(bool Confirmado, int Numero, DateOnly DiaEntrega, string Motivo)
{
    public static ResultadoConfirmacion Rechazado(string motivo) => new(false, 0, default, motivo);
}

// Guarda en memoria los pedidos confirmados y les da número.
public class RegistroPedidos
{
    private readonly Lock cerrojo = new();
    private int ultimoNumero = 1000;

    public ResultadoConfirmacion Confirmar(Pedido pedido, DateOnly hoy)
    {
        if (pedido.EstaVacio)
        {
            return ResultadoConfirmacion.Rechazado("El pedido está vacío.");
        }

        lock (cerrojo)
        {
            ultimoNumero++;
            return new ResultadoConfirmacion(true, ultimoNumero, CalendarioReparto.DiaDeEntrega(hoy), "");
        }
    }
}
