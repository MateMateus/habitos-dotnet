// HabitoViewModel.cs — Modelo que o frontend usa para conversar com a API
// ViewModel é o modelo "da View" — pode ter campos extras para exibição

using System.ComponentModel.DataAnnotations;

namespace HabitosFront.Models;

public class HabitoViewModel
{
    public int Id { get; set; }

    // [Required] — validação no frontend: campo obrigatório
    // [Display] — nome amigável exibido no formulário
    [Required(ErrorMessage = "O nome é obrigatório")]
    [Display(Name = "Nome do Hábito")]
    [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Display(Name = "Ícone (emoji)")]
    public string Icone { get; set; } = "✅";

    [Display(Name = "Concluído")]
    public bool Concluido { get; set; }

    public DateTime CriadoEm { get; set; }

    [Display(Name = "Categoria")]
    public string Categoria { get; set; } = "Geral";
}