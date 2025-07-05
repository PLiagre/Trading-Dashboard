using Microsoft.JSInterop;

namespace TradingDashboard.Client.Services;

/// <summary>
/// Service pour gérer les thèmes (dark/light mode)
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkMode = false;
    
    public event Action? ThemeChanged;

    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Indique si le mode sombre est activé
    /// </summary>
    public bool IsDarkMode => _isDarkMode;

    /// <summary>
    /// Initialise le thème depuis le localStorage
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            var theme = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
            _isDarkMode = theme == "dark";
            await ApplyThemeAsync();
        }
        catch
        {
            // Fallback si localStorage n'est pas disponible
            _isDarkMode = false;
        }
    }

    /// <summary>
    /// Bascule entre le mode sombre et clair
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        _isDarkMode = !_isDarkMode;
        await SaveThemeAsync();
        await ApplyThemeAsync();
        ThemeChanged?.Invoke();
    }

    /// <summary>
    /// Applique le thème actuel au document
    /// </summary>
    private async Task ApplyThemeAsync()
    {
        try
        {
            var theme = _isDarkMode ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("document.documentElement.setAttribute", "data-theme", theme);
        }
        catch
        {
            // Ignore si JS n'est pas disponible
        }
    }

    /// <summary>
    /// Sauvegarde le thème dans le localStorage
    /// </summary>
    private async Task SaveThemeAsync()
    {
        try
        {
            var theme = _isDarkMode ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
        }
        catch
        {
            // Ignore si localStorage n'est pas disponible
        }
    }

    /// <summary>
    /// Obtient les classes CSS pour le thème actuel
    /// </summary>
    public string GetThemeClasses()
    {
        return _isDarkMode ? "dark-theme" : "light-theme";
    }

    /// <summary>
    /// Obtient l'icône pour le bouton de basculement
    /// </summary>
    public string GetThemeIcon()
    {
        return _isDarkMode ? "☀️" : "🌙";
    }

    /// <summary>
    /// Obtient le texte pour le bouton de basculement
    /// </summary>
    public string GetThemeText()
    {
        return _isDarkMode ? "Mode clair" : "Mode sombre";
    }
} 