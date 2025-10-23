using GerenciadorContas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorContas.Api.Data
{
    public class AppDbContext : DbContext
    {
        // O construtor é necessário para a "Injeção de Dependência"
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Diz ao EF Core que existe uma tabela chamada "Contas"
        // que será baseada no modelo "Conta"
        public DbSet<Conta> Contas { get; set; }
    }
}