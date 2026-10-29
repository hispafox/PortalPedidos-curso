using PortalPedidos.Datos;

namespace PortalPedidos.Dominio;

// Lo que se puede cambiar sin tocar el código, en appsettings.json, sección «Portal».
public class OpcionesPortal
{
    public decimal PedidoMinimo { get; set; }
}

public record Resultado(bool Hecho, string Motivo = "")
{
    public static Resultado Bien { get; } = new(true);

    public static Resultado Mal(string motivo) => new(false, motivo);
}

public record ResultadoConfirmacion(bool Confirmado, int Numero, DateOnly DiaEntrega, string Motivo)
{
    public static ResultadoConfirmacion Rechazado(string motivo) => new(false, 0, default, motivo);
}

public record ResultadoRepeticion(bool Hecho, bool HayQuePreguntar, IReadOnlyList<Producto> Retirados, string Motivo);

public class ServicioPedidos(PortalContexto datos, Catalogo catalogo, AplicacionAlmacen almacen, OpcionesPortal opciones)
{
    public const int PrimerNumero = 1001;

    // Una tienda tiene como mucho un pedido en curso. Se guarda aunque cierre la sesión.
    public Pedido PedidoEnCurso(int tiendaId)
    {
        var pedido = datos.Pedidos.FirstOrDefault(p => p.TiendaId == tiendaId && p.Estado == EstadoPedido.EnCurso);
        if (pedido is null)
        {
            pedido = new Pedido { TiendaId = tiendaId, Tienda = datos.Tiendas.Find(tiendaId)! };
            datos.Pedidos.Add(pedido);
            datos.SaveChanges();
        }

        return pedido;
    }

    public Resultado Anadir(int tiendaId, string codigo, int cajas)
    {
        var producto = catalogo.Producto(codigo);
        if (producto is null || !producto.SeVende)
        {
            return Resultado.Mal("Ese producto no está en el catálogo.");
        }

        if (cajas < 1)
        {
            return Resultado.Mal("La cantidad tiene que ser un número entero mayor que cero.");
        }

        PedidoEnCurso(tiendaId).Anadir(producto, cajas);
        datos.SaveChanges();
        return Resultado.Bien;
    }

    public Resultado CambiarCajas(int tiendaId, string codigo, int cajas)
    {
        if (cajas < 1)
        {
            return Resultado.Mal("La cantidad tiene que ser un número entero mayor que cero.");
        }

        var pedido = PedidoEnCurso(tiendaId);
        if (pedido.Lineas.All(l => l.CodigoProducto != codigo))
        {
            return Resultado.Mal("Ese producto no está en el pedido.");
        }

        pedido.CambiarCajas(codigo, cajas);
        datos.SaveChanges();
        return Resultado.Bien;
    }

    public void Quitar(int tiendaId, string codigo)
    {
        PedidoEnCurso(tiendaId).Quitar(codigo);
        datos.SaveChanges();
    }

    // Las líneas que piden más cajas de las que hay. El aviso no impide confirmar.
    public IReadOnlyList<string> PasanDeLasExistencias(Pedido pedido) =>
        pedido.Lineas.Where(l => l.Cajas > catalogo.Existencias(l.Producto).Cajas)
            .Select(l => l.CodigoProducto).ToList();

    public decimal CuantoFaltaParaElMinimo(Pedido pedido) =>
        Math.Max(0, opciones.PedidoMinimo - pedido.Importe);

    public ResultadoConfirmacion Confirmar(int tiendaId, DateOnly hoy)
    {
        var pedido = PedidoEnCurso(tiendaId);
        if (pedido.EstaVacio)
        {
            return ResultadoConfirmacion.Rechazado("El pedido está vacío.");
        }

        var falta = CuantoFaltaParaElMinimo(pedido);
        if (falta > 0)
        {
            return ResultadoConfirmacion.Rechazado($"El pedido no llega al mínimo. Faltan {falta:C}.");
        }

        pedido.Numero = SiguienteNumero();
        pedido.Estado = EstadoPedido.Confirmado;
        pedido.FechaConfirmacion = hoy;
        pedido.DiaEntrega = CalendarioReparto.DiaDeEntrega(hoy);
        try
        {
            datos.SaveChanges();
        }
        catch (Exception)
        {
            pedido.Numero = null;
            pedido.Estado = EstadoPedido.EnCurso;
            pedido.FechaConfirmacion = null;
            pedido.DiaEntrega = null;
            return ResultadoConfirmacion.Rechazado("El pedido no se ha podido guardar. Sigue en curso: inténtalo otra vez.");
        }

        almacen.Recibir(pedido);
        return new ResultadoConfirmacion(true, pedido.Numero.Value, pedido.DiaEntrega.Value, "");
    }

    public IReadOnlyList<Pedido> Historial(int tiendaId) =>
        datos.Pedidos.Where(p => p.TiendaId == tiendaId && p.Estado != EstadoPedido.EnCurso)
            .OrderByDescending(p => p.Numero).ToList();

    public Pedido? DeLaTienda(int tiendaId, int numero) =>
        datos.Pedidos.FirstOrDefault(p => p.TiendaId == tiendaId && p.Numero == numero);

    public ResultadoRepeticion Repetir(int tiendaId, int numero, bool sustituirElEnCurso)
    {
        var anterior = DeLaTienda(tiendaId, numero);
        if (anterior is null)
        {
            return new ResultadoRepeticion(false, false, [], "Ese pedido no es de tu tienda.");
        }

        var retirados = anterior.LineasEnOrden.Where(l => !l.Producto.SeVende).Select(l => l.Producto).ToList();
        var quedan = anterior.LineasEnOrden.Where(l => l.Producto.SeVende).ToList();
        if (quedan.Count == 0)
        {
            return new ResultadoRepeticion(false, false, retirados, "Ninguno de los productos de ese pedido sigue en el catálogo.");
        }

        var enCurso = PedidoEnCurso(tiendaId);
        if (!enCurso.EstaVacio && !sustituirElEnCurso)
        {
            return new ResultadoRepeticion(false, true, retirados, "Ya tienes un pedido en curso. ¿Lo sustituyes por este?");
        }

        enCurso.Lineas.Clear();
        foreach (var linea in quedan)
        {
            enCurso.Anadir(linea.Producto, linea.Cajas);
        }

        datos.SaveChanges();
        return new ResultadoRepeticion(true, false, retirados, "");
    }

    // Para el almacén: los confirmados que no se han preparado, el más antiguo arriba.
    public IReadOnlyList<Pedido> PedidosNuevos() =>
        datos.Pedidos.Where(p => p.Estado == EstadoPedido.Confirmado).OrderBy(p => p.Numero).ToList();

    public Pedido? PorNumero(int numero) => datos.Pedidos.FirstOrDefault(p => p.Numero == numero);

    public Resultado MarcarPreparado(int numero)
    {
        var pedido = PorNumero(numero);
        if (pedido is null)
        {
            return Resultado.Mal("No hay ningún pedido con ese número.");
        }

        if (pedido.Estado != EstadoPedido.Confirmado)
        {
            return Resultado.Mal("Ese pedido ya estaba preparado.");
        }

        pedido.Estado = EstadoPedido.Preparado;
        datos.SaveChanges();
        return Resultado.Bien;
    }

    private int SiguienteNumero()
    {
        var ultimo = datos.Pedidos.Where(p => p.Numero != null).Max(p => (int?)p.Numero);
        return ultimo is null ? PrimerNumero : ultimo.Value + 1;
    }
}
