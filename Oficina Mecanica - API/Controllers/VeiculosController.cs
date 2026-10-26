using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Data;
using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VeiculosController : ControllerBase
    {
        private readonly Data.OficinaDbContext _context;

        public VeiculosController(Data.OficinaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? marca, [FromQuery] int? clienteId, [FromQuery] string? placa)
        {
            var query = _context.Veiculos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(marca))
            {
                var marcaBusca = marca.Trim();
                query = query.Where(v => v.Marca == marcaBusca);
            }

            if (clienteId.HasValue)
                query = query.Where(v => v.ClienteId == clienteId.Value);

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var placaBusca = placa.Trim().ToUpper();
                query = query.Where(v => v.Placa == placaBusca);
            }

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound("Veículo não encontrado.");

            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Veiculo veiculo)
        {
            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == veiculo.ClienteId);
            if (!clienteExiste)
                return BadRequest("Cliente informado não existe.");

            if (veiculo.AnoFabricacao > DateTime.Now.Year + 1)
                return BadRequest("O ano de fabricação não pode ser maior que o ano seguinte ao atual.");

            veiculo.Placa = veiculo.Placa.ToUpper();

            var placaJaExiste = await _context.Veiculos.AnyAsync(v => v.Placa == veiculo.Placa);
            if (placaJaExiste)
                return BadRequest("Já existe um veículo cadastrado com esta placa.");

            veiculo.Id = 0;
            veiculo.Modelo = veiculo.Modelo.Trim();
            veiculo.Marca = veiculo.Marca.Trim();

            _context.Veiculos.Add(veiculo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, veiculo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Veiculo veiculoAtualizado)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound("Veículo não encontrado.");

            var clienteExiste = await _context.Clientes.AnyAsync(c => c.Id == veiculoAtualizado.ClienteId);
            if (!clienteExiste)
                return BadRequest("Cliente informado não existe.");

            if (veiculoAtualizado.AnoFabricacao > DateTime.Now.Year + 1)
                return BadRequest("O ano de fabricação não pode ser maior que o ano seguinte ao atual.");

            var placa = veiculoAtualizado.Placa.ToUpper();

            var placaJaExiste = await _context.Veiculos.AnyAsync(v => v.Placa == placa && v.Id != id);
            if (placaJaExiste)
                return BadRequest("Já existe outro veículo cadastrado com esta placa.");

            veiculo.ClienteId = veiculoAtualizado.ClienteId;
            veiculo.Placa = placa;
            veiculo.Modelo = veiculoAtualizado.Modelo.Trim();
            veiculo.Marca = veiculoAtualizado.Marca.Trim();
            veiculo.AnoFabricacao = veiculoAtualizado.AnoFabricacao;

            await _context.SaveChangesAsync();
            return Ok(veiculo);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = PerfilUsuario.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
                return NotFound("Veículo não encontrado.");

            var possuiOrdens = await _context.OrdensServico.AnyAsync(o => o.VeiculoId == id);
            if (possuiOrdens)
                return BadRequest("Não é possível excluir um veículo que possui ordens de serviço.");

            _context.Veiculos.Remove(veiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{id}/ordensservico")]
        public async Task<IActionResult> GetOrdensServico(int id)
        {
            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == id);
            if (!veiculoExiste)
                return NotFound("Veículo não encontrado.");

            var ordens = await _context.OrdensServico
                .Where(o => o.VeiculoId == id)
                .ToListAsync();

            return Ok(ordens);
        }
    }
}
