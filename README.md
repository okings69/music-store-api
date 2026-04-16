# Music Store Showcase

Full-stack project with:

- an ASP.NET Core backend that generates songs, covers, and audio clips
- a React/Vite frontend that displays the data in Table View and Gallery View

## What the project does

- deterministic generation by seed
- language selection
- adjustable average likes
- table view with pagination
- gallery view with infinite scrolling
- expandable table row with cover, review, and audio
- dynamically generated cover
- audio clip playable by seed

## Run locally

### Backend

```powershell
cd backend\MusicStoreAPI
dotnet run
```

Local API:

- `http://localhost:5000/api/songs`
- `http://localhost:5000/healthz`

Swagger in development :

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

Then open:

- `http://localhost:5000/swagger`

### Frontend

```powershell
cd frontend
cmd /c npm install
cmd /c npm run dev
```

Then open:

- `http://localhost:5173`

## How to explain the project verbally

### 1. Frontend

The frontend is located in `frontend/src`.

- `App.jsx` controls global state: `seed`, `locale`, `avgLikes`, active view, and table pagination.
- `Toolbar.jsx` contains user controls.
- `SongTable.jsx` displays the paginated table with expandable detail.
- `Gallery.jsx` displays cards with infinite loading.
- `SongCard.jsx` displays a song in gallery mode.
- `services/api.js` centralizes the call to the backend.

### 2. Backend

The backend is located in `backend/MusicStoreAPI`.

- `Program.cs` configures the application and routes
- `SongsController.cs` exposes `/api/songs`
- `SongService.cs` generates the data
- `CoverController.cs` generates the cover art
- `AudioController.cs` generates the audio snippet
- `AudioPreviewGenerator.cs` synthesizes a deterministic WAV file
- `SeedHelper.cs` ensures reproducibility

### 3. Important Logic

- Same `seed` + same parameters = same results
- Changing only the likes does not change the titles/artists/albums
- Changing `seed` or `locale` regenerates the songs
- The table returns to page 1 when the parameters change
- The gallery restarts from the top when the parameters change

## Tests to perform before rendering

- Verify that `GET /api/songs` Returns 20 songs correctly
- Verify that the same seed returns the same tracks
- Verify that `avgLikes=0`, `0.5`, `3.7`, `10` works
- Verify the 3 languages
- Verify table and pagination
- Verify gallery and infinite scrolling
- Verify that a table row opens and closes
- Verify that a cover displays with the correct title and artist
- Verify that two `audioUrl`s from different seeds do not play exactly the same thing

## Render Deployment

The repo already contains a `render.yaml`.

### Simple Option

### Important

In `render.yaml`, the frontend variable `VITE_API_URL` points to:

- `https://music-store-api.onrender.com/api`

If Render gives you a different subdomain, replace this URL with the actual backend URL.

The Render backend uses a `Dockerfile` to avoid runtime surprises on .NET.

## Current Limitations

- The audio is generated correctly, but remains musically simple.
- The data is more realistic than initially, but can still be enriched.