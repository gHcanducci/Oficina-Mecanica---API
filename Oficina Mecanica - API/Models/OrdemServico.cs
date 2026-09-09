namespace Oficina_Mecanica___API.Models
{
    public class OrdemServico
    {
        public int Id { get; set; }
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }

        public DateTime DataAbertura { get; set; }
        public DateTime? DataConclusao { get; set; }
        public string DescricaoServico { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = "Aberta";
    }
}
