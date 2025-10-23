using GerenciadorContas.Api.Data; // IMPORTAR A PASTA DATA
using Microsoft.EntityFrameworkCore; // IMPORTAR O EF CORE

var builder = WebApplication.CreateBuilder(args);

// --- INÍCIO DA CONFIGURAÇÃO ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ADICIONA O SERVIÇO DE "CONTROLLERS" QUE VAMOS USAR
builder.Services.AddControllers();
// --- FIM DA CONFIGURAÇÃO ---

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseAuthorization();

// DIZ PARA A API USAR OS "CONTROLLERS" QUE CRIAMOS
app.MapControllers();

app.Run();