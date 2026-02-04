# Winecellar Backend (intern training)

This repo is intentionally set up so that:

- The **database runs in Docker**
- The **API runs locally**

## Quick start (run the project)

After you’ve run one of the setup scripts (or manually set `SA_PASSWORD` + user-secrets), you can run the project with:

```bash
docker compose up -d
dotnet run --project src/Api/Api.csproj
```

Intern guide: see `docs/dotnet-basics.md` for how to add models/migrations, create controllers, and protect endpoints.

## Prereqs

- Docker Desktop (Windows/macOS)
- .NET SDK (the project targets .NET 10)

## Start the database (Docker)

From the repo root:

Quick setup scripts:

- macOS:

```bash
bash scripts/setup-mac.sh
```

- Windows (PowerShell):

```powershell
Set-ExecutionPolicy -Scope Process Bypass
.\scripts\setup-windows.ps1 -StartDb
```

Manual setup:

1) Create a local `.env` (not committed):

```bash
cp .env.example .env
```

Edit `.env` and set `SA_PASSWORD`.

```bash
docker compose up -d
```

SQL Server will be available on `localhost:1433`.

> Apple Silicon (M1/M2/M3): the compose file pins the DB container to `linux/amd64` for compatibility.

## Run the API (host)

```bash
dotnet restore

dotnet run --project src/Api/Api.csproj
```

Swagger should be available when running in Development.

## Authentication (JWT)

This backend uses JWT bearer authentication.

- You must set a signing key `Jwt:Key` (user-secrets recommended).
- The setup scripts configure this automatically.

Note: `Jwt:Key` must be at least 32 bytes for HS256.

Set manually (dev only):

```bash
dotnet user-secrets set --project src/Api/Api.csproj "Jwt:Key" "YOUR_DEV_SIGNING_KEY"
dotnet user-secrets set --project src/Api/Api.csproj "Jwt:Issuer" "Winecellar"
dotnet user-secrets set --project src/Api/Api.csproj "Jwt:Audience" "Winecellar"
```

Auth endpoints:

- `POST /auth/register` → returns `{ accessToken }`
- `POST /auth/login` → returns `{ accessToken }`

Use the token in Swagger via `Authorize` → `Bearer {token}`.

## Data structure

The domain model is structured as:

`Cellar` → `StorageUnit` → `Wine`

Entitlements are enforced by membership: users only see cellars (and nested storage units/wines) they are members of.

## Connection string

For training purposes, we do **not** commit DB passwords.

Provide the connection string on your machine using **either** user-secrets (recommended) or an environment variable.

If you used the setup scripts, this is already done for you.

User-secrets (recommended):

```bash
dotnet user-secrets set --project src/Api/Api.csproj \
	"ConnectionStrings:Default" \
	"Server=localhost,1433;Database=WinecellarDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
```

Confirm it is set:

```bash
dotnet user-secrets list --project src/Api/Api.csproj
```

Find where secrets are stored on your machine:

```bash
dotnet user-secrets path
```

Environment variable (one session):

```bash
export ConnectionStrings__Default='Server=localhost,1433;Database=WinecellarDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True'
```

(Windows PowerShell)

```powershell
$env:ConnectionStrings__Default='Server=localhost,1433;Database=WinecellarDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True'
```

## Stop / reset

```bash
docker compose down
```

To remove DB data too:

```bash
docker compose down -v
```

## Troubleshooting

- SQL Server won’t start / container is unhealthy: `SA_PASSWORD` must satisfy SQL Server’s password policy (uppercase + lowercase + number + symbol, 8+ chars).
- You changed `SA_PASSWORD` but login fails: the DB volume keeps the old password. Reset with `docker compose down -v` and start again.
- API fails on startup about `ConnectionStrings:Default`: set it with `dotnet user-secrets set --project src/Api/Api.csproj "ConnectionStrings:Default" "..."`.
- API fails on startup about `Jwt:Key`: set a 32+ byte key with `dotnet user-secrets set --project src/Api/Api.csproj "Jwt:Key" "..."` (setup scripts do this automatically).
