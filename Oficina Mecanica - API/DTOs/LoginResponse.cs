using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracao { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
