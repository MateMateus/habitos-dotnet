// Program.cs — Ponto de entrada da API
// ESTRATÉGIA: SQLite local para desenvolvimento, PostgreSQL na nuvem (Railway)

using Microsoft.EntityFrameworkCore;
using HabitosAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ─── BANCO DE DADOS ───────────────────────────────────────────────────────────
// Detecta automaticamente se está rodando na nuvem ou localmente:
// - Se existir a variável DATABASE_URL (Railway injeta isso), usa PostgreSQL
// - Se não existir, usa SQLite local (desenvolvimento na sua máquina)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // ✅ PRODUÇÃO (Railway): usa PostgreSQL
    // DATABASE_URL é injetada automaticamente pelo Railway
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(databaseUrl));
}
else
{
    // ✅ DESENVOLVIMENTO (sua máquina): usa SQLite
    // Não precisa instalar nada — é só um arquivo .db
    var connectionString = builder.Configuration
                               .GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString));
}

// ─── CORS ─────────────────────────────────────────────────────────────────────
// Libera o front para chamar a API (portas diferentes = precisa de CORS)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Lê a porta do ambiente (Railway injeta a variável PORT automaticamente)
// Se não existir, usa 5000 para desenvolvimento local
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://+:{port}");

// ─── BUILD ────────────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseCors("AllowFront");
app.UseAuthorization();
app.MapControllers();

// ─── CRIA O BANCO AUTOMATICAMENTE AO INICIAR ──────────────────────────────────
// Aplica migrations pendentes sem precisar rodar comandos manuais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();