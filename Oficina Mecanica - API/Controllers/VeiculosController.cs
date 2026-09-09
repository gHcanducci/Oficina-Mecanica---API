using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Data;
using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly Data.OficinaDbContext _context;

        public VeiculosController(Data.OficinaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.Veiculos.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound();

            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Veiculo veiculo)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == veiculo.ClienteId);
            if (!clienteExiste)
                return BadRequest("Cliente informado não existe.");

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Veiculo veiculoAtualizado)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound();

            veiculo.Placa = veiculoAtualizado.Placa;
            veiculo.Modelo = veiculoAtualizado.Modelo;
            veiculo.Marca = veiculoAtualizado.Marca;
            veiculo.AnoFabricacao = veiculoAtualizado.AnoFabricacao;

            await _context.SaveChangesAsync();
            return Ok(veiculo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound();

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/ordensservico")]
        public async Task<IActionResult> GetOrdensServico(int id)
        {
            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == id);
            if (!veiculoExiste)
                return NotFound();

            var ordens = await _context.OrdensServico
                .Where(o => o.VeiculoId == id)
                .ToListAsync();

            return Ok(ordens);
        }
    }
}
