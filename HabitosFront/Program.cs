// Program.cs — Configura e inicia o frontend MVC

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte a Controllers + Views (padrão MVC)
builder.Services.AddControllersWithViews();

// Registra o HttpClient para chamar a API
// Definimos a URL base da API aqui
builder.Services.AddHttpClient("HabitosAPI", client =>
{
    // Endereço base da nossa API
    client.BaseAddress = new Uri("http://localhost:5000/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

// Configura tratamento de erros em produção
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

// Serve arquivos estáticos (CSS, JS locais se houver)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Define a rota padrão: controller=Habitos, action=Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Habitos}/{action=Index}/{id?}");

app.Run();  