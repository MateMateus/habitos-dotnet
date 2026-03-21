// Program.cs — Ponto de entrada da API. Configura e inicia tudo.

using Microsoft.EntityFrameworkCore;
using HabitosAPI.Data;

var builder = WebApplication.CreateBuilder(args);
// "builder" é o construtor da aplicação.
// Aqui você registra todos os serviços que a app vai usar.

// ─── SERVIÇOS ────────────────────────────────────────────────────────────────

// Registra os Controllers (nossas rotas da API)
builder.Services.AddControllers();

// Registra o banco de dados com SQLite
// Lê a string de conexão do appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// "DefaultConnection" é o nome que demos lá no appsettings.json
// UseSqlite diz: "use o SQLite como banco de dados"

// Habilita CORS para o front conseguir chamar a API
// CORS = Cross-Origin Resource Sharing
// Como front e API rodam em portas diferentes, precisamos liberar isso
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        policy.WithOrigins("http://localhost:5100") // porta do front
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ─── BUILD ───────────────────────────────────────────────────────────────────

var app = builder.Build();
// "app" é a aplicação construída e pronta para receber configurações de pipeline

// ─── MIDDLEWARE (pipeline de requisições) ────────────────────────────────────

app.UseCors("AllowFront");
// Aplica a política de CORS que definimos acima

app.UseAuthorization();
// Middleware de autorização (não usamos autenticação, mas é boa prática manter)

app.MapControllers();
// Mapeia automaticamente todos os Controllers e suas rotas

// ─── INICIALIZAÇÃO DO BANCO ──────────────────────────────────────────────────

// Cria e atualiza o banco automaticamente ao iniciar a API
// Isso evita precisar rodar "dotnet ef database update" manualmente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Aplica as migrations pendentes
}

app.Run();
// Inicia o servidor e fica escutando requisições