const localeOptions = [
  { value: "en-US", label: "English (USA)" },
  { value: "de-DE", label: "German (Germany)" },
  { value: "uk-UA", label: "Ukrainian (Ukraine)" }
];

export default function Toolbar({ params, onChange, onRandomizeSeed }) {
  return (
    <section className="toolbar">
      <div className="control-group">
        <label htmlFor="locale">Language</label>
        <select
          id="locale"
          value={params.locale}
          onChange={(event) => onChange({ locale: event.target.value })}
        >
          {localeOptions.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </div>

      <div className="control-group">
        <label htmlFor="seed">Seed (64-bit)</label>
        <div className="seed-row">
          <input
            id="seed"
            type="text"
            inputMode="numeric"
            value={params.seed}
            onChange={(event) => onChange({ seed: event.target.value.replace(/[^\d-]/g, "") || "0" })}
          />
          <button type="button" onClick={onRandomizeSeed}>
            Random
          </button>
        </div>
      </div>

      <div className="control-group">
        <label htmlFor="avgLikes">Likes per song (0-10)</label>
        <input
          id="avgLikes"
          type="number"
          min="0"
          max="10"
          step="0.1"
          value={params.avgLikes}
          onChange={(event) => onChange({ avgLikes: Number(event.target.value) || 0 })}
        />
      </div>
    </section>
  );
}
