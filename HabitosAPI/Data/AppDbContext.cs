// AppDbContext.cs — A "ponte" entre o C# e o banco de dados

using Microsoft.EntityFrameworkCore;
using HabitosAPI.Models;

namespace HabitosAPI.Data;

// DbContext é a classe base do Entity Framework Core.
// Ela representa a sessão com o banco de dados.
// Pense nela como o "gerente" que conversa com o SQLite.
public class AppDbContext : DbContext
{
    // Construtor: recebe as opções de configuração (string de conexão, etc.)
    // O ASP.NET Core passa essas opções automaticamente via injeção de dependência
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSet representa uma tabela no banco.
    // "Habitos" vai ser o nome da tabela no SQLite.
    // Você pode fazer: _context.Habitos.ToList() para buscar todos os registros.
    public DbSet<Habito> Habitos { get; set; }
}