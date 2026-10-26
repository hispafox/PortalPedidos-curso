namespace PortalPedidos.Dominio;

public record LineaPedido(Producto Producto, int Cajas)
{
    public decimal Importe => Producto.PrecioCaja * Cajas;
}

public class Pedido
{
    private readonly List<LineaPedido> lineas = [];

    public IReadOnlyList<LineaPedido> Lineas => lineas;

    public bool EstaVacio => lineas.Count == 0;

    public decimal Importe => lineas.Sum(l => l.Importe);

    public void Anadir(Producto producto, int cajas = 1)
    {
        if (cajas < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(cajas), "Hay que pedir al menos una caja.");
        }

        lineas.Add(new LineaPedido(producto, cajas));
    }

    public void Vaciar() => lineas.Clear();
}
