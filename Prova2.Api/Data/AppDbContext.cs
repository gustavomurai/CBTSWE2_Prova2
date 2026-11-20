
// ============================================================
// PROJETO: PROVA II - CBTSWE2
// INSTITUIÇÃO: IFSP - CAMPUS CUBATÃO
//
// INTEGRANTES:
// Gustavo Cerqueira Murai - CB3033261
// Igor Cerqueira Murai - CB3033295
//
// ============================================================


using Microsoft.EntityFrameworkCore;
using Prova2.Api.Models;

namespace Prova2.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.SenhaHash)
                .IsRequired();

            entity.HasIndex(u => u.Nome)
                .IsUnique();
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.IdUsuarioCadastro)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(p => p.IdUsuarioUpdate)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}