# Winecellar Backend (intern training)

This repo is intentionally set up so that:

- The **database runs in Docker**
- The **API runs locally**

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
