using GerenciadorContas.Maui.Models;

namespace GerenciadorContas.Maui.Services
{
    public interface IApiService
    {
        // Contrato para adicionar uma nova conta
        Task AdicionarContaAsync(ContaInputDto novaConta);

        // Contrato para obter o somatório
        Task<SomatorioDto> GetSomatorioAsync();

        // (Bónus) Contrato para obter todas as contas
        Task<List<Conta>> GetTodasContasAsync();
    }
}