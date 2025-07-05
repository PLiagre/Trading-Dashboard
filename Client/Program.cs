using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TradingDashboard.Client;
using TradingDashboard.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration du HttpClient pour les futurs appels API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Services métier
builder.Services.AddScoped<MarketDataService>();
builder.Services.AddScoped<ThemeService>();

await builder.Build().RunAsync();
