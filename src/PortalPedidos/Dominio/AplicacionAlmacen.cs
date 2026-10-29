using PortalPedidos.Datos;

namespace PortalPedidos.Dominio;

// Lo que la aplicación de pedidos del almacén tiene en su lista de preparación.
public class PedidoEnAlmacen
{
    public int Numero { get; set; }

    public string Tienda { get; set; } = "";

    public string Lineas { get; set; } = "";

    public DateOnly DiaReparto { get; set; }
}

// Los pedidos que esperan a que la aplicación del almacén esté disponible. Uno para toda la aplicación.
public class ColaAlmacen
{
    private readonly Queue<int> pendientes = new();

    public Lock Cerrojo { get; } = new();

    public bool Disponible { get; set; } = true;

    public int EnEspera
    {
        get
        {
            lock (Cerrojo)
            {
                return pendientes.Count;
            }
        }
    }

    public void Poner(int numero)
    {
        lock (Cerrojo)
        {
            pendientes.Enqueue(numero);
        }
    }

    public bool Sacar(out int numero)
    {
        lock (Cerrojo)
        {
            return pendientes.TryDequeue(out numero);
        }
    }
}

// Simula la aplicación de pedidos que ya usa el almacén. El portal le pasa los pedidos confirmados.
// Si no está disponible, los pedidos esperan y se pasan cuando vuelva; ninguno llega dos veces.
public class AplicacionAlmacen(PortalContexto datos, ColaAlmacen cola)
{
    public void Recibir(Pedido pedido)
    {
        cola.Poner(pedido.Numero!.Value);
        PasarPendientes();
    }

    public int PasarPendientes()
    {
        if (!cola.Disponible)
        {
            return 0;
        }

        var pasados = 0;
        while (cola.Sacar(out var numero))
        {
            if (datos.PedidosEnAlmacen.Any(p => p.Numero == numero))
            {
                continue;
            }

            var pedido = datos.Pedidos.Single(p => p.Numero == numero);
            datos.PedidosEnAlmacen.Add(new PedidoEnAlmacen
            {
                Numero = numero,
                Tienda = pedido.Tienda.Nombre,
                Lineas = string.Join("; ", pedido.LineasEnOrden.Select(l => $"{l.Producto.Nombre} x{l.Cajas}")),
                DiaReparto = pedido.DiaEntrega!.Value,
            });
            datos.SaveChanges();
            pasados++;
        }

        return pasados;
    }

    public IReadOnlyList<PedidoEnAlmacen> ListaDePreparacion() =>
        datos.PedidosEnAlmacen.OrderBy(p => p.Numero).ToList();
}
