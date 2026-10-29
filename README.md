# Portal de pedidos

El portal donde las tiendas de Distribuciones Ondalia hacen sus pedidos. Es el proyecto de ejemplo
del curso de Azure DevOps: el código es pequeño a propósito, porque lo que se practica con él es el
trabajo del equipo, con ramas, pull requests, conflictos y la pipeline.

- `src/PortalPedidos/Pages`: la web. Para las tiendas, el catálogo por familias con búsqueda y
  existencias, el pedido, la confirmación y sus pedidos anteriores. Para el almacén, los pedidos nuevos
  y la lista de preparación de su aplicación.
- `src/PortalPedidos/Dominio`: las reglas del pedido, el catálogo y la aplicación del almacén.
- `src/PortalPedidos/Datos`: los datos. **Todo vive en memoria** (EF Core con el proveedor InMemory) y
  `DatosDePrueba.cs` siembra tiendas, productos y pedidos al arrancar. Al reiniciar el portal se
  vuelve a ese estado.
- `src/PortalPedidos/Pantallas/confirmacion.html`: el texto de la pantalla de confirmación. Lo que va
  entre llaves (`{numero}`, `{dia_entrega}`) lo rellena el portal.
- `src/PortalPedidos/appsettings.json`: el pedido mínimo (`Portal:PedidoMinimo`), que se cambia sin
  tocar el código.
- `tests/PortalPedidos.Pruebas`: las pruebas automáticas, una clase por parte del portal.
- `azure-pipelines.yml`: la pipeline, que compila y pasa las pruebas en cada subida.

## Usuarios de prueba

La entrada está simulada: no usa ASP.NET Core Identity, solo una cookie con el usuario y su papel.

| Usuario | Contraseña | Entra como |
|---|---|---|
| `plaza` | `plaza` | Tienda de la plaza del mercado, con tres pedidos anteriores |
| `pruebas` | `pruebas` | Tienda de pruebas |
| `almacen` | `almacen` | Almacén |

## Compilar y probar

Hace falta el SDK de .NET 10:

```
dotnet build
dotnet test
```
