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
    public class OrdensServicoController : ControllerBase
    {
        private readonly Data.OficinaDbContext _context;

        public OrdensServicoController(Data.OficinaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? status,
            [FromQuery] int? veiculoId,
            [FromQuery] DateTime? dataInicial,
            [FromQuery] DateTime? dataFinal)
        {
            var query = _context.OrdensServico.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusBusca = StatusOrdemServico.Normalizar(status);
                if (statusBusca == null)
                    return BadRequest("Status inválido. Use: Aberta, Em andamento ou Concluída.");

                query = query.Where(o => o.Status == statusBusca);
            }

            if (veiculoId.HasValue)
                query = query.Where(o => o.VeiculoId == veiculoId.Value);

            if (dataInicial.HasValue && dataFinal.HasValue && dataInicial.Value.Date > dataFinal.Value.Date)
                return BadRequest("A data inicial não pode ser maior que a data final.");

            if (dataInicial.HasValue)
            {
                var inicio = dataInicial.Value.Date;
                query = query.Where(o => o.DataAbertura >= inicio);
            }

            if (dataFinal.HasValue)
            {
                // Inclui o dia final inteiro
                var fim = dataFinal.Value.Date.AddDays(1);
                query = query.Where(o => o.DataAbertura < fim);
            }

            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ordem = await _context.OrdensServico.FindAsync(id);
            if (ordem == null)
                return NotFound("Ordem de serviço não encontrada.");

            return Ok(ordem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrdemServico ordem)
        {
            var veiculoExiste = await _context.Veiculos.AnyAsync(v => v.Id == ordem.VeiculoId);
            if (!veiculoExiste)
                return BadRequest("Veículo informado não existe.");

            // Toda ordem de serviço nasce aberta; o status só muda pelo PUT
            ordem.Id = 0;
            ordem.DataAbertura = DateTime.Now;
            ordem.DataConclusao = null;
            ordem.Status = StatusOrdemServico.Aberta;
            ordem.DescricaoServico = ordem.DescricaoServico.Trim();

            _context.OrdensServico.Add(ordem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = ordem.Id }, ordem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrdemServico ordemAtualizada)
        {
            var ordem = await _context.OrdensServico.FindAsync(id);
            if (ordem == null)
                return NotFound("Ordem de serviço não encontrada.");

            if (ordem.Status == StatusOrdemServico.Concluida)
                return BadRequest("Uma ordem de serviço concluída não pode ser alterada.");

            var novoStatus = StatusOrdemServico.Normalizar(ordemAtualizada.Status);
            if (novoStatus == null)
                return BadRequest("Status inválido. Use: Aberta, Em andamento ou Concluída.");

            if (novoStatus == StatusOrdemServico.EmAndamento)
            {
                var jaTemEmAndamento = await _context.OrdensServico
                    .AnyAsync(o => o.VeiculoId == ordem.VeiculoId
                        && o.Status == StatusOrdemServico.EmAndamento
                        && o.Id != id);
                if (jaTemEmAndamento)
                    return BadRequest("Este veículo já possui uma ordem de serviço em andamento.");
            }

            if (novoStatus == StatusOrdemServico.Concluida && ordemAtualizada.ValorTotal <= 0)
                return BadRequest("Para concluir a ordem de serviço é necessário informar um valor total maior que zero.");

            ordem.DescricaoServico = ordemAtualizada.DescricaoServico.Trim();
            ordem.ValorTotal = ordemAtualizada.ValorTotal;
            ordem.Status = novoStatus;

            if (ordem.Status == StatusOrdemServico.Concluida)
                ordem.DataConclusao = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(ordem);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = PerfilUsuario.Administrador)]
        public async Task<IActionResult> Delete(int id)
        {
            var ordem = await _context.OrdensServico.FindAsync(id);
            if (ordem == null)
                return NotFound("Ordem de serviço não encontrada.");

            if (ordem.Status != StatusOrdemServico.Aberta)
                return BadRequest("Só é possível excluir ordens de serviço com status Aberta.");

            _context.OrdensServico.Remove(ordem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
