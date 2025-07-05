import { serve } from "https://deno.land/std@0.177.0/http/server.ts";

/**
 * Edge Function Supabase pour récupérer les données boursières
 * Sert de proxy sécurisé vers l'API Alpha Vantage
 * 
 * Variables d'environnement requises :
 * - AV_KEY : Clé API Alpha Vantage (gratuite sur alphavantage.co)
 * 
 * Limites gratuites :
 * - Alpha Vantage : 25 requêtes/jour
 * - Supabase : 500 000 invocations/mois
 */
serve(async (req) => {
  // Gestion des CORS pour les appels depuis le frontend
  const corsHeaders = {
    'Access-Control-Allow-Origin': '*',
    'Access-Control-Allow-Headers': 'authorization, x-client-info, apikey, content-type',
  };

  // Réponse aux requêtes OPTIONS (preflight)
  if (req.method === 'OPTIONS') {
    return new Response('ok', { headers: corsHeaders });
  }

  try {
    // Extraction du paramètre ticker depuis l'URL
    const { ticker } = Object.fromEntries(new URL(req.url).searchParams);
    
    if (!ticker) {
      return new Response(
        JSON.stringify({ error: 'Paramètre ticker manquant' }),
        { 
          status: 400,
          headers: { 
            ...corsHeaders,
            'Content-Type': 'application/json' 
          }
        }
      );
    }

    // Récupération de la clé API depuis les variables d'environnement
    const apiKey = Deno.env.get('AV_KEY');
    if (!apiKey) {
      return new Response(
        JSON.stringify({ error: 'Clé API Alpha Vantage non configurée' }),
        { 
          status: 500,
          headers: { 
            ...corsHeaders,
            'Content-Type': 'application/json' 
          }
        }
      );
    }

    // Construction de l'URL d'appel à Alpha Vantage
    const alphaVantageUrl = `https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol=${ticker}&apikey=${apiKey}`;
    
    // Appel à l'API Alpha Vantage
    const response = await fetch(alphaVantageUrl);
    const data = await response.json();

    // Vérification si la réponse contient une erreur
    if (data['Error Message']) {
      return new Response(
        JSON.stringify({ error: `Ticker invalide: ${ticker}` }),
        { 
          status: 400,
          headers: { 
            ...corsHeaders,
            'Content-Type': 'application/json' 
          }
        }
      );
    }

    // Retour des données au frontend
    return new Response(JSON.stringify(data), {
      headers: {
        ...corsHeaders,
        'Content-Type': 'application/json'
      }
    });

  } catch (error) {
    console.error('Erreur dans la fonction get-stock:', error);
    
    return new Response(
      JSON.stringify({ error: 'Erreur interne du serveur' }),
      { 
        status: 500,
        headers: { 
          ...corsHeaders,
          'Content-Type': 'application/json' 
        }
      }
    );
  }
}); 