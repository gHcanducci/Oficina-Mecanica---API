using System.Text.Json.Serialization;

namespace Oficina_Mecanica___API.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // A senha nunca é guardada nem devolvida em texto puro, só o hash
        [JsonIgnore]
        public string SenhaHash { get; set; } = string.Empty;

        public string Perfil { get; set; } = PerfilUsuario.Atendente;
    }
}
