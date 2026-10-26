using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Data;
using Oficina_Mecanica___API.DTOs;
using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = PerfilUsuario.Administrador)]
    public class UsuariosController : ControllerBase
    {
        private readonly OficinaDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(OficinaDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? nome, [FromQuery] string? perfil)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var nomeBusca = nome.Trim();
                query = query.Where(u => u.Nome.Contains(nomeBusca));
            }

            if (!string.IsNullOrWhiteSpace(perfil))
            {
                var perfilBusca = PerfilUsuario.Normalizar(perfil);
                if (perfilBusca == null)
                    return BadRequest("Perfil inválido. Use: Administrador ou Atendente.");

                query = query.Where(u => u.Perfil == perfilBusca);
            }

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest("A senha é obrigatória.");

            var perfil = PerfilUsuario.Normalizar(request.Perfil);
            if (perfil == null)
                return BadRequest("Perfil inválido. Use: Administrador ou Atendente.");

            var email = request.Email.Trim().ToLower();
            var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == email);
            if (emailJaExiste)
                return BadRequest("Já existe um usuário cadastrado com este e-mail.");

            var usuario = new Usuario
            {
                Nome = request.Nome.Trim(),
                Email = email,
                Perfil = perfil
            };
            usuario.SenhaHash = _passwordHasher.HashPassword(usuario, request.Senha);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var perfil = PerfilUsuario.Normalizar(request.Perfil);
            if (perfil == null)
                return BadRequest("Perfil inválido. Use: Administrador ou Atendente.");

            var email = request.Email.Trim().ToLower();
            var emailJaExiste = await _context.Usuarios.AnyAsync(u => u.Email == email && u.Id != id);
            if (emailJaExiste)
                return BadRequest("Já existe outro usuário cadastrado com este e-mail.");

            if (usuario.Perfil == PerfilUsuario.Administrador && perfil != PerfilUsuario.Administrador)
            {
                var outrosAdministradores = await _context.Usuarios
                    .AnyAsync(u => u.Perfil == PerfilUsuario.Administrador && u.Id != id);
                if (!outrosAdministradores)
                    return BadRequest("Não é possível remover o perfil do único administrador do sistema.");
            }

            usuario.Nome = request.Nome.Trim();
            usuario.Email = email;
            usuario.Perfil = perfil;

            if (!string.IsNullOrWhiteSpace(request.Senha))
                usuario.SenhaHash = _passwordHasher.HashPassword(usuario, request.Senha);

            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var idUsuarioLogado = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (usuario.Id == idUsuarioLogado)
                return BadRequest("Não é possível excluir o próprio usuário.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
