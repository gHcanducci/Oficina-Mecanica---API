namespace Oficina_Mecanica___API.Models
{
    public class Veiculo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }   

        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Marca { get; set; }
        public int AnoFabricacao { get; set; }

        public List<OrdemServico>? OrdensServico { get; set; } = new List<OrdemServico>();
    }
}
