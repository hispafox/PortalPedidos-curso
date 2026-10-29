using System.Globalization;
using System.Text;
using PortalPedidos.Datos;

namespace PortalPedidos.Dominio;

public record ExistenciasDe(int Cajas, bool SonDisponibles)
{
    public bool Agotado => Cajas <= 0;
}

public class Catalogo(PortalContexto datos)
{
    public IReadOnlyList<Familia> Familias() =>
        datos.Productos.AsEnumerable().Where(p => p.SeVende).Select(p => p.Familia!)
            .DistinctBy(f => f.Id).OrderBy(f => f.Nombre).ToList();

    public IReadOnlyList<Producto> ProductosDe(int familiaId) =>
        datos.Productos.AsEnumerable().Where(p => p.SeVende && p.FamiliaId == familiaId)
            .OrderBy(p => p.Nombre).ToList();

    // Busca por una parte del nombre, sin distinguir mayúsculas, minúsculas ni tildes.
    public IReadOnlyList<Producto> Buscar(string texto)
    {
        var buscado = Normalizar(texto);
        if (buscado.Length == 0)
        {
            return [];
        }

        return datos.Productos.AsEnumerable()
            .Where(p => p.SeVende && Normalizar(p.Nombre).Contains(buscado))
            .OrderBy(p => p.Nombre).ToList();
    }

    public Producto? Producto(string codigo) => datos.Productos.Find(codigo);

    // Las disponibles solo se enseñan cuando el almacén las ha rellenado en todos los productos;
    // hasta entonces, las totales del cierre del día anterior para todas las tiendas.
    public ExistenciasDe Existencias(Producto producto)
    {
        var todasRellenas = datos.Productos.All(p => p.CajasReservadas != null);
        return todasRellenas
            ? new ExistenciasDe(producto.CajasTotales - producto.CajasReservadas!.Value, true)
            : new ExistenciasDe(producto.CajasTotales, false);
    }

    // Para un producto retirado, otro de la misma familia que sí se vende.
    public Producto? Parecido(Producto retirado) =>
        datos.Productos.AsEnumerable()
            .Where(p => p.SeVende && p.FamiliaId == retirado.FamiliaId && p.Codigo != retirado.Codigo)
            .OrderBy(p => p.Nombre).FirstOrDefault();

    public static string Normalizar(string texto)
    {
        var descompuesto = texto.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sinTildes = descompuesto.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        return new string(sinTildes.ToArray()).Normalize(NormalizationForm.FormC);
    }
}
