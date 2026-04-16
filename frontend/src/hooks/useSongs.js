import { useEffect, useState } from "react";
import { fetchSongs } from "../services/api";

export function useSongs(params) {
  const [songs, setSongs] = useState([]);

  useEffect(() => {
    loadSongs();
  }, [params.seed, params.page, params.avgLikes, params.likes, params.locale]);

  async function loadSongs() {
    const data = await fetchSongs(params);
    setSongs(data);
  }

  return { songs };
}

export default useSongs;
