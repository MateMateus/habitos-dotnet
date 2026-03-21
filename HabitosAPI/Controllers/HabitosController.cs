// HabitosController.cs — Define todas as rotas (endpoints) da API

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HabitosAPI.Data;
using HabitosAPI.Models;

namespace HabitosAPI.Controllers;

// [ApiController] — diz que essa classe é um controller de API
// [Route("api/[controller]")] — define a rota base como /api/habitos
// "[controller]" é substituído automaticamente pelo nome da classe sem "Controller"
[ApiController]
[Route("api/[controller]")]
public class HabitosController : ControllerBase
{
    // Injeção de dependência: o ASP.NET Core entrega o DbContext pronto
    // Não precisamos criar manualmente: o framework faz isso por nós
    private readonly AppDbContext _context;

    public HabitosController(AppDbContext context)
    {
        _context = context;
    }

    // ─── GET /api/habitos ─────────────────────────────────────────────────────
    // Retorna todos os hábitos, com filtro opcional por categoria e status
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Habito>>> GetHabitos(
        [FromQuery] string? categoria,    // ?categoria=Saúde
        [FromQuery] bool? concluido)      // ?concluido=true
    {
        // Começa a query (ainda não vai ao banco)
        var query = _context.Habitos.AsQueryable();

        // Aplica filtro de categoria se informado
        if (!string.IsNullOrEmpty(categoria))
            query = query.Where(h => h.Categoria == categoria);

        // Aplica filtro de status se informado
        if (concluido.HasValue)
            query = query.Where(h => h.Concluido == concluido.Value);

        // Ordena por data de criação (mais recente primeiro)
        // e executa a query no banco
        var habitos = await query
            .OrderByDescending(h => h.CriadoEm)
            .ToListAsync();

        return Ok(habitos); // 200 OK + lista em JSON
    }

    // ─── GET /api/habitos/5 ───────────────────────────────────────────────────
    // Retorna um hábito específico pelo Id
    [HttpGet("{id}")]
    public async Task<ActionResult<Habito>> GetHabito(int id)
    {
        // FindAsync busca pelo Id (chave primária) — mais eficiente
        var habito = await _context.Habitos.FindAsync(id);

        // Se não encontrou, retorna 404 Not Found
        if (habito == null)
            return NotFound(new { mensagem = "Hábito não encontrado." });

        return Ok(habito); // 200 OK + objeto em JSON
    }

    // ─── POST /api/habitos ────────────────────────────────────────────────────
    // Cria um novo hábito
    [HttpPost]
    public async Task<ActionResult<Habito>> PostHabito(Habito habito)
    {
        // Validação básica: nome não pode ser vazio
        if (string.IsNullOrWhiteSpace(habito.Nome))
            return BadRequest(new { mensagem = "O nome do hábito é obrigatório." });

        // Define a data de criação agora
        habito.CriadoEm = DateTime.Now;

        // Adiciona ao contexto (ainda não salvou no banco)
        _context.Habitos.Add(habito);

        // Salva no banco de verdade (gera o INSERT SQL)
        await _context.SaveChangesAsync();

        // Retorna 201 Created + o objeto criado + a URL para acessá-lo
        return CreatedAtAction(nameof(GetHabito), new { id = habito.Id }, habito);
    }

    // ─── PUT /api/habitos/5 ───────────────────────────────────────────────────
    // Atualiza um hábito existente
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHabito(int id, Habito habito)
    {
        // O id da URL deve bater com o id do objeto enviado
        if (id != habito.Id)
            return BadRequest(new { mensagem = "Id inconsistente." });

        // Validação básica
        if (string.IsNullOrWhiteSpace(habito.Nome))
            return BadRequest(new { mensagem = "O nome do hábito é obrigatório." });

        // Marca o objeto como "modificado" no EF Core
        // Isso vai gerar um UPDATE no banco
        _context.Entry(habito).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync(); // Executa o UPDATE
        }
        catch (DbUpdateConcurrencyException)
        {
            // Se o registro não existe mais no banco
            if (!await _context.Habitos.AnyAsync(h => h.Id == id))
                return NotFound(new { mensagem = "Hábito não encontrado." });
            throw; // Relança se for outro erro
        }

        return NoContent(); // 204 No Content — atualização bem-sucedida
    }

    // ─── PATCH /api/habitos/5/toggle ─────────────────────────────────────────
    // Alterna o status concluído/pendente de um hábito
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleHabito(int id)
    {
        var habito = await _context.Habitos.FindAsync(id);

        if (habito == null)
            return NotFound(new { mensagem = "Hábito não encontrado." });

        // Inverte o status (true vira false, false vira true)
        habito.Concluido = !habito.Concluido;

        await _context.SaveChangesAsync();

        // Retorna o novo status para o front saber o que mostrar
        return Ok(new { concluido = habito.Concluido });
    }

    // ─── DELETE /api/habitos/5 ────────────────────────────────────────────────
    // Exclui um hábito
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHabito(int id)
    {
        var habito = await _context.Habitos.FindAsync(id);

        if (habito == null)
            return NotFound(new { mensagem = "Hábito não encontrado." });

        _context.Habitos.Remove(habito); // Marca para exclusão
        await _context.SaveChangesAsync(); // Executa o DELETE

        return NoContent(); // 204 — excluído com sucesso
    }

    // ─── GET /api/habitos/categorias ──────────────────────────────────────────
    // Retorna lista de categorias distintas (para o filtro)
    [HttpGet("categorias")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategorias()
    {
        var categorias = await _context.Habitos
            .Select(h => h.Categoria)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return Ok(categorias);
    }
}