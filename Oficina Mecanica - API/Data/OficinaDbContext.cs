
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.Data
{
    public class OficinaDbContext : DbContext
    {
        public OficinaDbContext(DbContextOptions<OficinaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("Clientes");
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
                entity.Property(c => c.CPF).IsRequired().HasMaxLength(11);
                entity.Property(c => c.Telefone).IsRequired().HasMaxLength(11);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(100);

                entity.HasIndex(c => c.CPF).IsUnique();
            });

            modelBuilder.Entity<Veiculo>(entity =>
            {
                entity.ToTable("Veiculos");
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Placa).IsRequired().HasMaxLength(7);
                entity.Property(v => v.Modelo).IsRequired().HasMaxLength(50);
                entity.Property(v => v.Marca).IsRequired().HasMaxLength(50);

                entity.HasIndex(v => v.Placa).IsUnique();

                entity.HasOne(v => v.Cliente)
                    .WithMany(c => c.Veiculos)
                    .HasForeignKey(v => v.ClienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrdemServico>(entity =>
            {
                entity.ToTable("OrdensServico");
                entity.HasKey(o => o.Id);

                entity.Property(o => o.DescricaoServico).IsRequired().HasMaxLength(500);
                entity.Property(o => o.Status).IsRequired().HasMaxLength(20);
                entity.Property(o => o.ValorTotal).HasPrecision(18, 2);

                entity.HasOne(o => o.Veiculo)
                    .WithMany(v => v.OrdensServico)
                    .HasForeignKey(o => o.VeiculoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Nome).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.SenhaHash).IsRequired().HasMaxLength(200);
                entity.Property(u => u.Perfil).IsRequired().HasMaxLength(20);

                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
