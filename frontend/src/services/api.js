const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

export async function fetchSongs({ seed = 12345, page = 1, avgLikes = 5, locale = "en-US" }) {
  const params = new URLSearchParams({
    seed: String(seed),
    page: String(page),
    avgLikes: String(avgLikes),
    locale
  });

  const response = await fetch(`${API_BASE_URL}/songs?${params.toString()}`);

  if (!response.ok) {
    throw new Error(`API error: ${response.status}`);
  }

  return response.json();
}

export const getSongs = fetchSongs;
