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
                d="M16 5H20V9"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M4 7H7C8.3 7 8.95 7 9.53 7.22C10.3 7.51 10.96 8.04 11.42 8.74C11.77 9.27 12.06 9.85 12.65 11.01L12.78 11.26C13.37 12.42 13.66 13 14.01 13.53C14.47 14.23 15.13 14.76 15.9 15.05C16.48 15.27 17.13 15.27 18.43 15.27H20"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M16 19H20V15"
                stroke="currentColor"
                strokeWidth="2.2"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <path
                d="M4 17H7C8.28 17 8.92 17 9.49 16.79C10.25 16.5 10.89 15.98 11.35 15.3C11.69 14.79 11.98 14.24 12.55 13.14L13 12.25"
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
