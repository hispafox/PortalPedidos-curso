namespace PortalPedidos.Pruebas;

// HU-02, HU-08, HU-18: catálogo, búsqueda y producto parecido.
public class CatalogoPruebas
{
    private readonly Entorno entorno = new();

    [Fact(DisplayName = "El catálogo enseña las familias")]
    public void ElCatalogoEnsenaLasFamilias()
    {
        Assert.Equal(["Alimentación", "Bebidas", "Droguería"], entorno.Catalogo.Familias().Select(f => f.Nombre));
    }

    [Fact(DisplayName = "Al elegir una familia salen sus productos")]
    public void AlElegirUnaFamiliaSalenSusProductos()
    {
        var bebidas = entorno.Catalogo.Familias().Single(f => f.Nombre == "Bebidas");

        Assert.Equal(["AGUA", "ZUMO"], entorno.Catalogo.ProductosDe(bebidas.Id).Select(p => p.Codigo));
    }

    [Theory(DisplayName = "Sin familia, sin precio o retirado, no sale en el catálogo")]
    [InlineData("SAL")]
    [InlineData("VINAGRE")]
    [InlineData("SUAVIZANTE")]
    public void LoQueNoSeVendeNoSale(string codigo)
    {
        var todos = entorno.Catalogo.Familias().SelectMany(f => entorno.Catalogo.ProductosDe(f.Id));

        Assert.DoesNotContain(todos, p => p.Codigo == codigo);
    }

    [Theory(DisplayName = "La búsqueda encuentra por una parte del nombre, sin mayúsculas ni tildes")]
    [InlineData("acei", "ACEITE")]
    [InlineData("LEJIA", "LEJIA")]
    [InlineData("lejía", "LEJIA")]
    [InlineData("maria", "GALLETAS")]
    public void LaBusquedaEncuentraPorUnaParteDelNombre(string texto, string codigo)
    {
        Assert.Contains(entorno.Catalogo.Buscar(texto), p => p.Codigo == codigo);
    }

    [Fact(DisplayName = "Si no hay ninguno, la búsqueda vuelve vacía")]
    public void SiNoHayNingunoLaBusquedaVuelveVacia()
    {
        Assert.Empty(entorno.Catalogo.Buscar("chocolate"));
    }

    [Fact(DisplayName = "La búsqueda no enseña productos retirados")]
    public void LaBusquedaNoEnsenaRetirados()
    {
        Assert.Empty(entorno.Catalogo.Buscar("suavizante"));
    }

    [Fact(DisplayName = "Para un producto retirado propone otro de su familia")]
    public void ParaUnRetiradoProponeOtroDeSuFamilia()
    {
        var parecido = entorno.Catalogo.Parecido(entorno.Producto("SUAVIZANTE"));

        Assert.NotNull(parecido);
        Assert.Equal("Droguería", parecido.Familia!.Nombre);
    }
}
