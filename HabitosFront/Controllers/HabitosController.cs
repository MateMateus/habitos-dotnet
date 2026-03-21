// HabitosController.cs (Front) — Chama a API e passa dados para as Views

using Microsoft.AspNetCore.Mvc;
using HabitosFront.Models;
using System.Text;
using System.Text.Json;

namespace HabitosFront.Controllers;

public class HabitosController : Controller
{
    // IHttpClientFactory cria HttpClients configurados (registrado no Program.cs)
    private readonly IHttpClientFactory _httpClientFactory;

    // Opções para deserializar JSON ignorando maiúsculas/minúsculas
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public HabitosController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // Cria um HttpClient nomeado (configurado no Program.cs)
    private HttpClient CriarCliente() => _httpClientFactory.CreateClient("HabitosAPI");

    // ─── INDEX — Tela principal com listagem e filtros ────────────────────────
    public async Task<IActionResult> Index(string? categoria, bool? concluido)
    {
        var client = CriarCliente();

        // Monta a URL com os filtros (query string)
        var url = "api/habitos";
        var parametros = new List<string>();
        if (!string.IsNullOrEmpty(categoria)) parametros.Add($"categoria={categoria}");
        if (concluido.HasValue) parametros.Add($"concluido={concluido.Value}");
        if (parametros.Any()) url += "?" + string.Join("&", parametros);

        // Chama a API
        var response = await client.GetAsync(url);
        var habitos = new List<HabitoViewModel>();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            habitos = JsonSerializer.Deserialize<List<HabitoViewModel>>(json, _jsonOptions)
                      ?? new List<HabitoViewModel>();
        }

        // Busca categorias para o dropdown de filtro
        var catResponse = await client.GetAsync("api/habitos/categorias");
        var categorias = new List<string>();
        if (catResponse.IsSuccessStatusCode)
        {
            var json = await catResponse.Content.ReadAsStringAsync();
            categorias = JsonSerializer.Deserialize<List<string>>(json, _jsonOptions)
                         ?? new List<string>();
        }

        // ViewBag = forma simples de passar dados extras para a View
        ViewBag.Categorias = categorias;
        ViewBag.CategoriaAtual = categoria;
        ViewBag.ConcluidoAtual = concluido;

        // TempData é usado para mostrar mensagens após redirect
        // (ex: "Hábito criado com sucesso!")

        return View(habitos); // Passa a lista para a View
    }

    // ─── CREATE (GET) — Exibe o formulário de criação ─────────────────────────
    public IActionResult Create()
    {
        return View(new HabitoViewModel());
    }

    // ─── CREATE (POST) — Envia o formulário para a API ────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken] // Proteção contra CSRF
    public async Task<IActionResult> Create(HabitoViewModel habito)
    {
        // ModelState.IsValid verifica as anotações [Required], [StringLength], etc.
        if (!ModelState.IsValid)
            return View(habito);

        var client = CriarCliente();

        // Serializa o objeto para JSON
        var json = JsonSerializer.Serialize(habito);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Envia POST para a API
        var response = await client.PostAsync("api/habitos", content);

        if (response.IsSuccessStatusCode)
        {
            // TempData["Sucesso"] fica disponível na próxima requisição
            TempData["Sucesso"] = "Hábito criado com sucesso! 🎉";
            return RedirectToAction(nameof(Index));
        }

        // Se a API retornou erro, mostra mensagem
        TempData["Erro"] = "Erro ao criar hábito. Tente novamente.";
        return View(habito);
    }

    // ─── EDIT (GET) — Carrega o hábito para edição ────────────────────────────
    public async Task<IActionResult> Edit(int id)
    {
        var client = CriarCliente();
        var response = await client.GetAsync($"api/habitos/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Erro"] = "Hábito não encontrado.";
            return RedirectToAction(nameof(Index));
        }

        var json = await response.Content.ReadAsStringAsync();
        var habito = JsonSerializer.Deserialize<HabitoViewModel>(json, _jsonOptions);

        return View(habito);
    }

    // ─── EDIT (POST) — Envia edição para a API ────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, HabitoViewModel habito)
    {
        if (!ModelState.IsValid)
            return View(habito);

        var client = CriarCliente();
        var json = JsonSerializer.Serialize(habito);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync($"api/habitos/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Sucesso"] = "Hábito atualizado com sucesso! ✏️";
            return RedirectToAction(nameof(Index));
        }

        TempData["Erro"] = "Erro ao atualizar hábito.";
        return View(habito);
    }

    // ─── DELETE (POST) — Exclui via formulário (botão) ────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var client = CriarCliente();
        var response = await client.DeleteAsync($"api/habitos/{id}");

        if (response.IsSuccessStatusCode)
            TempData["Sucesso"] = "Hábito excluído. 🗑️";
        else
            TempData["Erro"] = "Erro ao excluir hábito.";

        return RedirectToAction(nameof(Index));
    }

    // ─── TOGGLE — Alterna concluído/pendente via AJAX ─────────────────────────
    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var client = CriarCliente();
        var response = await client.PatchAsync($"api/habitos/{id}/toggle", null);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return Content(json, "application/json"); // Retorna JSON para o JS da página
        }

        return BadRequest();
    }
}