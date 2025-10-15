using CadastroAlimentos9.Models;
using Microsoft.EntityFrameworkCore;

namespace CadastroAlimentos9.Data;

public class AppDbContext : DbContext
{
    public DbSet<Alimento> Alimentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // ATENÇÃO: Substitua pelo nome da sua instância SQL Server
        string connectionString = "Server=PEDRO-LUCAS;Database=CadastroAlimentosDB_Final;Trusted_Connection=True;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);
    }
}