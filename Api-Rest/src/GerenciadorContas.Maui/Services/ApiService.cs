using GerenciadorContas.Maui.Models;
using System.Diagnostics;
using System.Net.Http.Json; // Pacote importante para POST/GET de JSON
using System.Text.Json;

namespace GerenciadorContas.Maui.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService()
        {
            _httpClient = new HttpClient();

            // --- ESTA É A LÓGICA DE URL MÁGICA ---
            // O .NET MAUI fornece uma forma de detetar a plataforma.
            // Se estivermos a rodar no Windows, usamos localhost.
            // Se estivermos a rodar num Emulador Android, o 'localhost'
            // do emulador é '10.0.2.2'.

            _baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:8080"
                : "http://localhost:8080";
        }

        public async Task AdicionarContaAsync(ContaInputDto novaConta)
        {
            try
            {
                var url = $"{_baseUrl}/api/contas";

                // Serializa o nosso objeto C# para JSON e envia como POST
                var response = await _httpClient.PostAsJsonAsync(url, novaConta);

                // Verifica se a API retornou sucesso (código 2xx)
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AdicionarContaAsync] Erro: {ex.Message}");
                // (Numa app real, mostraríamos um pop-up de erro ao utilizador)
            }
        }

        public async Task<SomatorioDto> GetSomatorioAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/contas/somatorio";

                // Faz um GET e pede ao HttpClient para converter o JSON
                // de volta para o nosso objeto SomatorioDto
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Lê o conteúdo da resposta e desserializa
                return await response.Content.ReadFromJsonAsync<SomatorioDto>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetSomatorioAsync] Erro: {ex.Message}");
                return new SomatorioDto { TotalGasto = 0 }; // Retorna 0 em caso de erro
            }
        }

        public async Task<List<Conta>> GetTodasContasAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/contas";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Lê e desserializa a lista de contas
                return await response.Content.ReadFromJsonAsync<List<Conta>>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GetTodasContasAsync] Erro: {ex.Message}");
                return new List<Conta>(); // Retorna lista vazia em caso de erro
            }
        }
    }
}