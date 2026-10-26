using PortalPedidos.Dominio;

namespace PortalPedidos.Pruebas;

public class ExistenciasPruebas
{
    [Fact(DisplayName = "La consulta da las cajas que hay en el almacén")]
    public void LaConsultaDaLasCajasQueHayEnElAlmacen()
    {
        var consulta = new ConsultaExistencias([new Existencia("ACEITE", 210, 60)]);

        Assert.Equal(210, consulta.CajasDe("ACEITE"));
    }

    [Fact(DisplayName = "Un producto que no está en el almacén tiene cero cajas")]
    public void UnProductoQueNoEstaEnElAlmacenTieneCeroCajas()
    {
        var consulta = new ConsultaExistencias([]);

        Assert.Equal(0, consulta.CajasDe("ACEITE"));
    }
}
