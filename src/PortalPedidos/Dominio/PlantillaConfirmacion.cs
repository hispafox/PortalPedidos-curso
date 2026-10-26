namespace PortalPedidos.Dominio;

// Rellena los datos entre llaves de la pantalla de confirmación.
// Lo que no reconoce lo deja tal cual: para el compilador, la plantilla es solo texto.
public static class PlantillaConfirmacion
{
    private static readonly string[] Dias =
        ["domingo", "lunes", "martes", "miércoles", "jueves", "viernes", "sábado"];

    private static readonly string[] Meses =
        ["enero", "febrero", "marzo", "abril", "mayo", "junio",
         "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre"];

    public static string Rellenar(string plantilla, int numero, DateOnly diaEntrega) =>
        plantilla
            .Replace("{numero}", numero.ToString())
            .Replace("{dia_entrega}", ConNombre(diaEntrega));

    public static string ConNombre(DateOnly dia) =>
        $"{Dias[(int)dia.DayOfWeek]} {dia.Day} de {Meses[dia.Month - 1]}";
}
