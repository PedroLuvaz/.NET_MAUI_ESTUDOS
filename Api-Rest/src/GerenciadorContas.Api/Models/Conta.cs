using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GerenciadorContas.Api.Models
{
    public class Conta
    {
        [Key] // Diz ao EF Core que esta é a Chave Primária
        public int Id { get; set; }

        [Required] // O Nome é obrigatório
        [StringLength(100)] // Define um tamanho máximo
        public string Nome { get; set; }

        [Required] // O Valor é obrigatório
        [Column(TypeName = "decimal(10, 2)")] // Define a precisão correta para dinheiro
        public decimal Valor { get; set; }

        public DateTime DataRegistro { get; set; }
    }
}