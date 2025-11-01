using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AgroLaboratorio.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AgroLaboratorio.Data;

public partial class AuthDbContext : IdentityDbContext<Usuario, Perfil, string>
{
    public AuthDbContext()
    {
    }

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=AuthPostgresConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity
                .ToTable("perfil");
            entity.Property(e => e.Id)
                .HasColumnName("cod_perfil");
            entity.HasKey(e => e.Id).HasName("perfil_pkey");
            entity.Property(e => e.CodUserAlta)
                .HasMaxLength(20)
                .HasColumnName("cod_user_alta");
            entity.Property(e => e.CodUserModif)
                .HasMaxLength(20)
                .HasColumnName("cod_user_modif");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaModif)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modif");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            

            entity.ToTable("usuario");

            /* Adaptaciones de la tabla */
            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .HasColumnName("cod_usuario");
            entity.HasKey(e => e.Id).HasName("usuario_pkey");
            //entity.Property(e => e.Clave)
            //    .HasMaxLength(20)
            //    .HasColumnName("clave");
            //entity.Property(e => e.CodPerfil).HasColumnName("cod_perfil");
            entity.Property(e => e.CodUserAlta)
                .HasMaxLength(20)
                .HasColumnName("cod_user_alta");
            entity.Property(e => e.CodUserModif)
                .HasMaxLength(20)
                .HasColumnName("cod_user_modif");
            entity.Property(e => e.Estado)
                .HasDefaultValue(true)
                .HasColumnName("estado");
            entity.Property(e => e.FechaAlta)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.FechaModif)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha_modif");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
        });
    }
}
