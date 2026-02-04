# .NET basics for this repo (intern guide)

This repo is structured so you practice **.NET CLI + EF Core** on your machine while only running **SQL Server in Docker**.

## Repo structure (where things go)

- `src/Domain/` → plain C# domain entities (your “models”)
- `src/Infrastructure/` → EF Core (`AppDbContext`), migrations, database-related code
- `src/Api/` → controllers/endpoints, authentication, DI wiring (`Program.cs`)

## Common commands

From the repo root:

- Restore/build/run:

```bash
dotnet restore
dotnet build
dotnet run --project src/Api/Api.csproj
```

- Run DB in Docker:

```bash
docker compose up -d
```

- Apply migrations to your local DB:

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Api/Api.csproj
```

If `dotnet ef` is missing, install the tool:

```bash
dotnet tool install --global dotnet-ef
```

## Add a new model (EF Core entity) + migrate DB

Example: add a `Region` model.

### 1) Create the entity in Domain

Create `src/Domain/Entities/Region.cs`:

```csharp
namespace Domain.Entities;

public class Region
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
```

### 2) Add it to the DbContext

In `src/Infrastructure/AppDbContext.cs`:

- Add a `DbSet<Region>`
- Optionally configure constraints in `OnModelCreating` (e.g. max lengths, indexes)

Typical pattern:

```csharp
public DbSet<Region> Regions => Set<Region>();

// in OnModelCreating
modelBuilder.Entity<Region>(entity =>
{
    entity.Property(x => x.Name).HasMaxLength(200);
});
```

### 3) Create a migration

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef migrations add AddRegions \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Api/Api.csproj \
  --output-dir Migrations
```

This writes migration files under `src/Infrastructure/Migrations/`.

### 4) Apply it to the local database

```bash
ASPNETCORE_ENVIRONMENT=Development dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Api/Api.csproj
```

### 5) Sanity-check

- `dotnet build`
- Run the API and ensure it starts without exceptions.

## Create a controller + endpoints that fetch models

You generally want:

- **DTOs** for requests/responses (don’t return EF entities directly for anything non-trivial)
- `AppDbContext` injected into your controller
- `async` + EF Core LINQ queries

Example controller: `src/Api/Controllers/RegionsController.cs`

```csharp
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("regions")]
public class RegionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RegionsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<object>>> List()
    {
        var items = await _db.Regions
            .OrderBy(r => r.Name)
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();

        return Ok(items);
    }

    public sealed record CreateRegionRequest(string Name);

    [HttpPost]
    public async Task<ActionResult<object>> Create(CreateRegionRequest request)
    {
        var region = new Domain.Entities.Region
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        _db.Regions.Add(region);
        await _db.SaveChangesAsync();

        return Ok(new { region.Id });
    }
}
```

## Hide endpoints behind authentication

This project uses **JWT bearer auth**.

### 1) Require authentication

Add `[Authorize]`:

```csharp
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("regions")]
public class RegionsController : ControllerBase
{
    // ...
}
```

Now requests must send:

`Authorization: Bearer <token>`

### 2) Get the current user id

The JWT includes `ClaimTypes.NameIdentifier` (set in `JwtTokenService`). In a controller:

```csharp
using System.Security.Claims;

var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
```

### 3) Enforce “entitlements” (only show what the user should see)

Authentication answers “who are you?”. Authorization/entitlements answers “what can you access?”.

Common approaches:

- **Ownership column** (simple): add `OwnerUserId` to the entity and filter by it.
- **Join table** (flexible): e.g. a membership table linking users to a resource (cellar membership).

Example query pattern:

```csharp
var userId = /* parse ClaimTypes.NameIdentifier to Guid */;

var items = await _db.SomeEntities
    .Where(x => x.OwnerUserId == userId)
    .ToListAsync();
```

Or for membership:

```csharp
var items = await _db.Cellars
    .Where(c => c.Memberships.Any(m => m.UserId == userId))
    .ToListAsync();
```

### 4) Common pitfalls

- If you get a 401: you forgot the `Authorization: Bearer ...` header.
- If you get a 404 on protected nested resources: the API may deliberately return `NotFound()` instead of `Forbidden()` to avoid leaking that the resource exists.

## Adding packages (NuGet)

From repo root:

```bash
dotnet add src/Api/Api.csproj package <PackageName>
```

or for Infrastructure:

```bash
dotnet add src/Infrastructure/Infrastructure.csproj package <PackageName>
```

## Quick auth smoke test

1) Start DB: `docker compose up -d`
2) Run API: `dotnet run --project src/Api/Api.csproj`
3) Use Swagger to call:
   - `POST /auth/register`
   - `POST /auth/login`
   - then call a protected endpoint with the Bearer token
