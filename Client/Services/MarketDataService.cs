using System.ComponentModel;

namespace TradingDashboard.Client.Services;

/// <summary>
/// Service pour simuler les données des marchés financiers
/// À terme, ce service appellera une vraie API financière
/// </summary>
public class MarketDataService
{
    private readonly Random _random = new();
    private readonly Timer _timer;
    private readonly Dictionary<string, MarketData> _marketData = new();
    private readonly Dictionary<string, List<ChartPoint>> _historicalData = new();

    public event Action? DataUpdated;

    public MarketDataService()
    {
        InitializeMarketData();
        GenerateHistoricalData();
        
        // Mise à jour des données toutes les 3 secondes
        _timer = new Timer(UpdatePrices, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
    }

    /// <summary>
    /// Initialise les données des marchés avec des valeurs de base
    /// </summary>
    private void InitializeMarketData()
    {
        // Indices actions
        _marketData["CAC40"] = new MarketData
        {
            Symbol = "CAC40",
            Name = "CAC 40",
            Price = 7854.32m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Indices,
            Currency = "EUR"
        };

        _marketData["SP500"] = new MarketData
        {
            Symbol = "SP500",
            Name = "S&P 500",
            Price = 4789.45m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Indices,
            Currency = "USD"
        };

        _marketData["NASDAQ"] = new MarketData
        {
            Symbol = "NASDAQ",
            Name = "NASDAQ 100",
            Price = 16234.78m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Indices,
            Currency = "USD"
        };

        // Cryptomonnaies
        _marketData["BTC"] = new MarketData
        {
            Symbol = "BTC",
            Name = "Bitcoin",
            Price = 43250.00m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Crypto,
            Currency = "USD"
        };

        _marketData["ETH"] = new MarketData
        {
            Symbol = "ETH",
            Name = "Ethereum",
            Price = 2580.45m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Crypto,
            Currency = "USD"
        };

        // Matières premières
        _marketData["GOLD"] = new MarketData
        {
            Symbol = "GOLD",
            Name = "Or",
            Price = 2045.67m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Commodities,
            Currency = "USD"
        };

        // Obligations
        _marketData["US10Y"] = new MarketData
        {
            Symbol = "US10Y",
            Name = "US Treasury 10Y",
            Price = 4.35m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Bonds,
            Currency = "%"
        };

        _marketData["FR10Y"] = new MarketData
        {
            Symbol = "FR10Y",
            Name = "France 10Y",
            Price = 2.89m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Bonds,
            Currency = "%"
        };

        _marketData["DE10Y"] = new MarketData
        {
            Symbol = "DE10Y",
            Name = "Allemagne 10Y",
            Price = 2.45m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Bonds,
            Currency = "%"
        };

        _marketData["EU10Y"] = new MarketData
        {
            Symbol = "EU10Y",
            Name = "UE 10Y",
            Price = 2.67m,
            Change = 0.0m,
            ChangePercent = 0.0m,
            Category = MarketCategory.Bonds,
            Currency = "%"
        };
    }

    /// <summary>
    /// Génère des données historiques simulées sur 7 jours
    /// </summary>
    private void GenerateHistoricalData()
    {
        var startDate = DateTime.Now.AddDays(-7);
        
        foreach (var kvp in _marketData)
        {
            var symbol = kvp.Key;
            var basePrice = kvp.Value.Price;
            var dataPoints = new List<ChartPoint>();

            for (int i = 0; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                var variation = (_random.NextDouble() - 0.5) * 0.05; // Variation de ±5%
                var price = basePrice * (1 + (decimal)variation);
                
                dataPoints.Add(new ChartPoint
                {
                    Date = date,
                    Value = price
                });
            }

            _historicalData[symbol] = dataPoints;
        }
    }

    /// <summary>
    /// Met à jour les prix de façon aléatoire
    /// </summary>
    private void UpdatePrices(object? state)
    {
        foreach (var kvp in _marketData.ToList())
        {
            var symbol = kvp.Key;
            var data = kvp.Value;
            var oldPrice = data.Price;

            // Génération d'une variation aléatoire
            var maxVariation = symbol switch
            {
                "BTC" or "ETH" => 0.03, // Crypto plus volatiles
                "GOLD" => 0.01, // Or moins volatil
                _ when data.Category == MarketCategory.Bonds => 0.005, // Obligations très stables
                _ => 0.015 // Indices actions
            };

            var variation = (_random.NextDouble() - 0.5) * maxVariation;
            var newPrice = oldPrice * (1 + (decimal)variation);

            // Mise à jour des données
            data.Price = Math.Round(newPrice, 2);
            data.Change = data.Price - oldPrice;
            data.ChangePercent = Math.Round((data.Change / oldPrice) * 100, 2);
            data.LastUpdated = DateTime.Now;

            // Mise à jour de l'historique
            if (_historicalData.ContainsKey(symbol))
            {
                _historicalData[symbol].Add(new ChartPoint
                {
                    Date = DateTime.Now,
                    Value = data.Price
                });

                // Garder seulement les 50 derniers points
                if (_historicalData[symbol].Count > 50)
                {
                    _historicalData[symbol].RemoveAt(0);
                }
            }
        }

        DataUpdated?.Invoke();
    }

    /// <summary>
    /// Récupère toutes les données du marché
    /// </summary>
    public Dictionary<string, MarketData> GetAllMarketData()
    {
        return new Dictionary<string, MarketData>(_marketData);
    }

    /// <summary>
    /// Récupère les données d'un actif spécifique
    /// </summary>
    public MarketData? GetMarketData(string symbol)
    {
        return _marketData.TryGetValue(symbol, out var data) ? data : null;
    }

    /// <summary>
    /// Récupère les données par catégorie
    /// </summary>
    public Dictionary<string, MarketData> GetMarketDataByCategory(MarketCategory category)
    {
        return _marketData.Where(kvp => kvp.Value.Category == category)
                         .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Récupère les données historiques d'un actif
    /// </summary>
    public List<ChartPoint> GetHistoricalData(string symbol)
    {
        return _historicalData.TryGetValue(symbol, out var data) ? data : new List<ChartPoint>();
    }

    /// <summary>
    /// Récupère les données historiques de plusieurs actifs
    /// </summary>
    public Dictionary<string, List<ChartPoint>> GetHistoricalDataForSymbols(string[] symbols)
    {
        var result = new Dictionary<string, List<ChartPoint>>();
        
        foreach (var symbol in symbols)
        {
            if (_historicalData.TryGetValue(symbol, out var data))
            {
                result[symbol] = data;
            }
        }
        
        return result;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

/// <summary>
/// Modèle de données pour un actif financier
/// </summary>
public class MarketData
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change { get; set; }
    public decimal ChangePercent { get; set; }
    public MarketCategory Category { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.Now;
}

/// <summary>
/// Point de données pour les graphiques
/// </summary>
public class ChartPoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
}

/// <summary>
/// Catégories d'actifs financiers
/// </summary>
public enum MarketCategory
{
    Indices,
    Crypto,
    Commodities,
    Bonds
} 