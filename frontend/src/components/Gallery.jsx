import { useEffect, useRef, useState } from "react";
import { getSongs } from "../services/api";
import SongCard from "./SongCard";

export default function Gallery({ params, resetToken }) {
  const [songs, setSongs] = useState([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const sentinelRef = useRef(null);

  useEffect(() => {
    setSongs([]);
    setPage(1);
    setError("");
    window.scrollTo({ top: 0, behavior: "auto" });
  }, [resetToken, params.seed, params.avgLikes, params.locale]);

  useEffect(() => {
    let ignore = false;

    async function loadSongs() {
      setLoading(true);
      setError("");

      try {
        const data = await getSongs({ ...params, page });
        if (!ignore) {
          setSongs((current) => (page === 1 ? data : [...current, ...data]));
        }
      } catch {
        if (!ignore) {
          setError("Unable to load gallery songs.");
        }
      } finally {
        if (!ignore) {
          setLoading(false);
        }
      }
    }

    loadSongs();

    return () => {
      ignore = true;
    };
  }, [page, params]);

  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel) {
      return undefined;
    }

    const observer = new IntersectionObserver((entries) => {
      if (entries[0]?.isIntersecting && !loading) {
        setPage((currentPage) => currentPage + 1);
      }
    }, { rootMargin: "300px" });

    observer.observe(sentinel);

    return () => {
      observer.disconnect();
    };
  }, [loading]);

  return (
    <section>
      {error ? <div className="feedback error">{error}</div> : null}
      {!loading && !error && songs.length === 0 ? (
        <div className="feedback">No songs found for the selected parameters.</div>
      ) : null}
      <div className="songs-grid">
        {songs.map((song) => (
          <SongCard key={song.index} song={song} />
        ))}
      </div>
      <div ref={sentinelRef} className="scroll-sentinel">
        {loading ? "Loading more songs..." : "Scroll for more"}
      </div>
    </section>
  );
}
