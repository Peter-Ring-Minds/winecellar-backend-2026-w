using System.Security.Claims;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("cellars")]
[Authorize]
public class CellarsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CellarsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<object>>> GetMine()
    {
        var userId = GetUserId();

        var cellars = await _db.Cellars
            .Where(c => c.Memberships.Any(m => m.UserId == userId))
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync();

        return Ok(cellars);
    }

    public sealed record CreateCellarRequest(string Name);

    [HttpPost]
    public async Task<ActionResult<object>> Create(CreateCellarRequest request)
    {
        var userId = GetUserId();

        var cellar = new Cellar
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Memberships = new List<CellarMembership>
            {
                new()
                {
                    CellarId = Guid.Empty, // will be overwritten below
                    UserId = userId,
                    Role = CellarRole.Owner
                }
            }
        };

        cellar.Memberships.First().CellarId = cellar.Id;

        _db.Cellars.Add(cellar);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMine), new { id = cellar.Id }, new { cellar.Id, cellar.Name });
    }

    [HttpGet("{cellarId:guid}/storage-units")]
    public async Task<ActionResult<IReadOnlyList<object>>> ListStorageUnits(Guid cellarId)
    {
        var userId = GetUserId();

        var hasAccess = await _db.CellarMemberships.AnyAsync(m => m.CellarId == cellarId && m.UserId == userId);
        if (!hasAccess)
        {
            return NotFound();
        }

        var units = await _db.StorageUnits
            .Where(su => su.CellarId == cellarId)
            .OrderBy(su => su.Name)
            .Select(su => new { su.Id, su.Name })
            .ToListAsync();

        return Ok(units);
    }

    public sealed record CreateStorageUnitRequest(string Name);

    [HttpPost("{cellarId:guid}/storage-units")]
    public async Task<ActionResult<object>> CreateStorageUnit(Guid cellarId, CreateStorageUnitRequest request)
    {
        var userId = GetUserId();

        var hasAccess = await _db.CellarMemberships.AnyAsync(m => m.CellarId == cellarId && m.UserId == userId);
        if (!hasAccess)
        {
            return NotFound();
        }

        var unit = new StorageUnit
        {
            Id = Guid.NewGuid(),
            CellarId = cellarId,
            Name = request.Name
        };

        _db.StorageUnits.Add(unit);
        await _db.SaveChangesAsync();

        return Ok(new { unit.Id, unit.Name });
    }

    [HttpGet("{cellarId:guid}/storage-units/{storageUnitId:guid}/wines")]
    public async Task<ActionResult<IReadOnlyList<object>>> ListWines(Guid cellarId, Guid storageUnitId)
    {
        var userId = GetUserId();

        var hasAccess = await _db.CellarMemberships.AnyAsync(m => m.CellarId == cellarId && m.UserId == userId);
        if (!hasAccess)
        {
            return NotFound();
        }

        var validUnit = await _db.StorageUnits.AnyAsync(su => su.Id == storageUnitId && su.CellarId == cellarId);
        if (!validUnit)
        {
            return NotFound();
        }

        var wines = await _db.Wines
            .Where(w => w.StorageUnitId == storageUnitId)
            .OrderBy(w => w.Name)
            .Select(w => new { w.Id, w.Name, w.Producer, w.Vintage, w.Quantity })
            .ToListAsync();

        return Ok(wines);
    }

    public sealed record CreateWineRequest(string Name, string? Producer, int? Vintage, int Quantity);

    [HttpPost("{cellarId:guid}/storage-units/{storageUnitId:guid}/wines")]
    public async Task<ActionResult<object>> CreateWine(Guid cellarId, Guid storageUnitId, CreateWineRequest request)
    {
        var userId = GetUserId();

        var hasAccess = await _db.CellarMemberships.AnyAsync(m => m.CellarId == cellarId && m.UserId == userId);
        if (!hasAccess)
        {
            return NotFound();
        }

        var validUnit = await _db.StorageUnits.AnyAsync(su => su.Id == storageUnitId && su.CellarId == cellarId);
        if (!validUnit)
        {
            return NotFound();
        }

        var wine = new Wine
        {
            Id = Guid.NewGuid(),
            StorageUnitId = storageUnitId,
            Name = request.Name,
            Producer = request.Producer,
            Vintage = request.Vintage,
            Quantity = request.Quantity
        };

        _db.Wines.Add(wine);
        await _db.SaveChangesAsync();

        return Ok(new { wine.Id });
    }

    private Guid GetUserId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId))
        {
            throw new InvalidOperationException("Authenticated user id claim is missing or invalid.");
        }

        return userId;
    }
}
