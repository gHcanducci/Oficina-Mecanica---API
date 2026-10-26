namespace Oficina_Mecanica___API.Models
{
    public static class PerfilUsuario
    {
        public const string Administrador = "Administrador";
        public const string Atendente = "Atendente";

        public static readonly string[] Todos = { Administrador, Atendente };

        public static string? Normalizar(string? perfil)
        {
            if (string.IsNullOrWhiteSpace(perfil))
                return null;

            return Todos.FirstOrDefault(p => p.Equals(perfil.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
