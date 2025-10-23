// Usamos System.Text.Json para ajudar a "descodificar" o JSON da API
using System.Text.Json.Serialization;

namespace GerenciadorContas.Maui.Models
{
    public class Conta
    {
        // O [JsonPropertyName] garante que o nome bate 100% com o JSON
        // que a nossa API envia (embora C# já faça isso, é uma boa prática)
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonPropertyName("dataRegistro")]
        public DateTime DataRegistro { get; set; }
    }
}