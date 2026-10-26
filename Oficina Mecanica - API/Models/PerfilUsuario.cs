namespace Oficina_Mecanica___API.Models
{
    public static class PerfilUsuario
    {
        public const string Administrador = "Administrador";
        public const string Atendente = "Atendente";

        public static readonly string[] Todos = { Administrador, Atendente };

        // Devolve o perfil com a grafia padrão ou null quando o valor não é um perfil válido
        public static string? Normalizar(string? perfil)
        {
            if (string.IsNullOrWhiteSpace(perfil))
                return null;

            return Todos.FirstOrDefault(p => p.Equals(perfil.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
