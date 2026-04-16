# Music Store Showcase

Projet full-stack avec :

- un backend ASP.NET Core qui génère des chansons, covers et extraits audio
- un frontend React/Vite qui affiche les données en `Table View` et `Gallery View`

## Ce que fait le projet

- génération déterministe par `seed`
- choix de langue
- moyenne de likes modifiable
- vue table avec pagination
- vue galerie avec scroll infini
- ligne table extensible avec cover, review et audio
- cover générée dynamiquement
- extrait audio reproductible par seed

## Lancer en local

### Backend

```powershell
cd backend\MusicStoreAPI
dotnet run
```

API locale :

- `http://localhost:5000/api/songs`
- `http://localhost:5000/healthz`

Swagger en développement :

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run
```

Puis ouvrir :

- `http://localhost:5000/swagger`

### Frontend

```powershell
cd frontend
cmd /c npm install
cmd /c npm run dev
```

Puis ouvrir :

- `http://localhost:5173`

## Comment expliquer le projet à l'oral

### 1. Frontend

Le frontend est dans `frontend/src`.

- `App.jsx` pilote l'état global : `seed`, `locale`, `avgLikes`, vue active et pagination table
- `Toolbar.jsx` contient les contrôles utilisateur
- `SongTable.jsx` affiche la table paginée avec détail extensible
- `Gallery.jsx` affiche les cartes avec chargement infini
- `SongCard.jsx` affiche une chanson en mode galerie
- `services/api.js` centralise l'appel vers le backend

### 2. Backend

Le backend est dans `backend/MusicStoreAPI`.

- `Program.cs` configure l'application et les routes
- `SongsController.cs` expose `/api/songs`
- `SongService.cs` génère les données
- `CoverController.cs` génère la cover
- `AudioController.cs` génère l'extrait audio
- `AudioPreviewGenerator.cs` synthétise un WAV déterministe
- `SeedHelper.cs` garantit la reproductibilité

### 3. Logique importante

- même `seed` + mêmes paramètres = mêmes résultats
- changer seulement les likes ne change pas les titres/artistes/albums
- changer `seed` ou `locale` régénère les chansons
- la table revient à la page 1 quand les paramètres changent
- la galerie repart du haut quand les paramètres changent

## Tests à faire avant rendu

- vérifier que `GET /api/songs` retourne bien 20 chansons
- vérifier que le même seed redonne les mêmes morceaux
- vérifier que `avgLikes=0`, `0.5`, `3.7`, `10` fonctionne
- vérifier les 3 langues
- vérifier table + pagination
- vérifier galerie + infinite scroll
- vérifier qu'une ligne de table s'ouvre et se referme
- vérifier qu'une cover s'affiche avec le bon titre et le bon artiste
- vérifier que deux `audioUrl` de seeds différents ne jouent pas exactement la même chose

## Push GitHub

À la racine du projet :

```powershell
git init
git add .
git commit -m "Initial project setup"
git branch -M main
git remote add origin https://github.com/TON-USER/TON-REPO.git
git push -u origin main
```

## Déploiement Render

Le repo contient déjà un `render.yaml`.

### Option simple

1. pousse le projet sur GitHub
2. connecte le repo à Render
3. choisis Blueprint / `render.yaml`
4. crée les 2 services :
   - `music-store-api`
   - `music-store-frontend`

### Important

Dans `render.yaml`, la variable `VITE_API_URL` du frontend pointe vers :

- `https://music-store-api.onrender.com/api`

Si Render te donne un autre sous-domaine, remplace cette URL par la vraie URL du backend.

Le backend Render utilise un `Dockerfile` pour éviter les surprises de runtime sur .NET.

## Limites actuelles

- l'audio est bien généré, mais reste simple musicalement
- les données sont plus réalistes qu'au début, mais peuvent encore être enrichies
- pour un rendu plus propre, il faut tester une dernière fois en local avant le push
