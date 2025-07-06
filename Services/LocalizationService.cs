using Microsoft.JSInterop;
using System.Globalization;

namespace TradingDashboard.Services;

/// <summary>
/// Service de localisation pour la gestion des langues et formats
/// </summary>
public class LocalizationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<LocalizationService> _logger;
    private CultureInfo _currentCulture = new("fr-FR");

    public LocalizationService(IJSRuntime jsRuntime, ILogger<LocalizationService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <summary>
    /// Événement déclenché lors du changement de langue
    /// </summary>
    public event Action<CultureInfo>? CultureChanged;

    /// <summary>
    /// Initialise le service de localisation
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Récupération de la langue depuis localStorage ou navigateur
            var storedCulture = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "culture");
            
            if (!string.IsNullOrEmpty(storedCulture))
            {
                _currentCulture = new CultureInfo(storedCulture);
            }
            else
            {
                // Détection automatique de la langue du navigateur
                var browserLanguage = await _jsRuntime.InvokeAsync<string>("navigator.language");
                if (!string.IsNullOrEmpty(browserLanguage))
                {
                    try
                    {
                        _currentCulture = new CultureInfo(browserLanguage);
                    }
                    catch
                    {
                        // Fallback en français
                        _currentCulture = new CultureInfo("fr-FR");
                    }
                }
            }

            // Application de la culture
            CultureInfo.CurrentCulture = _currentCulture;
            CultureInfo.CurrentUICulture = _currentCulture;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'initialisation de la localisation");
        }
    }

    /// <summary>
    /// Récupère la culture actuelle
    /// </summary>
    public CultureInfo GetCurrentCulture()
    {
        return _currentCulture;
    }

    /// <summary>
    /// Définit la culture et la persiste
    /// </summary>
    public async Task SetCultureAsync(string cultureName)
    {
        try
        {
            var newCulture = new CultureInfo(cultureName);
            _currentCulture = newCulture;
            
            // Application de la nouvelle culture
            CultureInfo.CurrentCulture = newCulture;
            CultureInfo.CurrentUICulture = newCulture;
            
            // Sauvegarde en localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "culture", cultureName);
            
            // Notification du changement
            CultureChanged?.Invoke(_currentCulture);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la définition de la culture {Culture}", cultureName);
        }
    }

    /// <summary>
    /// Formate un nombre selon la culture actuelle
    /// </summary>
    public string FormatNumber(decimal number, int decimals = 2)
    {
        return number.ToString($"N{decimals}", _currentCulture);
    }

    /// <summary>
    /// Formate une devise selon la culture actuelle
    /// </summary>
    public string FormatCurrency(decimal amount, string? currencyCode = null)
    {
        if (!string.IsNullOrEmpty(currencyCode))
        {
            return amount.ToString("C", _currentCulture).Replace(_currentCulture.NumberFormat.CurrencySymbol, GetCurrencySymbol(currencyCode));
        }
        return amount.ToString("C", _currentCulture);
    }

    /// <summary>
    /// Formate un pourcentage selon la culture actuelle
    /// </summary>
    public string FormatPercentage(decimal percentage, int decimals = 2)
    {
        return (percentage / 100).ToString($"P{decimals}", _currentCulture);
    }

    /// <summary>
    /// Formate une date selon la culture actuelle
    /// </summary>
    public string FormatDate(DateTime date, string format = "d")
    {
        return date.ToString(format, _currentCulture);
    }

    /// <summary>
    /// Formate une date et heure selon la culture actuelle
    /// </summary>
    public string FormatDateTime(DateTime dateTime, string format = "g")
    {
        return dateTime.ToString(format, _currentCulture);
    }

    /// <summary>
    /// Récupère les traductions pour l'interface
    /// </summary>
    public Dictionary<string, string> GetTranslations()
    {
        return _currentCulture.Name switch
        {
            "en-US" => GetEnglishTranslations(),
            "de-DE" => GetGermanTranslations(),
            "es-ES" => GetSpanishTranslations(),
            _ => GetFrenchTranslations() // Français par défaut
        };
    }

    /// <summary>
    /// Récupère une traduction spécifique
    /// </summary>
    public string GetTranslation(string key)
    {
        var translations = GetTranslations();
        return translations.TryGetValue(key, out var translation) ? translation : key;
    }

    /// <summary>
    /// Récupère le symbole d'une devise
    /// </summary>
    private static string GetCurrencySymbol(string currencyCode)
    {
        return currencyCode.ToUpper() switch
        {
            "EUR" => "€",
            "USD" => "$",
            "GBP" => "£",
            "JPY" => "¥",
            "CHF" => "CHF",
            "CAD" => "C$",
            "AUD" => "A$",
            "BTC" => "₿",
            "ETH" => "Ξ",
            _ => currencyCode
        };
    }

    /// <summary>
    /// Traductions françaises
    /// </summary>
    private static Dictionary<string, string> GetFrenchTranslations()
    {
        return new Dictionary<string, string>
        {
            { "Dashboard", "Tableau de bord" },
            { "Markets", "Marchés" },
            { "Portfolio", "Portefeuille" },
            { "Analytics", "Analyses" },
            { "Settings", "Paramètres" },
            { "Loading", "Chargement..." },
            { "Error", "Erreur" },
            { "Refresh", "Actualiser" },
            { "LastUpdate", "Dernière mise à jour" },
            { "Price", "Prix" },
            { "Change", "Variation" },
            { "Volume", "Volume" },
            { "High", "Haut" },
            { "Low", "Bas" },
            { "Open", "Ouverture" },
            { "Close", "Fermeture" }
        };
    }

    /// <summary>
    /// Traductions anglaises
    /// </summary>
    private static Dictionary<string, string> GetEnglishTranslations()
    {
        return new Dictionary<string, string>
        {
            { "Dashboard", "Dashboard" },
            { "Markets", "Markets" },
            { "Portfolio", "Portfolio" },
            { "Analytics", "Analytics" },
            { "Settings", "Settings" },
            { "Loading", "Loading..." },
            { "Error", "Error" },
            { "Refresh", "Refresh" },
            { "LastUpdate", "Last update" },
            { "Price", "Price" },
            { "Change", "Change" },
            { "Volume", "Volume" },
            { "High", "High" },
            { "Low", "Low" },
            { "Open", "Open" },
            { "Close", "Close" }
        };
    }

    /// <summary>
    /// Traductions allemandes
    /// </summary>
    private static Dictionary<string, string> GetGermanTranslations()
    {
        return new Dictionary<string, string>
        {
            { "Dashboard", "Dashboard" },
            { "Markets", "Märkte" },
            { "Portfolio", "Portfolio" },
            { "Analytics", "Analysen" },
            { "Settings", "Einstellungen" },
            { "Loading", "Lädt..." },
            { "Error", "Fehler" },
            { "Refresh", "Aktualisieren" },
            { "LastUpdate", "Letzte Aktualisierung" },
            { "Price", "Preis" },
            { "Change", "Änderung" },
            { "Volume", "Volumen" },
            { "High", "Hoch" },
            { "Low", "Niedrig" },
            { "Open", "Eröffnung" },
            { "Close", "Schluss" }
        };
    }

    /// <summary>
    /// Traductions espagnoles
    /// </summary>
    private static Dictionary<string, string> GetSpanishTranslations()
    {
        return new Dictionary<string, string>
        {
            { "Dashboard", "Panel" },
            { "Markets", "Mercados" },
            { "Portfolio", "Cartera" },
            { "Analytics", "Análisis" },
            { "Settings", "Configuración" },
            { "Loading", "Cargando..." },
            { "Error", "Error" },
            { "Refresh", "Actualizar" },
            { "LastUpdate", "Última actualización" },
            { "Price", "Precio" },
            { "Change", "Cambio" },
            { "Volume", "Volumen" },
            { "High", "Alto" },
            { "Low", "Bajo" },
            { "Open", "Apertura" },
            { "Close", "Cierre" }
        };
    }
} 