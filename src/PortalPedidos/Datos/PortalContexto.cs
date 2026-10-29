using Microsoft.EntityFrameworkCore;
using PortalPedidos.Dominio;

namespace PortalPedidos.Datos;

// Todo vive en memoria (proveedor InMemory de EF Core). Para pasar a una base de datos
// de verdad basta con cambiar el proveedor en Program.cs.
public class PortalContexto(DbContextOptions<PortalContexto> opciones) : DbContext(opciones)
{
    public DbSet<Familia> Familias => Set<Familia>();

    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<Tienda> Tiendas => Set<Tienda>();

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Pedido> Pedidos => Set<Pedido>();

    public DbSet<PedidoEnAlmacen> PedidosEnAlmacen => Set<PedidoEnAlmacen>();

    protected override void OnModelCreating(ModelBuilder modelo)
    {
        modelo.Entity<Producto>().HasKey(p => p.Codigo);
        modelo.Entity<Usuario>().HasKey(u => u.Nombre);
        modelo.Entity<PedidoEnAlmacen>().HasKey(p => p.Numero);
        modelo.Entity<LineaPedido>()
            .HasOne(l => l.Producto).WithMany().HasForeignKey(l => l.CodigoProducto);
        modelo.Entity<Pedido>().Ignore(p => p.LineasEnOrden);
        modelo.Entity<Pedido>().Navigation(p => p.Lineas).AutoInclude();
        modelo.Entity<LineaPedido>().Navigation(l => l.Producto).AutoInclude();
        modelo.Entity<Pedido>().Navigation(p => p.Tienda).AutoInclude();
        modelo.Entity<Producto>().Navigation(p => p.Familia).AutoInclude();
    }
}
