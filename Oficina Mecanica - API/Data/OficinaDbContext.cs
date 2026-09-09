
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
    }
}