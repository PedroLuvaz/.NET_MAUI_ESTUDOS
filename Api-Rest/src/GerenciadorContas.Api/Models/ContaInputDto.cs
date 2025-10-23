using System.ComponentModel.DataAnnotations;

namespace GerenciadorContas.Api.Models
{
    // DTO = Data Transfer Object
    // Usamos esta classe para validar os dados que chegam na API
    public class ContaInputDto
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [Range(0.01, 9999999.99)] // O valor não pode ser zero ou negativo
        public decimal Valor { get; set; }
    }
}