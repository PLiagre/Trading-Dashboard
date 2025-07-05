# 📈 Trading Dashboard

Dashboard financier moderne développé avec Blazor WebAssembly (.NET 8) pour suivre les marchés financiers en temps réel.

## 🎯 Fonctionnalités

### 🏠 Dashboard Principal
- **Vue d'ensemble** des marchés avec résumé par catégorie
- **Graphiques multi-actifs** en temps réel 
- **Statistiques** : actifs en hausse/baisse, volatilité moyenne
- **Indicateur de statut** des marchés (ouvert/fermé)

### 📊 Page Marchés
- **Filtrage par catégorie** : Indices, Crypto, Matières premières, Obligations
- **Vues multiples** : Grille et Liste
- **Tri avancé** : Par nom, prix, variation
- **Graphiques spécialisés** pour chaque catégorie

### 💎 Actifs Suivis

#### 📈 Indices Actions
- **CAC 40** (France)
- **S&P 500** (États-Unis)
- **NASDAQ 100** (États-Unis)

#### ₿ Cryptomonnaies
- **Bitcoin (BTC)**
- **Ethereum (ETH)**

#### 🥇 Matières Premières
- **Or (Gold)**

#### 🏛️ Obligations
- **US Treasury 10Y** (États-Unis)
- **France 10Y** (France)
- **Allemagne 10Y** (Bund)
- **Union Européenne 10Y**

## 🛠️ Technologies

- **Frontend** : Blazor WebAssembly (.NET 8)
- **Styling** : CSS moderne avec variables CSS
- **Graphiques** : CSS + SVG (sans dépendances externes)
- **Thèmes** : Support Dark/Light mode
- **Responsive** : Compatible mobile et desktop

## 🏗️ Architecture

```
/Client/
├── Pages/
│   ├── Dashboard.razor          # Vue principale
│   └── Markets.razor           # Page détaillée par catégorie
├── Components/
│   ├── MarketCard.razor        # Carte d'actif financier
│   ├── MultiChart.razor        # Graphique multi-courbes
│   ├── BondYieldChart.razor    # Graphique spécialisé obligations
│   └── LoadingSpinner.razor    # Indicateur de chargement
├── Services/
│   ├── MarketDataService.cs    # Simulation données marchés
│   └── ThemeService.cs         # Gestion dark/light mode
├── Shared/
│   ├── MainLayout.razor        # Layout principal
│   └── NavMenu.razor          # Navigation
└── wwwroot/
    ├── index.html             # Point d'entrée
    └── style.css              # Styles globaux
```

## 🚀 Démarrage Rapide

### Prérequis
- **.NET 8 SDK** ([Télécharger](https://dotnet.microsoft.com/download/dotnet/8.0))
- **IDE** : Visual Studio 2022, VS Code, ou JetBrains Rider

### Installation
```bash
# Cloner le projet
git clone <url-du-repo>
cd TradingDashboard

# Restaurer les dépendances
dotnet restore Client/TradingDashboard.Client.csproj

# Compiler le projet
dotnet build Client/TradingDashboard.Client.csproj
```

### Exécution
```bash
# Mode développement avec hot reload
dotnet watch run --project Client/TradingDashboard.Client.csproj

# Ou lancement standard
dotnet run --project Client/TradingDashboard.Client.csproj
```

L'application sera accessible sur `https://localhost:5001` ou `http://localhost:5000`.

### Build de Production
```bash
dotnet publish Client/TradingDashboard.Client.csproj -c Release -o dist/
```

## 📱 Fonctionnalités UX

### 🎨 Interface
- **Design moderne** avec gradient et ombres
- **Animations fluides** sur les interactions
- **Icons émojis** pour une meilleure lisibilité
- **Cards interactives** avec effets hover

### 🌙 Dark Mode
- **Toggle** automatique dans la barre de navigation
- **Persistance** via localStorage
- **Transition fluide** entre les thèmes

### 📊 Graphiques
- **Graphiques CSS/SVG** natifs (sans librairies)
- **Multi-courbes** pour comparer les actifs
- **Graphiques spécialisés** pour les obligations
- **Tooltips informatifs** sur survol

### 📱 Responsive
- **Mobile-first** design
- **Navigation adaptative** selon la taille d'écran
- **Grilles flexibles** pour tous les formats

## 🔄 Données Simulées

Le service `MarketDataService` génère des données simulées avec :

- **Mise à jour automatique** toutes les 3 secondes
- **Variations réalistes** selon le type d'actif
- **Historique sur 7 jours** pour les graphiques
- **Volatilité différenciée** :
  - Crypto : ±3%
  - Indices : ±1.5%
  - Or : ±1%
  - Obligations : ±0.5%

## 🔮 Extensions Futures

### 🌐 API Réelle
- Intégration **Alpha Vantage** ou **Financial Modeling Prep**
- **Supabase Edge Functions** comme proxy sécurisé
- **Rate limiting** et cache des données

### 📈 Fonctionnalités Avancées
- **Alertes** de prix personnalisées
- **Portefeuille virtuel** avec P&L
- **Indicateurs techniques** (RSI, MACD, Bollinger)
- **Actualités financières** intégrées

### 🔧 Technique
- **Progressive Web App** (PWA)
- **Notifications push** pour les alertes
- **Mode offline** avec cache
- **Tests unitaires** et d'intégration

## 🤝 Contribution

1. **Fork** le projet
2. **Créer** une branche feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** les changes (`git commit -m 'Add: Amazing Feature'`)
4. **Push** vers la branche (`git push origin feature/AmazingFeature`)
5. **Ouvrir** une Pull Request

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

## 👥 Auteurs

- **Développeur Principal** - [@votre-nom](https://github.com/votre-nom)

## 🙏 Remerciements

- **Microsoft** pour Blazor WebAssembly
- **Alpha Vantage** pour l'inspiration des données financières
- **Communauté .NET** pour les ressources et guides 