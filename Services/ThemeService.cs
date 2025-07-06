using Microsoft.JSInterop;

namespace TradingDashboard.Services;

/// <summary>
/// Service de gestion des thèmes (clair/sombre) avec persistance en localStorage
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<ThemeService> _logger;
    private bool _isDarkMode = true; // Thème sombre par défaut

    public ThemeService(IJSRuntime jsRuntime, ILogger<ThemeService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <summary>
    /// Événement déclenché lors du changement de thème
    /// </summary>
    public event Action<bool>? ThemeChanged;

    /// <summary>
    /// Initialise le service et charge le thème depuis localStorage
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            // Récupération du thème depuis localStorage
            var storedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme");
            
            if (storedTheme != null)
            {
                _isDarkMode = storedTheme == "dark";
            }
            else
            {
                // Détection automatique basée sur les préférences système
                var prefersDark = await _jsRuntime.InvokeAsync<bool>("window.matchMedia", "(prefers-color-scheme: dark)");
                _isDarkMode = prefersDark;
            }

            await ApplyThemeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'initialisation du thème");
        }
    }

    /// <summary>
    /// Vérifie si le mode sombre est activé
    /// </summary>
    public async Task<bool> IsDarkModeAsync()
    {
        return await Task.FromResult(_isDarkMode);
    }

    /// <summary>
    /// Définit le thème et le persiste
    /// </summary>
    public async Task SetThemeAsync(bool isDarkMode)
    {
        try
        {
            _isDarkMode = isDarkMode;
            
            // Sauvegarde en localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", isDarkMode ? "dark" : "light");
            
            // Application du thème
            await ApplyThemeAsync();
            
            // Notification du changement
            ThemeChanged?.Invoke(_isDarkMode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la définition du thème");
        }
    }

    /// <summary>
    /// Bascule entre thème clair et sombre
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        await SetThemeAsync(!_isDarkMode);
    }

    /// <summary>
    /// Applique le thème actuel au DOM
    /// </summary>
    private async Task ApplyThemeAsync()
    {
        try
        {
            var themeClass = _isDarkMode ? "dark-theme" : "light-theme";
            
            // Application de la classe CSS au body
            await _jsRuntime.InvokeVoidAsync("eval", $@"
                document.body.className = document.body.className
                    .replace(/\b(light-theme|dark-theme)\b/g, '')
                    .trim() + ' {themeClass}';
                
                // Mise à jour des CSS custom properties
                const root = document.documentElement;
                if ('{_isDarkMode}' === 'True') {{
                    root.style.setProperty('--bg-color', '#1a1a2e');
                    root.style.setProperty('--text-color', '#ffffff');
                    root.style.setProperty('--card-bg', '#16213e');
                    root.style.setProperty('--border-color', '#2d3748');
                    root.style.setProperty('--chart-bg', '#2d3748');
                }} else {{
                    root.style.setProperty('--bg-color', '#ffffff');
                    root.style.setProperty('--text-color', '#212529');
                    root.style.setProperty('--card-bg', '#f8f9fa');
                    root.style.setProperty('--border-color', '#dee2e6');
                    root.style.setProperty('--chart-bg', '#ffffff');
                }}
            ");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'application du thème");
        }
    }

    /// <summary>
    /// Récupère les couleurs du thème actuel
    /// </summary>
    public ThemeColors GetCurrentThemeColors()
    {
        return _isDarkMode ? GetDarkThemeColors() : GetLightThemeColors();
    }

    /// <summary>
    /// Couleurs du thème sombre
    /// </summary>
    private static ThemeColors GetDarkThemeColors()
    {
        return new ThemeColors
        {
            Background = "#1a1a2e",
            Text = "#ffffff",
            CardBackground = "#16213e",
            Border = "#2d3748",
            Primary = "#007bff",
            Success = "#28a745",
            Danger = "#dc3545",
            Warning = "#ffc107"
        };
    }

    /// <summary>
    /// Couleurs du thème clair
    /// </summary>
    private static ThemeColors GetLightThemeColors()
    {
        return new ThemeColors
        {
            Background = "#ffffff",
            Text = "#212529",
            CardBackground = "#f8f9fa",
            Border = "#dee2e6",
            Primary = "#007bff",
            Success = "#28a745",
            Danger = "#dc3545",
            Warning = "#ffc107"
        };
    }
}

/// <summary>
/// Modèle des couleurs de thème
/// </summary>
public class ThemeColors
{
    public string Background { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string CardBackground { get; set; } = string.Empty;
    public string Border { get; set; } = string.Empty;
    public string Primary { get; set; } = string.Empty;
    public string Success { get; set; } = string.Empty;
    public string Danger { get; set; } = string.Empty;
    public string Warning { get; set; } = string.Empty;
} 