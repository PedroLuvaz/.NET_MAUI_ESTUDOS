using System.Text.Json.Serialization;

namespace GerenciadorContas.Maui.Models
{
    public class SomatorioDto
    {
        [JsonPropertyName("totalGasto")]
        public decimal TotalGasto { get; set; }
    }
}