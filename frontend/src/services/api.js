const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

function normalizeSongParams(seedOrParams, page, avgLikes, locale) {
  if (typeof seedOrParams === "object" && seedOrParams !== null) {
    return {
      seed: seedOrParams.seed ?? 12345,
      page: seedOrParams.page ?? 1,
      avgLikes: seedOrParams.avgLikes ?? seedOrParams.likes ?? 5,
      locale: seedOrParams.locale ?? "en-US"
    };
  }

  return {
    seed: seedOrParams ?? 12345,
    page: page ?? 1,
    avgLikes: avgLikes ?? 5,
    locale: locale ?? "en-US"
  };
}

export const fetchSongs = async (seedOrParams, page, avgLikes, locale) => {
  const normalized = normalizeSongParams(seedOrParams, page, avgLikes, locale);
  const params = new URLSearchParams({
    seed: normalized.seed.toString(),
    page: normalized.page.toString(),
    avgLikes: normalized.avgLikes.toString(),
    locale: normalized.locale
  });

  const response = await fetch(`${API_BASE_URL}/songs?${params}`);
  if (!response.ok) {
    throw new Error("Erreur lors du chargement des chansons");
  }

  return response.json();
};

export const getSongs = fetchSongs;
