using Microsoft.EntityFrameworkCore;
using SIOL.Models;
using Tienda.API.Models;

namespace Tienda.API.Data
{
    public class TiendaDbContext : DbContext
    {
        public TiendaDbContext(DbContextOptions<TiendaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<TipoProducto> TiposProducto { get; set; }
        public DbSet<UnidadMedida> UnidadesMedida { get; set; }
        public DbSet<MaestroTabla> Maestros { get; set; }
        public DbSet<MaestroTablaDetalle> MaestroDetalles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<CuentaPorCobrar> CuentasPorCobrar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MaestroTabla>(entity => {
                entity.ToTable("MaestroTabla");
                entity.HasKey(e => e.MaestroTablaID);
                entity.Property(e => e.MaestroTablaID).HasColumnName("MaestroTablaID");
                entity.Property(e => e.Codigo).HasColumnName("Codigo");
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion");
                entity.Property(e => e.Activo).HasColumnName("Activo");
            });

            modelBuilder.Entity<MaestroTablaDetalle>(entity => {
                entity.ToTable("MaestroTablaDetalle");
                entity.HasKey(e => e.MaestroTablaDetalleID);
                entity.HasAlternateKey(e => e.Valor);
                entity.Property(e => e.MaestroTablaDetalleID).HasColumnName("MaestroTablaDetalleID");
                entity.Property(e => e.MaestroTablaID).HasColumnName("MaestroTablaID");
                entity.Property(e => e.Code).HasColumnName("Code");
                entity.Property(e => e.Valor).HasColumnName("Valor");
                entity.Property(e => e.Texto).HasColumnName("Texto");
                entity.Property(e => e.Activo).HasColumnName("Activo");

                entity.HasOne(d => d.MaestroTabla)
                      .WithMany(m => m.Detalles)
                      .HasForeignKey(d => d.MaestroTablaID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Marca>(entity => {
                entity.ToTable("marcas");
                entity.HasKey(e => e.MarcaID);
                entity.Property(e => e.MarcaID).HasColumnName("marcaid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            modelBuilder.Entity<TipoProducto>(entity => {
                entity.ToTable("tiposproducto");
                entity.HasKey(e => e.TipoProductoID);
                entity.Property(e => e.TipoProductoID).HasColumnName("tipoproductoid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            modelBuilder.Entity<UnidadMedida>(entity => {
                entity.ToTable("unidadesmedida");
                entity.HasKey(e => e.UnidadMedidaID);
                entity.Property(e => e.UnidadMedidaID).HasColumnName("unidadmedidaid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Abreviatura).HasColumnName("abreviatura");
                entity.Property(e => e.Activo).HasColumnName("activo");
            });

            modelBuilder.Entity<Producto>(entity => {
                entity.ToTable("productos");
                entity.HasKey(e => e.ProductoID);
                entity.Property(e => e.ProductoID).HasColumnName("productoid");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Precio).HasColumnName("precio");
                entity.Property(e => e.Stock).HasColumnName("stock");
                entity.Property(e => e.StockMinimo).HasColumnName("stockminimo");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.CategoriaID).HasColumnName("categoriaid");
                entity.Property(e => e.CodigoBarras).HasColumnName("codigobarras");
                entity.Property(e => e.ImagenUrl).HasColumnName("imagenurl");
                entity.Property(e => e.Activo).HasColumnName("activo");
                entity.Property(e => e.FechaModificacion).HasColumnName("fechamodificacion");
                entity.Property(e => e.MarcaID).HasColumnName("marcaid");
                entity.Property(e => e.TipoProductoID).HasColumnName("tipoproductoid");
                entity.Property(e => e.UnidadMedidaID).HasColumnName("unidadmedidaid");
                entity.Property(e => e.ValorVolumen).HasColumnName("valorvolumen");
            });

            modelBuilder.Entity<Cliente>(entity => {
                entity.ToTable("clientes");
                entity.HasKey(e => e.ClienteID);
                entity.Property(e => e.ClienteID).HasColumnName("clienteid");
                entity.Property(e => e.NombreRazonSocial).HasColumnName("nombrerazonsocial");
                entity.Property(e => e.NumeroDocumento).HasColumnName("numerodocumento");
                entity.Property(e => e.TipoDocumento).HasColumnName("tipodocumento");
                entity.Property(e => e.Telefono).HasColumnName("telefono");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Activo).HasColumnName("activo");

                entity.HasOne(c => c.MaestroTablaDetalle)
                      .WithMany()
                      .HasForeignKey(c => c.TipoDocumento)
                      .HasPrincipalKey(d => d.Valor)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Venta>(entity => {
                entity.ToTable("ventas");
                entity.HasKey(e => e.VentaID);
                entity.Property(e => e.VentaID).HasColumnName("ventaid");
                entity.Property(e => e.ClienteID).HasColumnName("clienteid");
                entity.Property(e => e.TipoComprobante).HasColumnName("tipocomprobante");
                entity.Property(e => e.NumeroComprobante).HasColumnName("numerocomprobante");
                entity.Property(e => e.Total).HasColumnName("total");
                entity.Property(e => e.EsCredito).HasColumnName("escredito");
                entity.Property(e => e.FechaRegistro).HasColumnName("fecharegistro");

                entity.Property(e => e.EsEfectivo).HasColumnName("esefectivo");
                entity.Property(e => e.MontoEfectivo).HasColumnName("montoefectivo");
                entity.Property(e => e.EsDigital).HasColumnName("esdigital");
                entity.Property(e => e.MontoDigital).HasColumnName("montodigital");

                entity.HasOne(v => v.Cliente).WithMany().HasForeignKey(v => v.ClienteID);
            });

            modelBuilder.Entity<DetalleVenta>(entity => {
                entity.ToTable("detalleventas");
                entity.HasKey(e => e.DetalleVentaID);
                entity.Property(e => e.DetalleVentaID).HasColumnName("detalleventaid");
                entity.Property(e => e.VentaID).HasColumnName("ventaid");
                entity.Property(e => e.ProductoID).HasColumnName("productoid");
                entity.Property(e => e.Cantidad).HasColumnName("cantidad");
                entity.Property(e => e.PrecioUnitario).HasColumnName("preciounitario");
                entity.Property(e => e.Subtotal).HasColumnName("subtotal");
                entity.Property(e => e.FechaRegistro).HasColumnName("fecharegistro");

                entity.HasOne(d => d.Venta).WithMany(v => v.Detalles).HasForeignKey(d => d.VentaID);
                entity.HasOne(d => d.Producto).WithMany().HasForeignKey(d => d.ProductoID);
            });

            modelBuilder.Entity<CuentaPorCobrar>(entity => {
                entity.ToTable("cuentasporcobrar");
                entity.HasKey(e => e.CuentaCobrarID);
                entity.Property(e => e.CuentaCobrarID).HasColumnName("cuentacobrarid");
                entity.Property(e => e.VentaID).HasColumnName("ventaid");
                entity.Property(e => e.MontoTotal).HasColumnName("montototal");
                entity.Property(e => e.MontoPagado).HasColumnName("montopagado");
                entity.Property(e => e.SaldoPendiente).HasColumnName("saldopendiente");
                entity.Property(e => e.Detalle).HasColumnName("detalle");
                entity.Property(e => e.FechaRegistro).HasColumnName("fecharegistro");
                entity.HasOne(c => c.Venta).WithMany().HasForeignKey(c => c.VentaID);
            });
        }
    }
}