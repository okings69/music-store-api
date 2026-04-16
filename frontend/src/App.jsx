import { useEffect, useState } from "react";
import Toolbar from "./components/Toolbar";
import SongTable from "./components/SongTable";
import Gallery from "./components/Gallery";
import { fetchSongs } from "./services/api";
import "./App.css";

function randomSeed64() {
  const values = crypto.getRandomValues(new Uint32Array(2));
  const seed = (BigInt(values[0]) << 31n) ^ BigInt(values[1]);
  return (seed & 0x7fffffffffffffffn).toString();
}

function App() {
  const [params, setParams] = useState({
    seed: "12345",
    avgLikes: 5,
    locale: "en-US"
  });
  const [view, setView] = useState("table");
  const [tablePage, setTablePage] = useState(1);
  const [galleryResetToken, setGalleryResetToken] = useState(0);
  const [songs, setSongs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [reloadToken, setReloadToken] = useState(0);

  useEffect(() => {
    let ignore = false;

    async function loadSongs() {
      setLoading(true);
      setError("");

      try {
        const data = await fetchSongs({
          ...params,
          page: tablePage
        });

        if (!ignore) {
          setSongs(data);
        }
      } catch {
        if (!ignore) {
          setError("Unable to load songs.");
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
  }, [params, tablePage, reloadToken]);

  function updateParams(partialParams) {
    setParams((current) => ({ ...current, ...partialParams }));
    setTablePage(1);
    setGalleryResetToken((current) => current + 1);
  }

  function handleRandomSeed() {
    updateParams({ seed: randomSeed64() });
  }

  function handleRetry() {
    setReloadToken((current) => current + 1);
  }

  return (
    <div className="app">
      <div className="shell">
        <header className="hero">
          <div>
            <p className="eyebrow">Music Store Showcase</p>
            <h1>Explore generated songs in two views</h1>
            <p className="hero-copy">
              A full-stack demo that generates reproducible tracks, covers, and short audio previews
              from a configurable seed.
            </p>
          </div>
        </header>

        <Toolbar
          params={params}
          onChange={updateParams}
          onRandomizeSeed={handleRandomSeed}
        />

        <section className="view-header">
          <div className="view-switcher" role="tablist" aria-label="Display mode">
            <button
              type="button"
              className={view === "table" ? "active" : ""}
              onClick={() => setView("table")}
            >
              Table View
            </button>
            <button
              type="button"
              className={view === "gallery" ? "active" : ""}
              onClick={() => setView("gallery")}
            >
              Gallery View
            </button>
          </div>

          {view === "table" ? (
            <div className="page-controls">
              <span>Page {tablePage}</span>
              <button
                type="button"
                onClick={() => setTablePage((page) => Math.max(1, page - 1))}
                disabled={tablePage === 1}
              >
                Previous
              </button>
              <button type="button" onClick={() => setTablePage((page) => page + 1)}>
                Next
              </button>
            </div>
          ) : (
            <p className="gallery-hint">Infinite scroll is active in gallery mode.</p>
          )}
        </section>

        {error ? (
          <div className="feedback error feedback-with-action">
            <span>{error}</span>
            <button type="button" className="feedback-action" onClick={handleRetry}>
              Retry
            </button>
          </div>
        ) : null}
        {loading && view === "table" ? <div className="feedback">Loading songs...</div> : null}
        {!loading && !error && view === "table" && songs.length === 0 ? (
          <div className="feedback">No songs found for the selected parameters.</div>
        ) : null}

        {view === "table" ? (
          <SongTable
            key={`${tablePage}-${params.seed}-${params.locale}-${params.avgLikes}`}
            songs={songs}
          />
        ) : (
          <Gallery params={params} resetToken={galleryResetToken} />
        )}

        <section className="about-panel">
          <p className="about-label">About this project</p>
          <h2>Generated media, deterministic logic, and deploy-ready full stack</h2>
          <p>
            The frontend is built with React and Vite, while the backend uses ASP.NET Core to
            generate seeded song metadata, dynamic cover art, and short WAV previews. Render hosts
            both the API and the static frontend.
          </p>
        </section>
      </div>
    </div>
  );
}

export default App;
