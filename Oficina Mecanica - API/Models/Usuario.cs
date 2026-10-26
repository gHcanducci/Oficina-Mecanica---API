using System.Text.Json.Serialization;

namespace Oficina_Mecanica___API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string SenhaHash { get; set; } = string.Empty;

        public string Perfil { get; set; } = PerfilUsuario.Atendente;
    }
}
