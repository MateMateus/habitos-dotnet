// Habito.cs — Define o modelo de dados (representa a tabela no banco)

namespace HabitosAPI.Models;

// Esta classe representa um "Hábito" no sistema.
// O Entity Framework Core vai ler esta classe e criar
// automaticamente uma tabela chamada "Habitos" no SQLite.
public class Habito
{
    // Chave primária — EF reconhece "Id" automaticamente
    // O banco vai gerar esse número sozinho (auto-increment)
    public int Id { get; set; }

    // Nome do hábito — obrigatório
    // Ex: "Beber água", "Estudar 1h", "Fazer exercício"
    public string Nome { get; set; } = string.Empty;

    // Descrição opcional para mais detalhes
    public string? Descricao { get; set; }

    // Ícone do hábito (emoji ou texto curto)
    // Ex: "💧", "📚", "🏋️"
    public string Icone { get; set; } = "✅";

    // Status: true = concluído hoje, false = pendente
    public bool Concluido { get; set; } = false;

    // Data de criação — preenchida automaticamente
    public DateTime CriadoEm { get; set; } = DateTime.Now;

    // Categoria do hábito para filtro
    // Ex: "Saúde", "Estudos", "Bem-estar"
    public string Categoria { get; set; } = "Geral";
}