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
            <th>#</th>
            <th>Title</th>
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
      <tr className="summary-row" onClick={onToggle}>
        <td>{song.index}</td>
        <td>{song.title}</td>
        <td>{song.artist}</td>
        <td>{song.album}</td>
        <td>{song.genre}</td>
        <td>{song.likes}</td>
      </tr>

      {expanded ? (
        <tr className="detail-row">
          <td colSpan="6">
            <div className="detail-card">
              <img className="detail-cover" src={song.coverUrl} alt={`Cover for ${song.title}`} />

              <div className="detail-copy">
                <p className="detail-title">{song.title}</p>
                <p className="detail-meta">{song.artist} • {song.album} • {song.genre}</p>
                <p className="detail-review">{song.review}</p>
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
