// Program.cs — Configura e inicia o frontend MVC
// MUDANÇA: a URL da API agora pode vir de variável de ambiente

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registra o HttpClient que chama a API
// Lemos a URL base da API de uma variável de ambiente
// Se não existir, usa localhost (para desenvolvimento local)
builder.Services.AddHttpClient("HabitosAPI", client =>
{
    // Na nuvem (Railway), defina a variável API_BASE_URL com a URL da sua API
    // Localmente, usa http://localhost:5000/
    var apiUrl = Environment.GetEnvironmentVariable("API_BASE_URL")
                 ?? "http://localhost:5000/";

    client.BaseAddress = new Uri(apiUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Habitos}/{action=Index}/{id?}");

app.Run();