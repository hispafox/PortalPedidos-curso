using Microsoft.EntityFrameworkCore;
using PortalPedidos.Datos;
using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

// Un portal recién arrancado, con los datos de prueba, en una base en memoria propia para cada prueba.
public class Entorno
{
    public const int Plaza = 1;

    public const int TiendaDePruebas = 2;

    public static readonly DateOnly Lunes = new(2026, 11, 2);

    public Entorno(decimal pedidoMinimo = 0)
    {
        var opciones = new DbContextOptionsBuilder<PortalContexto>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Datos = new PortalContexto(opciones);
        DatosDePrueba.Sembrar(Datos);
        Cola = new ColaAlmacen();
        Almacen = new AplicacionAlmacen(Datos, Cola);
        Catalogo = new Catalogo(Datos);
        Pedidos = new ServicioPedidos(Datos, Catalogo, Almacen, new OpcionesPortal { PedidoMinimo = pedidoMinimo });
    }

    public PortalContexto Datos { get; }

    public ColaAlmacen Cola { get; }

    public AplicacionAlmacen Almacen { get; }

    public Catalogo Catalogo { get; }

    public ServicioPedidos Pedidos { get; }

    public Producto Producto(string codigo) => Datos.Productos.Find(codigo)!;
}
