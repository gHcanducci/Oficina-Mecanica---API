
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Data;
using Oficina_Mecanica___API.Models;


namespace Oficina_Mecanica___API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly Data.OficinaDbContext _context;

        public ClientesController(Data.OficinaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? nome, [FromQuery] string? cpf)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var nomeBusca = nome.Trim();
                query = query.Where(c => c.Nome.Contains(nomeBusca));
            }

            if (!string.IsNullOrWhiteSpace(cpf))
            {
                var cpfBusca = cpf.Trim();
                query = query.Where(c => c.CPF == cpfBusca);
            }

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            return Ok(cliente);
        }

        [HttpGet("{id}/veiculos")]
        public async Task<IActionResult> GetVeiculos(int id)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == id);
            if (!clienteExiste)
                return NotFound("Cliente não encontrado.");

            var veiculos = await _context.Veiculos
                .Where(v => v.ClienteId == id)
                .ToListAsync();

            return Ok(veiculos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cliente cliente)
        {
            var cpfJaExiste = await _context.Clientes.AnyAsync(c => c.CPF == cliente.CPF);
            if (cpfJaExiste)
                return BadRequest("Já existe um cliente cadastrado com este CPF.");

            cliente.Id = 0;
            cliente.Nome = cliente.Nome.Trim();
            cliente.Email = cliente.Email.Trim();

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cliente clienteAtualizado)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            var cpfJaExiste = await _context.Clientes
                .AnyAsync(c => c.CPF == clienteAtualizado.CPF && c.Id != id);
            if (cpfJaExiste)
                return BadRequest("Já existe outro cliente cadastrado com este CPF.");

            cliente.Nome = clienteAtualizado.Nome.Trim();
            cliente.CPF = clienteAtualizado.CPF;
            cliente.Telefone = clienteAtualizado.Telefone;
            cliente.Email = clienteAtualizado.Email.Trim();

            await _context.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound("Cliente não encontrado.");

            var possuiVeiculos = await _context.Veiculos.AnyAsync(v => v.ClienteId == id);
            if (possuiVeiculos)
                return BadRequest("Não é possível excluir um cliente que possui veículos cadastrados.");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
