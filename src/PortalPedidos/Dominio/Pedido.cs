namespace PortalPedidos.Dominio;

public enum EstadoPedido
{
    EnCurso,
    Confirmado,
    Preparado,
    Entregado,
}

public class LineaPedido
{
    public int Id { get; set; }

    public string CodigoProducto { get; set; } = "";

    public Producto Producto { get; set; } = null!;

    public int Cajas { get; set; }

    // Las líneas salen en el orden en que la tienda las añadió.
    public int Orden { get; set; }

    public decimal Importe => (Producto.PrecioCaja ?? 0) * Cajas;
}

public class Pedido
{
    public int Id { get; set; }

    public int TiendaId { get; set; }

    public Tienda Tienda { get; set; } = null!;

    // Lo pone el portal al confirmar.
    public int? Numero { get; set; }

    public EstadoPedido Estado { get; set; } = EstadoPedido.EnCurso;

    public DateOnly? FechaConfirmacion { get; set; }

    public DateOnly? DiaEntrega { get; set; }

    public List<LineaPedido> Lineas { get; set; } = [];

    public IEnumerable<LineaPedido> LineasEnOrden => Lineas.OrderBy(l => l.Orden);

    public bool EstaVacio => Lineas.Count == 0;

    // El total del pedido son las cajas de todas sus líneas.
    public int TotalCajas => Lineas.Sum(l => l.Cajas);

    public decimal Importe => Lineas.Sum(l => l.Importe);

    public void Anadir(Producto producto, int cajas)
    {
        ComprobarCajas(cajas);

        var linea = Lineas.FirstOrDefault(l => l.CodigoProducto == producto.Codigo);
        if (linea is not null)
        {
            linea.Cajas += cajas;
            return;
        }

        var orden = Lineas.Count == 0 ? 1 : Lineas.Max(l => l.Orden) + 1;
        Lineas.Add(new LineaPedido { CodigoProducto = producto.Codigo, Producto = producto, Cajas = cajas, Orden = orden });
    }

    public void CambiarCajas(string codigoProducto, int cajas)
    {
        ComprobarCajas(cajas);
        var linea = Lineas.FirstOrDefault(l => l.CodigoProducto == codigoProducto)
            ?? throw new InvalidOperationException("Ese producto no está en el pedido.");
        linea.Cajas = cajas;
    }

    public void Quitar(string codigoProducto) =>
        Lineas.RemoveAll(l => l.CodigoProducto == codigoProducto);

    private static void ComprobarCajas(int cajas)
    {
        if (cajas < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(cajas), "La cantidad tiene que ser un número entero mayor que cero.");
        }
    }
}
