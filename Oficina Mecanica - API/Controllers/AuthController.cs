using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Data;
using Oficina_Mecanica___API.DTOs;
using Oficina_Mecanica___API.Models;
using Oficina_Mecanica___API.Services;

namespace Oficina_Mecanica___API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly OficinaDbContext _context;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(OficinaDbContext context, TokenService tokenService, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var email = login.Email.Trim().ToLower();
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

            // Mesma mensagem para e-mail inexistente e senha errada, para não revelar quais e-mails existem
            if (usuario == null)
                return Unauthorized("E-mail ou senha inválidos.");

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, login.Senha);
            if (resultado == PasswordVerificationResult.Failed)
                return Unauthorized("E-mail ou senha inválidos.");

            var (token, expiracao) = _tokenService.GerarToken(usuario);

            return Ok(new LoginResponse
            {
                Token = token,
                Expiracao = expiracao,
                Usuario = usuario
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            // O id vem do próprio token, na claim NameIdentifier
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }
    }
}
