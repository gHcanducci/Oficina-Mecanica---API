using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Oficina_Mecanica___API.Models
{
    public class OrdemServico
    {
        public int Id { get; set; }

        public int VeiculoId { get; set; }

        [JsonIgnore]
        public Veiculo? Veiculo { get; set; }

        public DateTime DataAbertura { get; set; }
        public DateTime? DataConclusao { get; set; }

        [Required(ErrorMessage = "A descrição do serviço é obrigatória.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "A descrição do serviço deve ter entre 5 e 500 caracteres.")]
        public string DescricaoServico { get; set; } = string.Empty;

        [Range(0, 1000000, ErrorMessage = "O valor total deve estar entre 0 e 1.000.000.")]
        public decimal ValorTotal { get; set; }

        public string Status { get; set; } = StatusOrdemServico.Aberta;
    }
}
