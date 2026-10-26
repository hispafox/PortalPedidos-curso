namespace PortalPedidos.Dominio;

public static class Catalogo
{
    public static readonly IReadOnlyList<Producto> Productos =
    [
        new("AGUA", "Agua mineral", "Caja de 6 botellas de 1,5 l", 3.90m),
        new("ACEITE", "Aceite de girasol", "Caja de 12 botellas de 1 l", 21.60m),
        new("LEJIA", "Lejía", "Caja de 12 botellas de 1 l", 10.80m),
        new("ARROZ", "Arroz redondo", "Caja de 10 paquetes de 1 kg", 12.50m),
        new("GALLETAS", "Galletas María", "Caja de 20 paquetes", 18.00m),
    ];

    public static Producto? Buscar(string codigo) =>
        Productos.FirstOrDefault(p => p.Codigo == codigo);
}
