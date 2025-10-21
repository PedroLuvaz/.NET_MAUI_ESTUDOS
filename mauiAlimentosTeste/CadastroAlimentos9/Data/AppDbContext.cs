using CadastroAlimentos9.Models;
using Microsoft.EntityFrameworkCore;

namespace CadastroAlimentos9.Data;

public class AppDbContext : DbContext
{
    public DbSet<Alimento> Alimentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ATENÇÃO: Substitua pelo nome da sua instância SQL Server
        // Use a senha forte que você definiu no comando 'docker run'.
        string connectionString = "Server=172.30.15.34,1433;Database=CadastroAlimentosDB_Final;User ID=sa;Password=TesteForte123!;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);
    }
}