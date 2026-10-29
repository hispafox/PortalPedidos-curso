using PortalPedidos.Dominio;

namespace PortalPedidos.Datos;

// Lo que hay al arrancar el portal. Como todo vive en memoria, al reiniciar se vuelve a esto.
public static class DatosDePrueba
{
    public static void Sembrar(PortalContexto datos)
    {
        if (datos.Productos.Any())
        {
            return;
        }

        var bebidas = new Familia { Id = 1, Nombre = "Bebidas" };
        var alimentacion = new Familia { Id = 2, Nombre = "Alimentación" };
        var drogueria = new Familia { Id = 3, Nombre = "Droguería" };
        datos.Familias.AddRange(bebidas, alimentacion, drogueria);

        Producto Nuevo(string codigo, string nombre, string formato, decimal? precio, Familia? familia, int cajas, bool enCatalogo = true) =>
            new() { Codigo = codigo, Nombre = nombre, Formato = formato, PrecioCaja = precio, FamiliaId = familia?.Id, Familia = familia, CajasTotales = cajas, EnCatalogo = enCatalogo };

        var agua = Nuevo("AGUA", "Agua mineral", "Caja de 6 botellas de 1,5 l", 3.90m, bebidas, 480);
        var aceite = Nuevo("ACEITE", "Aceite de girasol", "Caja de 6 botellas de 1 l", 10.80m, alimentacion, 210);
        var lejia = Nuevo("LEJIA", "Lejía", "Caja de 12 botellas de 1 l", 10.80m, drogueria, 150);
        var suavizante = Nuevo("SUAVIZANTE", "Suavizante", "Caja de 4 garrafas de 2 l", 17.00m, drogueria, 0, enCatalogo: false);
        datos.Productos.AddRange(
            agua,
            Nuevo("ZUMO", "Zumo de naranja", "Caja de 12 briks de 1 l", 14.40m, bebidas, 0),
            aceite,
            Nuevo("ARROZ", "Arroz redondo", "Caja de 10 paquetes de 1 kg", 12.50m, alimentacion, 300),
            Nuevo("GALLETAS", "Galletas María", "Caja de 20 paquetes", 18.00m, alimentacion, 260),
            Nuevo("LENTEJAS", "Lentejas pardinas", "Caja de 10 paquetes de 1 kg", 16.00m, alimentacion, 90),
            lejia,
            Nuevo("DETERGENTE", "Detergente líquido", "Caja de 4 garrafas de 3 l", 22.00m, drogueria, 70),
            suavizante,
            Nuevo("SAL", "Sal fina", "Caja de 24 paquetes de 1 kg", 9.60m, null, 120),
            Nuevo("VINAGRE", "Vinagre de vino", "Caja de 12 botellas de 1 l", null, alimentacion, 80));

        var plaza = new Tienda { Id = 1, Nombre = "Tienda de la plaza del mercado", Ruta = "Centro" };
        var pruebas = new Tienda { Id = 2, Nombre = "Tienda de pruebas", Ruta = "Norte" };
        datos.Tiendas.AddRange(plaza, pruebas);

        datos.Usuarios.AddRange(
            new Usuario { Nombre = "plaza", HuellaContrasena = Acceso.Huella("plaza"), Rol = Rol.Tienda, TiendaId = plaza.Id },
            new Usuario { Nombre = "pruebas", HuellaContrasena = Acceso.Huella("pruebas"), Rol = Rol.Tienda, TiendaId = pruebas.Id },
            new Usuario { Nombre = "almacen", HuellaContrasena = Acceso.Huella("almacen"), Rol = Rol.Almacen });

        Pedido Anterior(Tienda tienda, int numero, EstadoPedido estado, DateOnly dia, params (Producto Producto, int Cajas)[] lineas)
        {
            var pedido = new Pedido
            {
                Tienda = tienda, TiendaId = tienda.Id, Numero = numero, Estado = estado,
                FechaConfirmacion = dia, DiaEntrega = CalendarioReparto.DiaDeEntrega(dia),
            };
            foreach (var (producto, cajas) in lineas)
            {
                pedido.Anadir(producto, cajas);
            }

            return pedido;
        }

        // El primero lleva un producto que ya se ha retirado del catálogo, para repetirlo.
        var pedidos = new[]
        {
            Anterior(plaza, 1001, EstadoPedido.Entregado, new DateOnly(2026, 10, 13), (agua, 12), (aceite, 6), (lejia, 2), (suavizante, 3)),
            Anterior(plaza, 1002, EstadoPedido.Preparado, new DateOnly(2026, 10, 27), (agua, 12), (aceite, 6), (lejia, 2)),
            Anterior(plaza, 1003, EstadoPedido.Confirmado, new DateOnly(2026, 10, 29), (agua, 6), (lejia, 4)),
            Anterior(pruebas, 1004, EstadoPedido.Confirmado, new DateOnly(2026, 10, 29), (aceite, 3)),
        };
        datos.Pedidos.AddRange(pedidos);

        foreach (var pedido in pedidos)
        {
            datos.PedidosEnAlmacen.Add(new PedidoEnAlmacen
            {
                Numero = pedido.Numero!.Value,
                Tienda = pedido.Tienda.Nombre,
                Lineas = string.Join("; ", pedido.LineasEnOrden.Select(l => $"{l.Producto.Nombre} x{l.Cajas}")),
                DiaReparto = pedido.DiaEntrega!.Value,
            });
        }

        datos.SaveChanges();
    }
}
