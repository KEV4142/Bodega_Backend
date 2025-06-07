﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modelo.Custom;
using Modelo.Entidades;

namespace Persistencia;

public partial class BackendContext : IdentityDbContext<Usuario>
{
    public BackendContext()
    {
    }

    public BackendContext(DbContextOptions<BackendContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Lote> Lotes { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<SalidaDet> SalidaDets { get; set; }

    public virtual DbSet<SalidaEnc> SalidaEncs { get; set; }

    public virtual DbSet<Sucursal> Sucursales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var adminRoleId = "51df7aae-a506-46ff-8e34-9f2f0c661885";
        var clientRoleId = "368cb24e-03d3-4a01-b558-dbde9b33272c";

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.LoteID).HasName("pkLotesID");

            entity.Property(e => e.Costo)
                .HasColumnType("decimal(5, 2)");

            entity.Property(e => e.CampoConcurrencia)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Producto).WithMany(p => p.Lotes)
                .HasForeignKey(d => d.ProductoID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkLotesProductoID");

            entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("ckLotesCantidad", "Cantidad > -1");
                    tb.HasCheckConstraint("ckLotesCosto", "Costo > 0");
                });
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoID).HasName("pkProductosID");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A");
            
            entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("ckProductosEstado", "Estado IN('A','I','B')");
                });
        });

        modelBuilder.Entity<SalidaDet>(entity =>
        {
            entity.HasKey(e => new { e.SalidaID, e.LoteID }).HasName("pkSalidaDetID");

            entity.ToTable("SalidaDet");

            entity.Property(e => e.Costo).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Lote).WithMany(p => p.SalidaDets)
                .HasForeignKey(d => d.LoteID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkSalidaDetLoteID");

            entity.HasOne(d => d.Salida).WithMany(p => p.SalidaDets)
                .HasForeignKey(d => d.SalidaID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkSalidaDetSalidaID");

            entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("ckSalidaDetCantidad", "Cantidad > 0");
                    tb.HasCheckConstraint("ckSalidaDetCosto", "Costo > 0");
                });
        });

        modelBuilder.Entity<SalidaEnc>(entity =>
        {
            entity.HasKey(e => e.SalidaID).HasName("pkSalidaEncID");

            entity.ToTable("SalidaEnc");

            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("E");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaRecibido)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime");
            entity.Property(e => e.UsuarioID).HasMaxLength(450);
            entity.Property(e => e.UsuarioRecibe).HasMaxLength(450);

            entity.HasOne(d => d.Sucursales).WithMany(p => p.SalidaEncs)
                .HasForeignKey(d => d.SucursalID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkSalidaEncSucursalID");
            entity.HasOne(d => d.Usuario).WithMany(p => p.SalidasEnviadas)
                .HasForeignKey(d => d.UsuarioID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fkSalidaEncUsuarioID");

            entity.HasOne(d => d.UsuarioRecibeRelacion).WithMany(p => p.SalidasRecibidas)
                .HasForeignKey(d => d.UsuarioRecibe)
                .HasConstraintName("fkSalidaEncUsuarioRecibe");

            entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("ckSalidaEncEstado", "Estado IN('E','R','B')");
                    tb.HasCheckConstraint("ckSalidaEncFechaRecibido", "FechaRecibido IS NULL OR Fecha < FechaRecibido");
                });
        });

        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.SucursalID).HasName("pkSucursalID");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A");
            entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("ckSucursalesEstado", "Estado IN('A','I','B')");
                });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A");
            entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("ckUsuarioEstado", "Estado IN('A','I','B')");
                });
        });         

        modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = CustomRoles.ADMINBODEGA,
                    NormalizedName = CustomRoles.ADMINBODEGA
                },
                new IdentityRole
                {
                    Id = clientRoleId,
                    Name = CustomRoles.CLIENT,
                    NormalizedName = CustomRoles.CLIENT
                }
            );

        modelBuilder.Entity<Sucursal>().HasData(
            new Sucursal { SucursalID = 1, Descripcion = "CENTRO", Direccion = "BO. CENTRO" },
            new Sucursal { SucursalID = 2, Descripcion = "ALTARA", Direccion = "MALL ALTARA" },
            new Sucursal { SucursalID = 3, Descripcion = "GALERIAS", Direccion = "MALL GALERIAS DEL VALLE" },
            new Sucursal { SucursalID = 4, Descripcion = "MEGAMALL", Direccion = "MALL MEGAMALL" }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto { ProductoID = 1, Descripcion = "ALKAZERSER" },
            new Producto { ProductoID = 2, Descripcion = "JARABE PARA LA TOS" },
            new Producto { ProductoID = 3, Descripcion = "PANADOL" }
        );

        modelBuilder.Entity<Lote>().HasData(
            new Lote { LoteID = 1, ProductoID = 1, FechaVencimiento = new DateOnly(2027, 10, 1), Costo = 10, Cantidad = 100 },
            new Lote { LoteID = 2, ProductoID = 1, FechaVencimiento = new DateOnly(2025, 11, 1), Costo = 10, Cantidad = 50 },
            new Lote { LoteID = 3, ProductoID = 1, FechaVencimiento = new DateOnly(2026, 12, 1), Costo = 10, Cantidad = 100 },
            new Lote { LoteID = 4, ProductoID = 2, FechaVencimiento = new DateOnly(2027, 1, 1), Costo = 100, Cantidad = 500 },
            new Lote { LoteID = 5, ProductoID = 2, FechaVencimiento = new DateOnly(2027, 2, 1), Costo = 100, Cantidad = 500 },
            new Lote { LoteID = 6, ProductoID = 2, FechaVencimiento = new DateOnly(2025, 12, 1), Costo = 100, Cantidad = 250 },
            new Lote { LoteID = 7, ProductoID = 3, FechaVencimiento = new DateOnly(2025, 11, 1), Costo = 25, Cantidad = 0 },
            new Lote { LoteID = 8, ProductoID = 3, FechaVencimiento = new DateOnly(2025, 12, 1), Costo = 25, Cantidad = 100 }
        );

        base.OnModelCreating(modelBuilder);
    }

}
