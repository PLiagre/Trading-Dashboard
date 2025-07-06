using System.Text.Json;

namespace TradingDashboard.Services;

/// <summary>
/// Service de gestion des données de marché avec intégration Supabase
/// </summary>
public class MarketDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<MarketDataService> _logger;

    public MarketDataService(IHttpClientFactory httpClientFactory, ILogger<MarketDataService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Récupère les données d'aperçu du marché depuis Supabase
    /// </summary>
    public async Task<MarketOverviewData> GetMarketOverviewAsync()
    {
        try
        {
            // Utilisation du client HTTP configuré pour Supabase
            var httpClient = _httpClientFactory.CreateClient("supabase");
            
            // Appel vers la table market_overview (exemple)
            var response = await httpClient.GetAsync("/rest/v1/market_overview?select=*");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<MarketDataPoint>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                
                return new MarketOverviewData
                {
                    LastUpdate = DateTime.UtcNow,
                    DataPoints = data ?? new List<MarketDataPoint>(),
                    IsConnected = true
                };
            }
            else
            {
                _logger.LogWarning("Erreur lors de l'appel Supabase: {StatusCode}", response.StatusCode);
                return GetFallbackData();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des données de marché");
            return GetFallbackData();
        }
    }

    /// <summary>
    /// Récupère les données détaillées d'un instrument depuis Supabase
    /// </summary>
    public async Task<InstrumentData?> GetInstrumentDataAsync(string symbol)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("supabase");
            
            // Appel avec filtre sur le symbole
            var response = await httpClient.GetAsync($"/rest/v1/instruments?symbol=eq.{symbol}&select=*");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var instruments = JsonSerializer.Deserialize<List<InstrumentData>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });
                
                return instruments?.FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'instrument {Symbol}", symbol);
        }
        
        return null;
    }

    /// <summary>
    /// Sauvegarde des données de marché vers Supabase
    /// </summary>
    public async Task<bool> SaveMarketDataAsync(MarketDataPoint dataPoint)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("supabase");
            
            // Configuration des headers pour l'insertion
            httpClient.DefaultRequestHeaders.Add("Prefer", "return=minimal");
            
            var jsonContent = JsonSerializer.Serialize(dataPoint, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });
            
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/rest/v1/market_data", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la sauvegarde des données de marché");
            return false;
        }
    }

    /// <summary>
    /// Données de fallback en cas d'erreur de connexion
    /// </summary>
    private static MarketOverviewData GetFallbackData()
    {
        var random = new Random();
        return new MarketOverviewData
        {
            LastUpdate = DateTime.UtcNow,
            IsConnected = false,
            DataPoints = new List<MarketDataPoint>
            {
                new() { Symbol = "BTC/EUR", Price = 42000m + (decimal)(random.NextDouble() * 2000), Timestamp = DateTime.UtcNow },
                new() { Symbol = "ETH/EUR", Price = 2800m + (decimal)(random.NextDouble() * 200), Timestamp = DateTime.UtcNow },
                new() { Symbol = "EUR/USD", Price = 1.09m + (decimal)(random.NextDouble() * 0.02), Timestamp = DateTime.UtcNow },
                new() { Symbol = "AAPL", Price = 190m + (decimal)(random.NextDouble() * 20), Timestamp = DateTime.UtcNow },
                new() { Symbol = "GOOGL", Price = 2700m + (decimal)(random.NextDouble() * 100), Timestamp = DateTime.UtcNow }
            }
        };
    }
}

// Modèles de données
public class MarketOverviewData
{
    public DateTime LastUpdate { get; set; }
    public bool IsConnected { get; set; }
    public List<MarketDataPoint> DataPoints { get; set; } = new();
}

public class MarketDataPoint
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal? Change24h { get; set; }
    public decimal? Volume { get; set; }
}

public class InstrumentData
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public decimal Volume { get; set; }
    public DateTime LastUpdate { get; set; }
} 