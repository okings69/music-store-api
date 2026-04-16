export default function SongCard({ song }) {
  return (
    <article className="song-card">
      <img className="song-cover" src={song.coverUrl} alt={`Cover for ${song.title}`} />

      <div className="song-header">
        <span className="song-index">#{song.index}</span>
        <span className="song-likes">Likes {song.likes}</span>
      </div>

      <h3>{song.title}</h3>
      <p className="artist">{song.artist}</p>
      <p className="album">Album: {song.album}</p>
      <p className="genre">Genre: {song.genre}</p>
      <p className="review">"{song.review}"</p>

      <audio controls preload="none" className="card-audio">
        <source src={song.audioUrl} type="audio/wav" />
      </audio>
    </article>
  );
}
