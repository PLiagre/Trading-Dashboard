using Microsoft.JSInterop;
using System.Globalization;

namespace TradingDashboard.Client.Services
{
    public class LocalizationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly Dictionary<string, Dictionary<string, string>> _resources;

        public event Action? OnLanguageChanged;

        public LocalizationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _resources = InitializeResources();
        }

        private Dictionary<string, Dictionary<string, string>> InitializeResources()
        {
            return new Dictionary<string, Dictionary<string, string>>
            {
                {
                    "fr", new Dictionary<string, string>
                    {
                        {"Dashboard", "Tableau de bord"},
                        {"Markets", "Marchés"},
                        {"Home", "Accueil"},
                        {"TradingDashboard", "Tableau de bord Trading"},
                        {"WelcomeMessage", "Bienvenue sur votre tableau de bord financier"},
                        {"MarketOverview", "Aperçu des marchés"},
                        {"AllMarkets", "Tous les marchés"},
                        {"MarketPerformance", "Performance des marchés"},
                        {"BondYields", "Rendements obligataires"},
                        {"Loading", "Chargement..."},
                        {"Indices", "Indices"},
                        {"Crypto", "Crypto"},
                        {"Commodities", "Matières premières"},
                        {"Bonds", "Obligations"},
                        {"All", "Tous"},
                        {"Price", "Prix"},
                        {"Change", "Variation"},
                        {"LastUpdate", "Dernière mise à jour"},
                        {"Volume", "Volume"},
                        {"High", "Haut"},
                        {"Low", "Bas"},
                        {"Open", "Ouverture"},
                        {"Close", "Fermeture"},
                        {"Search", "Rechercher"},
                        {"Filter", "Filtrer"},
                        {"SortBy", "Trier par"},
                        {"ViewMode", "Mode d'affichage"},
                        {"Grid", "Grille"},
                        {"List", "Liste"},
                        {"DarkMode", "Mode sombre"},
                        {"LightMode", "Mode clair"},
                        {"Language", "Langue"},
                        {"French", "Français"},
                        {"English", "Anglais"},
                        {"Today", "Aujourd'hui"},
                        {"ThisWeek", "Cette semaine"},
                        {"ThisMonth", "Ce mois"},
                        {"ThisYear", "Cette année"},
                        {"NoData", "Aucune donnée disponible"},
                        {"Error", "Erreur"},
                        {"Refresh", "Actualiser"},
                        {"AssetsUp", "Actifs en hausse"},
                        {"AssetsDown", "Actifs en baisse"},
                        {"AverageVolatility", "Volatilité moyenne"},
                        {"LastUpdateTime", "Dernière mise à jour"},
                        {"Name", "Nom"},
                        {"Details", "Détails"},
                        {"NoAssetsFound", "Aucun actif trouvé"},
                        {"NoAssetsMessage", "Aucun actif ne correspond aux critères sélectionnés."},
                        {"ViewAllAssets", "Voir tous les actifs"},
                        {"AllAssets", "Tous les actifs"}
                    }
                },
                {
                    "en", new Dictionary<string, string>
                    {
                        {"Dashboard", "Dashboard"},
                        {"Markets", "Markets"},
                        {"Home", "Home"},
                        {"TradingDashboard", "Trading Dashboard"},
                        {"WelcomeMessage", "Welcome to your financial dashboard"},
                        {"MarketOverview", "Market Overview"},
                        {"AllMarkets", "All Markets"},
                        {"MarketPerformance", "Market Performance"},
                        {"BondYields", "Bond Yields"},
                        {"Loading", "Loading..."},
                        {"Indices", "Indices"},
                        {"Crypto", "Crypto"},
                        {"Commodities", "Commodities"},
                        {"Bonds", "Bonds"},
                        {"All", "All"},
                        {"Price", "Price"},
                        {"Change", "Change"},
                        {"LastUpdate", "Last update"},
                        {"Volume", "Volume"},
                        {"High", "High"},
                        {"Low", "Low"},
                        {"Open", "Open"},
                        {"Close", "Close"},
                        {"Search", "Search"},
                        {"Filter", "Filter"},
                        {"SortBy", "Sort by"},
                        {"ViewMode", "View mode"},
                        {"Grid", "Grid"},
                        {"List", "List"},
                        {"DarkMode", "Dark mode"},
                        {"LightMode", "Light mode"},
                        {"Language", "Language"},
                        {"French", "French"},
                        {"English", "English"},
                        {"Today", "Today"},
                        {"ThisWeek", "This week"},
                        {"ThisMonth", "This month"},
                        {"ThisYear", "This year"},
                        {"NoData", "No data available"},
                        {"Error", "Error"},
                        {"Refresh", "Refresh"},
                        {"AssetsUp", "Assets up"},
                        {"AssetsDown", "Assets down"},
                        {"AverageVolatility", "Average volatility"},
                        {"LastUpdateTime", "Last update"},
                        {"Name", "Name"},
                        {"Details", "Details"},
                        {"NoAssetsFound", "No assets found"},
                        {"NoAssetsMessage", "No assets match the selected criteria."},
                        {"ViewAllAssets", "View all assets"},
                        {"AllAssets", "All assets"}
                    }
                }
            };
        }

        public string CurrentLanguage { get; private set; } = "fr";

        public string GetString(string key)
        {
            if (_resources.TryGetValue(CurrentLanguage, out var languageResources))
            {
                if (languageResources.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            return key; // Return key if not found
        }

        public async Task SetLanguageAsync(string languageCode)
        {
            if (languageCode == CurrentLanguage)
                return;

            if (_resources.ContainsKey(languageCode))
            {
                CurrentLanguage = languageCode;
                
                var culture = new CultureInfo(languageCode);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;

                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "language", languageCode);
                
                OnLanguageChanged?.Invoke();
            }
        }

        public async Task<string> GetStoredLanguageAsync()
        {
            try
            {
                var storedLanguage = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "language");
                return storedLanguage ?? "fr";
            }
            catch
            {
                return "fr";
            }
        }

        public async Task InitializeAsync()
        {
            var storedLanguage = await GetStoredLanguageAsync();
            if (storedLanguage != CurrentLanguage)
            {
                await SetLanguageAsync(storedLanguage);
            }
        }

        public List<(string Code, string Name)> GetAvailableLanguages()
        {
            return new List<(string, string)>
            {
                ("fr", GetString("French")),
                ("en", GetString("English"))
            };
        }
    }
} 