using System;
using System.Collections.Generic;
using AgroLaboratorio.Models;
using Microsoft.EntityFrameworkCore;

namespace AgroLaboratorio.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ElemQuimico> ElemQuimicos { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<SolAnalisisCab> SolAnalisisCabs { get; set; }

    public virtual DbSet<SolAnalisisDet> SolAnalisisDets { get; set; }

    public virtual DbSet<Solubilidad> Solubilidads { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=AppPostgresConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ElemQuimico>(entity =>
        {
            entity.HasKey(e => e.CodElemento).HasName("elem_quimico_pkey");

            entity.ToTable("elem_quimico");

            entity.Property(e => e.CodElemento)
                .HasMaxLength(10)
                .HasColumnName("cod_elemento");
            entity.Property(e => e.CodUserAlta)
                .HasMaxLength(20)
                .HasColumnName("cod_user_alta");
            entity.Property(e => e.CodUserModif)
                .HasMaxLength(20)
                .HasColumnName("cod_user_modif");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaModif)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modif");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.CodPersona).HasName("persona_pkey");

            entity.ToTable("persona");

            entity.Property(e => e.CodPersona).HasColumnName("cod_persona");
            entity.Property(e => e.Apellido)
                .HasMaxLength(20)
                .HasColumnName("apellido");
            entity.Property(e => e.CodUserAlta)
                .HasMaxLength(20)
                .HasColumnName("cod_user_alta");
            entity.Property(e => e.CodUserModif)
                .HasMaxLength(20)
                .HasColumnName("cod_user_modif");
            entity.Property(e => e.Documento)
                .HasMaxLength(15)
                .HasColumnName("documento");
            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaModif)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modif");
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
            entity.Property(e => e.TipoPersona)
                .HasMaxLength(1)
                .HasColumnName("tipo_persona");
        });

        modelBuilder.Entity<SolAnalisisCab>(entity =>
        {
            entity.HasKey(e => e.CodAnalisis).HasName("sol_analisis_cab_pkey");

            entity.ToTable("sol_analisis_cab");

            entity.Property(e => e.CodAnalisis).HasColumnName("cod_analisis");
            entity.Property(e => e.CodCliente).HasColumnName("cod_cliente");
            entity.Property(e => e.CodUserAlta)
                .HasMaxLength(20)
                .HasColumnName("cod_user_alta");
            entity.Property(e => e.CodUserModif)
                .HasMaxLength(20)
                .HasColumnName("cod_user_modif");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(254)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(1)
                .HasColumnName("estado");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaModif)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modif");
            entity.Property(e => e.NombCliente)
                .HasMaxLength(150)
                .HasColumnName("nomb_cliente");

            entity.HasOne(d => d.CodClienteNavigation).WithMany(p => p.SolAnalisisCabs)
                .HasForeignKey(d => d.CodCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cliente");
        });

        modelBuilder.Entity<SolAnalisisDet>(entity =>
        {
            entity.HasKey(e => new { e.CodAnalisis, e.NroLinea }).HasName("pk_sol_analisis_det");

            entity.ToTable("sol_analisis_det");

            entity.Property(e => e.CodAnalisis).HasColumnName("cod_analisis");
            entity.Property(e => e.NroLinea).HasColumnName("nro_linea");
            entity.Property(e => e.CodElemento)
                .HasMaxLength(10)
                .HasColumnName("cod_elemento");
            entity.Property(e => e.CodSoluble).HasColumnName("cod_soluble");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.GarantiaVal)
                .HasPrecision(7, 3)
                .HasColumnName("garantia_val");

            entity.HasOne(d => d.CodAnalisisNavigation).WithMany(p => p.SolAnalisisDets)
                .HasForeignKey(d => d.CodAnalisis)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_sol_analisis");

            entity.HasOne(d => d.CodElementoNavigation).WithMany(p => p.SolAnalisisDets)
                .HasForeignKey(d => d.CodElemento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_elem_quimico");

            entity.HasOne(d => d.CodSolubleNavigation).WithMany(p => p.SolAnalisisDets)
                .HasForeignKey(d => d.CodSoluble)
                .HasConstraintName("fk_cod_soluble");
        });

        modelBuilder.Entity<Solubilidad>(entity =>
        {
            entity.HasKey(e => e.CodSoluble).HasName("solubilidad_pkey");

            entity.ToTable("solubilidad");

            entity.Property(e => e.CodSoluble).HasColumnName("cod_soluble");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .HasColumnName("descripcion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
