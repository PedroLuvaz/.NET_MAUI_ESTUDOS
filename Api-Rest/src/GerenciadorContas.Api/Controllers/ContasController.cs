using GerenciadorContas.Api.Data;
using GerenciadorContas.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorContas.Api.Controllers
{
    [ApiController] // Marca esta classe como um Controller de API
    [Route("api/[controller]")] // Define a URL base: "http://.../api/contas"
    public class ContasController : ControllerBase
    {
        // Variável para acessar o banco de dados
        private readonly AppDbContext _context;

        // O Construtor recebe o AppDbContext (Injeção de Dependência)
        public ContasController(AppDbContext context)
        {
            _context = context;
        }

        // === Endpoint 1: Adicionar uma nova conta ===
        // Rota: POST http://.../api/contas
        [HttpPost]
        public async Task<IActionResult> AdicionarConta([FromBody] ContaInputDto input)
        {
            // Cria um novo objeto "Conta" a partir dos dados de entrada
            var novaConta = new Conta
            {
                Nome = input.Nome,
                Valor = input.Valor,
                DataRegistro = DateTime.UtcNow // Pega a data/hora atual
            };

            // Adiciona no EF Core e Salva as mudanças no banco
            _context.Contas.Add(novaConta);
            await _context.SaveChangesAsync();

            // Retorna um status 201 (Created) e o objeto criado
            return CreatedAtAction(nameof(GetContaPorId), new { id = novaConta.Id }, novaConta);
        }

        // (Este é um método auxiliar para o endpoint de cima)
        // Rota: GET http://.../api/contas/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContaPorId(int id)
        {
            var conta = await _context.Contas.FindAsync(id);
            if (conta == null)
            {
                return NotFound(); // Retorna 404 se não achar
            }
            return Ok(conta); // Retorna 200 OK e a conta
        }

        // === Endpoint 2: Ver o somatório total ===
        // Rota: GET http://.../api/contas/somatorio
        [HttpGet("somatorio")]
        public async Task<IActionResult> GetSomatorio()
        {
            // Soma a coluna "Valor" de todas as entradas na tabela "Contas"
            var total = await _context.Contas.SumAsync(c => c.Valor);

            // Retorna um JSON simples: { "totalGasto": 123.45 }
            return Ok(new { totalGasto = total });
        }

        // === Endpoint Bônus: Ver todas as contas ===
        // Rota: GET http://.../api/contas
        [HttpGet]
        public async Task<IActionResult> GetTodasContas()
        {
            var contas = await _context.Contas
                                    .OrderByDescending(c => c.DataRegistro)
                                    .ToListAsync();

            return Ok(contas);
        }
    }
}