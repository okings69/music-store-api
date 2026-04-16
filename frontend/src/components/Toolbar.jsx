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
            <svg
              className="shuffle-icon"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
              aria-hidden="true"
            >
              <path
                d="M16 7H20V11"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M4 18L10 18C11.2 18 11.8 18 12.3 17.7C12.74 17.46 13.11 17.09 13.35 16.65C13.64 16.12 13.64 15.47 13.64 14.18C13.64 12.89 13.64 12.24 13.93 11.71C14.17 11.27 14.54 10.9 14.98 10.66C15.51 10.36 16.16 10.36 17.45 10.36H20"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M16 17H20V13"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M4 6L10 6C11.2 6 11.8 6 12.3 6.3C12.74 6.54 13.11 6.91 13.35 7.35C13.64 7.88 13.64 8.53 13.64 9.82"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
            </svg>
            <span className="sr-only">Randomize seed</span>
          </button>
        </div>
      </div>

      <div className="control-group">
        <label htmlFor="avgLikes">Likes per song (0-10)</label>
        <div className="likes-control">
          <input
            id="avgLikes"
            className="likes-slider"
            type="range"
            min="0"
            max="10"
            step="0.1"
            value={params.avgLikes}
            onChange={(event) => onChange({ avgLikes: Number(event.target.value) || 0 })}
          />
          <output className="likes-value" htmlFor="avgLikes">
            {Number(params.avgLikes).toFixed(1)}
          </output>
        </div>
      </div>
    </section>
  );
}
