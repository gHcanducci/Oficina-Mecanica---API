using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Oficina_Mecanica___API.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O cliente do veículo é obrigatório.")]
        public int ClienteId { get; set; }

        [JsonIgnore]
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "A placa é obrigatória.")]
        [RegularExpression(@"^[A-Za-z]{3}\d[A-Za-z0-9]\d{2}$", ErrorMessage = "A placa deve estar no formato ABC1234 ou ABC1D23.")]
        public string Placa { get; set; } = string.Empty;

        [Required(ErrorMessage = "O modelo é obrigatório.")]
        [StringLength(50, ErrorMessage = "O modelo deve ter no máximo 50 caracteres.")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A marca é obrigatória.")]
        [StringLength(50, ErrorMessage = "A marca deve ter no máximo 50 caracteres.")]
        public string Marca { get; set; } = string.Empty;

        [Range(1950, 2100, ErrorMessage = "O ano de fabricação deve estar entre 1950 e 2100.")]
        public int AnoFabricacao { get; set; }

        [JsonIgnore]
        public List<OrdemServico>? OrdensServico { get; set; } = new List<OrdemServico>();
    }
}
