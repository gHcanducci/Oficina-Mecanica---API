using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (string Token, DateTime Expiracao) GerarToken(Usuario usuario)
        {
            var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
            var expiracao = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:ExpiracaoMinutos"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiracao,
                signingCredentials: credenciais);

            return (new JwtSecurityTokenHandler().WriteToken(token), expiracao);
        }
    }
}
