// Service Worker pour Trading Dashboard - Blazor Web App .NET 8
const CACHE_NAME = 'trading-dashboard-v2-hybrid';
const STATIC_CACHE_NAME = 'trading-dashboard-static-v2';
const DYNAMIC_CACHE_NAME = 'trading-dashboard-dynamic-v2';

// Ressources à mettre en cache de manière statique
const STATIC_ASSETS = [
  '/',
  '/index.html',
  '/manifest.json',
  '/favicon.ico',
  '/icon-192.png',
  '/icon-512.png',
  '/_framework/blazor.web.js',
  '/bootstrap/bootstrap.min.css',
  '/app.css'
];

// Ressources dynamiques (API, données Supabase)
const DYNAMIC_ROUTES = [
  '/api/',
  '/rest/v1/',
  '/_framework/'
];

// Installation du Service Worker
self.addEventListener('install', function(event) {
  console.log('[SW] Installation du Service Worker');
  
  event.waitUntil(
    Promise.all([
      // Cache statique
      caches.open(STATIC_CACHE_NAME)
        .then(function(cache) {
          console.log('[SW] Mise en cache des ressources statiques');
          return cache.addAll(STATIC_ASSETS);
        }),
      
      // Cache dynamique (vide au départ)
      caches.open(DYNAMIC_CACHE_NAME)
        .then(function(cache) {
          console.log('[SW] Initialisation du cache dynamique');
          return cache;
        })
    ])
  );
  
  // Force l'activation immédiate
  self.skipWaiting();
});

// Activation du Service Worker
self.addEventListener('activate', function(event) {
  console.log('[SW] Activation du Service Worker');
  
  event.waitUntil(
    // Nettoyage des anciens caches
    caches.keys().then(function(cacheNames) {
      return Promise.all(
        cacheNames.map(function(cacheName) {
          if (cacheName !== STATIC_CACHE_NAME && 
              cacheName !== DYNAMIC_CACHE_NAME &&
              cacheName.startsWith('trading-dashboard-')) {
            console.log('[SW] Suppression de l\'ancien cache:', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    }).then(function() {
      // Prend le contrôle immédiatement
      return self.clients.claim();
    })
  );
});

// Interception des requêtes
self.addEventListener('fetch', function(event) {
  const request = event.request;
  const url = new URL(request.url);
  
  // Stratégie différente selon le type de ressource
  if (isStaticAsset(url.pathname)) {
    // Cache First pour les ressources statiques
    event.respondWith(cacheFirstStrategy(request, STATIC_CACHE_NAME));
  }
  else if (isDynamicRoute(url.pathname)) {
    // Network First pour les données dynamiques
    event.respondWith(networkFirstStrategy(request, DYNAMIC_CACHE_NAME));
  }
  else if (isSupabaseRequest(url)) {
    // Network Only pour Supabase avec fallback
    event.respondWith(networkOnlyWithFallback(request));
  }
  else {
    // Stratégie par défaut
    event.respondWith(defaultStrategy(request));
  }
});

// Cache First Strategy (ressources statiques)
function cacheFirstStrategy(request, cacheName) {
  return caches.open(cacheName)
    .then(function(cache) {
      return cache.match(request)
        .then(function(response) {
          if (response) {
            console.log('[SW] Ressource servie depuis le cache:', request.url);
            return response;
          }
          
          // Si pas en cache, récupérer depuis le réseau
          return fetch(request)
            .then(function(networkResponse) {
              // Mettre en cache si réussi
              if (networkResponse && networkResponse.status === 200) {
                cache.put(request, networkResponse.clone());
              }
              return networkResponse;
            });
        });
    });
}

// Network First Strategy (données dynamiques)
function networkFirstStrategy(request, cacheName) {
  return fetch(request)
    .then(function(response) {
      // Si succès, mettre en cache
      if (response && response.status === 200) {
        caches.open(cacheName)
          .then(function(cache) {
            cache.put(request, response.clone());
          });
      }
      return response;
    })
    .catch(function() {
      // En cas d'échec, essayer le cache
      return caches.open(cacheName)
        .then(function(cache) {
          return cache.match(request);
        });
    });
}

// Network Only avec fallback (Supabase)
function networkOnlyWithFallback(request) {
  return fetch(request)
    .catch(function() {
      // En cas d'échec, retourner une réponse d'erreur structurée
      return new Response(
        JSON.stringify({
          error: "Connexion Supabase indisponible",
          offline: true,
          timestamp: new Date().toISOString()
        }),
        {
          status: 503,
          statusText: 'Service Unavailable',
          headers: {
            'Content-Type': 'application/json'
          }
        }
      );
    });
}

// Stratégie par défaut
function defaultStrategy(request) {
  return fetch(request)
    .catch(function() {
      // Pour les pages, retourner la page d'accueil depuis le cache
      if (request.destination === 'document') {
        return caches.match('/');
      }
      return new Response('Resource not available offline', {
        status: 503,
        statusText: 'Service Unavailable'
      });
    });
}

// Fonctions utilitaires
function isStaticAsset(pathname) {
  return STATIC_ASSETS.some(asset => pathname === asset) ||
         pathname.includes('.css') ||
         pathname.includes('.js') ||
         pathname.includes('.png') ||
         pathname.includes('.jpg') ||
         pathname.includes('.ico');
}

function isDynamicRoute(pathname) {
  return DYNAMIC_ROUTES.some(route => pathname.startsWith(route));
}

function isSupabaseRequest(url) {
  return url.hostname.includes('supabase.co');
}

// Gestion des messages du client
self.addEventListener('message', function(event) {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    self.skipWaiting();
  }
  
  if (event.data && event.data.type === 'CACHE_CLEAR') {
    event.waitUntil(
      caches.keys().then(function(cacheNames) {
        return Promise.all(
          cacheNames.map(function(cacheName) {
            if (cacheName.startsWith('trading-dashboard-')) {
              return caches.delete(cacheName);
            }
          })
        );
      })
    );
  }
}); 