namespace Oficina_Mecanica___API.Models
{
    public static class StatusOrdemServico
    {
        public const string Aberta = "Aberta";
        public const string EmAndamento = "Em andamento";
        public const string Concluida = "Concluída";

        public static readonly string[] Todos = { Aberta, EmAndamento, Concluida };

        // Devolve o status com a grafia padrão ou null quando o valor não é um status válido
        public static string? Normalizar(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            return Todos.FirstOrDefault(s => s.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
