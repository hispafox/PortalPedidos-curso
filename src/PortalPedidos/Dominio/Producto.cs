namespace PortalPedidos.Dominio;

public class Familia
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "";
}

public class Producto
{
    public string Codigo { get; set; } = "";

    public string Nombre { get; set; } = "";

    public string Formato { get; set; } = "";

    // La aplicación del almacén no siempre lo rellena.
    public decimal? PrecioCaja { get; set; }

    public int? FamiliaId { get; set; }

    public Familia? Familia { get; set; }

    // Los productos entran y salen del catálogo cada temporada.
    public bool EnCatalogo { get; set; } = true;

    // Las del cierre del día anterior.
    public int CajasTotales { get; set; }

    // Las reservadas para otros pedidos. Vacío mientras el almacén no lo rellene.
    public int? CajasReservadas { get; set; }

    // Solo sale en el catálogo lo que tiene familia y precio y no se ha retirado.
    public bool SeVende => EnCatalogo && FamiliaId is not null && PrecioCaja is not null;
}

public class Tienda
{
    public int Id { get; set; }

    public string Nombre { get; set; } = "";

    public string Ruta { get; set; } = "";
}

public enum Rol
{
    Tienda,
    Almacen,
}

// Entrada simulada: usuarios de prueba fijos, sin ASP.NET Core Identity.
public class Usuario
{
    public string Nombre { get; set; } = "";

    // La contraseña no se guarda en claro.
    public string HuellaContrasena { get; set; } = "";

    public Rol Rol { get; set; }

    public int? TiendaId { get; set; }
}
