using TradingDashboard.Components;
using TradingDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services pour Blazor Web App hybride (.NET 8)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()    // Support du render mode Server
    .AddInteractiveWebAssemblyComponents(); // Support du render mode WebAssembly

// Configuration HttpClient pour appels API Supabase
builder.Services.AddHttpClient("supabase", client =>
{
    // URL de base Supabase (à configurer via appsettings.json en production)
    client.BaseAddress = new Uri(builder.Configuration["Supabase:Url"] ?? "https://your-project.supabase.co");
    client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:AnonKey"] ?? "your-anon-key");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Supabase:AnonKey"] ?? "your-anon-key"}");
});

// HttpClient générique pour autres appels
builder.Services.AddHttpClient();

// Services métier (réutilisés depuis l'ancien projet)
builder.Services.AddScoped<MarketDataService>();
builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<LocalizationService>();

// Support de la localisation
builder.Services.AddLocalization();

var app = builder.Build();

// Configuration du pipeline de requêtes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Configuration du routage pour Blazor hybride
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()    // Active le render mode Server
    .AddInteractiveWebAssemblyRenderMode(); // Active le render mode WebAssembly

app.Run(); 