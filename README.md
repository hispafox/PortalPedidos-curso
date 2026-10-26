# Portal de pedidos

El portal donde las tiendas hacen sus pedidos. Es el proyecto de ejemplo del curso de Azure DevOps:
el código es pequeño a propósito, porque lo que se practica con él es el trabajo del equipo, con
ramas, pull requests, conflictos y la pipeline.

- `src/PortalPedidos`: la web, con el catálogo, el pedido, la confirmación y las existencias.
- `src/PortalPedidos/Pantallas/confirmacion.html`: el texto de la pantalla de confirmación. Lo que va
  entre llaves (`{numero}`, `{dia_entrega}`) lo rellena el portal.
- `tests/PortalPedidos.Pruebas`: las pruebas automáticas.
- `azure-pipelines.yml`: la pipeline, que compila y pasa las pruebas en cada subida.

Para compilar y probar en tu equipo hace falta el SDK de .NET 10:

```
dotnet build
dotnet test
```
