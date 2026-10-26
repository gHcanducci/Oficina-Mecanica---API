using System.ComponentModel.DataAnnotations;

namespace Oficina_Mecanica___API.DTOs
{
    public class UsuarioRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        [StringLength(100, ErrorMessage = "O e-mail deve ter no máximo 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres.")]
        public string? Senha { get; set; }

        [Required(ErrorMessage = "O perfil é obrigatório.")]
        public string Perfil { get; set; } = string.Empty;
    }
}
