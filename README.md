# Music Store Showcase

Full-stack music store app built with ASP.NET Core and React/Vite, featuring seeded fake song generation, pagination, gallery view, cover images, and audio preview.

## Project Overview

Music Store Showcase is a deterministic media-generation demo. A user selects a language, a 64-bit seed, and an average likes value, then explores generated tracks in two views:

- a paginated table view with expandable details
- a gallery view with infinite scrolling

Each generated song includes metadata, a dynamic cover image, a short review, and a generated WAV preview.

## Tech Stack

- Backend: ASP.NET Core Web API (.NET 10)
- Frontend: React + Vite
- Data generation: Bogus
- Image generation: SixLabors.ImageSharp
- Deployment: Render

## Live Deployment

- Frontend: `https://music-store-frontend-8jop.onrender.com`
- Backend API: `https://music-store-api-k1ty.onrender.com`
- Health check: `https://music-store-api-k1ty.onrender.com/healthz`

## Screenshots

Add 1 to 3 screenshots in a folder such as `docs/screenshots/` and reference them here for the final GitHub presentation.

Suggested captures:

- toolbar and table view
- expanded row detail view
- gallery view

## Main Features

- deterministic song generation from a seed
- language switching
- adjustable average likes
- paginated table view
- expandable track detail panel
- gallery with infinite scrolling
- generated cover images
- generated audio preview
- Render deployment for both frontend and backend

## Project Structure

```text
Task5/
├── backend/
│   └── MusicStoreAPI/
│       ├── Controllers/
│       ├── DTOs/
│       ├── Models/
│       ├── Services/
│       ├── Utils/
│       ├── Program.cs
│       └── Dockerfile
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── hooks/
│   │   ├── pages/
│   │   ├── services/
│   │   ├── App.jsx
│   │   └── App.css
│   ├── .env.example
│   └── package.json
├── render.yaml
└── README.md
```

## Run Locally

### Backend

```powershell
cd backend\MusicStoreAPI
dotnet run
```

Local endpoints:

- `http://localhost:5000/`
- `http://localhost:5000/healthz`
- `http://localhost:5000/api/songs`

Swagger in development:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

Then open:

- `http://localhost:5000/swagger`

### Frontend

```powershell
cd frontend
copy .env.example .env
cmd /c npm install
cmd /c npm run dev
```

Then open:

- `http://localhost:5173`

## API Example

```text
http://localhost:5000/api/songs?seed=1234&page=1&avgLikes=5&locale=en-US
```

## Manual Checks

- same `seed` + same parameters returns the same songs
- changing `page` returns the next 20 songs
- `avgLikes=0` and `avgLikes=10` work correctly
- invalid `page` returns an error
- invalid `avgLikes` returns an error
- table row expands correctly
- gallery keeps loading more songs
- cover and audio endpoints work for generated songs

## Deployment Notes

The project includes a ready-to-use `render.yaml`.

- the backend is deployed as a Docker web service
- the frontend is deployed as a static site
- `VITE_API_URL` is injected by Render in production

## Future Improvements

- add automated backend tests
- improve multilingual text quality and encoding consistency
- add screenshot assets and a short demo GIF
- refine loading states and skeleton UI
- add architecture diagram and challenge/solution section
