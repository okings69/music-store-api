import { useState } from "react";

export default function SongTable({ songs }) {
  const [expandedRow, setExpandedRow] = useState(null);

  function toggleRow(id) {
    setExpandedRow((current) => (current === id ? null : id));
  }

  return (
    <section className="table-panel">
      <table className="songs-table">
        <thead>
          <tr>
            <th className="chevron-column" aria-label="Expand row"></th>
            <th>#</th>
            <th>Song</th>
            <th>Artist</th>
            <th>Album</th>
            <th>Genre</th>
            <th>Likes</th>
          </tr>
        </thead>
        <tbody>
          {songs.map((song) => (
            <FragmentRow
              key={song.id}
              song={song}
              expanded={expandedRow === song.id}
              onToggle={() => toggleRow(song.id)}
            />
          ))}
        </tbody>
      </table>
    </section>
  );
}

function FragmentRow({ song, expanded, onToggle }) {
  return (
    <>
      <tr
        className={`summary-row ${expanded ? "is-expanded" : ""}`}
        onClick={onToggle}
        aria-expanded={expanded}
      >
        <td className="chevron-cell">
          <button
            type="button"
            className={`row-chevron ${expanded ? "is-open" : ""}`}
            onClick={(event) => {
              event.stopPropagation();
              onToggle();
            }}
            aria-label={expanded ? `Collapse ${song.title}` : `Expand ${song.title}`}
          >
            <span></span>
          </button>
        </td>
        <td>{song.index}</td>
        <td className="song-title-cell">{song.title}</td>
        <td>{song.artist}</td>
        <td>{song.album}</td>
        <td>{song.genre}</td>
        <td>{song.likes}</td>
      </tr>

      {expanded ? (
        <tr className="detail-row">
          <td colSpan="7">
            <div className="detail-card detail-card-inline">
              <div className="detail-cover-shell">
                <img className="detail-cover" src={song.coverUrl} alt={`Cover for ${song.title}`} />
                <div className="detail-like-badge">Likes {song.likes}</div>
              </div>

              <div className="detail-copy">
                <div className="detail-topline">
                  <p className="detail-title">{song.title}</p>
                  <span className="detail-chip">{song.genre}</span>
                </div>

                <p className="detail-meta">
                  <strong>{song.artist}</strong>
                  <span>/</span>
                  <span>{song.album}</span>
                </p>

                <div className="detail-facts">
                  <div className="detail-fact">
                    <span className="detail-fact-label">Track</span>
                    <span>#{song.index}</span>
                  </div>
                  <div className="detail-fact">
                    <span className="detail-fact-label">Album</span>
                    <span>{song.album}</span>
                  </div>
                  <div className="detail-fact">
                    <span className="detail-fact-label">Artist</span>
                    <span>{song.artist}</span>
                  </div>
                </div>

                <div className="detail-review-block">
                  <span className="detail-review-label">Review</span>
                  <p className="detail-review">{song.review}</p>
                </div>

                <audio controls preload="none" className="detail-audio">
                  <source src={song.audioUrl} type="audio/wav" />
                </audio>
              </div>
            </div>
          </td>
        </tr>
      ) : null}
    </>
  );
}
