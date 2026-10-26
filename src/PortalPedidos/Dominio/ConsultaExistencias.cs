namespace PortalPedidos.Dominio;

public record Existencia(string CodigoProducto, int CajasTotales, int CajasReservadas);

// Lo que hay en el almacén, tal como lo pasa cada noche la aplicación del almacén.
public class ConsultaExistencias
{
    private readonly IReadOnlyList<Existencia> almacen;

    public ConsultaExistencias(IReadOnlyList<Existencia> almacen)
    {
        this.almacen = almacen;
    }

    public static IReadOnlyList<Existencia> AlmacenDePrueba { get; } =
    [
        new("AGUA", 480, 120),
        new("ACEITE", 210, 60),
        new("LEJIA", 150, 20),
        new("ARROZ", 300, 75),
        new("GALLETAS", 260, 40),
    ];

    public int CajasDe(string codigoProducto) =>
        almacen.FirstOrDefault(e => e.CodigoProducto == codigoProducto)?.CajasTotales ?? 0;
}
