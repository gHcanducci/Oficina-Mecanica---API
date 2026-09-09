
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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Clientes.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Cliente clienteAtualizado)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            cliente.Nome = clienteAtualizado.Nome;
            cliente.CPF = clienteAtualizado.CPF;
            cliente.Telefone = clienteAtualizado.Telefone;
            cliente.Email = clienteAtualizado.Email;

            await _context.SaveChangesAsync();
            return Ok(cliente);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}