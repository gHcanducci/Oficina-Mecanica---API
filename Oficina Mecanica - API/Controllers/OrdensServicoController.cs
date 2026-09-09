using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Oficina_Mecanica___API.Data;
using Oficina_Mecanica___API.Models;

namespace Oficina_Mecanica___API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdensServicoController : ControllerBase
    {
        private readonly Data.OficinaDbContext _context;

        public OrdensServicoController(Data.OficinaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _context.OrdensServico.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ordem = await _context.OrdensServico.FindAsync(id);
            if (ordem == null)
                return NotFound();

            return Ok(ordem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrdemServico ordem)
        {
            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == ordem.VeiculoId);
            if (!veiculoExiste)
                return BadRequest("Veículo informado não existe.");

            var jaTemEmAndamento = await _context.OrdensServico
                .AnyAsync(o => o.VeiculoId == ordem.VeiculoId && o.Status == "Em andamento");
            if (jaTemEmAndamento)
                return BadRequest("Este veículo já possui uma ordem de serviço em andamento.");

            ordem.DataAbertura = DateTime.Now;
            ordem.Status = "Aberta";

            _context.OrdensServico.Add(ordem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ordem.Id }, ordem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrdemServico ordemAtualizada)
        {
            var ordem = await _context.OrdensServico.FindAsync(id);
            if (ordem == null)
                return NotFound();

            ordem.DescricaoServico = ordemAtualizada.DescricaoServico;
            ordem.ValorTotal = ordemAtualizada.ValorTotal;
            ordem.Status = ordemAtualizada.Status;

            if (ordem.Status == "Concluída")
                ordem.DataConclusao = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(ordem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ordem = await _context.OrdensServico.FindAsync(id);
            if (ordem == null)
                return NotFound();

            _context.OrdensServico.Remove(ordem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
