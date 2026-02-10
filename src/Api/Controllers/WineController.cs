using Api.Contracts.StorageUnit;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Api.Contracts.Wine;


namespace Api.Controllers;


[ApiController] 
[Route("api/[controller]")]
[Authorize]
public class WineController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public WineController(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public  async Task<ActionResult<WineContract>> PostWine(RegisterWineRequest request)
    {
        var wine = new Domain.Wine
        {
            WineId = Guid.NewGuid(),
            StorageUnitId = request.StorageUnitId,
            Name = request.Name,
            Wineyard = request.Wineyard,
            Type = request.Type,
            Vintage = request.Vintage
        };

        _context.Wines.Add(wine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetWine),
            new { id = wine.WineId },
            new WineContract(wine.WineId, wine.StorageUnitId ?? Guid.Empty, string.Empty, wine.Name, wine.Wineyard, wine.Type, wine.Vintage));
    }

    [HttpGet]
    public async Task<ActionResult<List<WineContract>>> GetWines()
    {
        var userId = GetCurrentUserId();
        var userCellars = await _context.Cellars
            .Where(c => c.UserId == userId)
            .Select(c => c.CellarId)
            .ToListAsync();
        var storageUnits = await _context.StorageUnits
            .Where(su => userCellars.Contains(su.CellarId))
            .Select(su => su.StorageUnitId)
            .ToListAsync();
        var wines = await _context.Wines
            .Where(w => w.StorageUnitId != null && storageUnits.Contains(w.StorageUnitId.Value))
            .Select(w => new WineContract(w.WineId, w.StorageUnitId ?? Guid.Empty, string.Empty, w.Name, w.Wineyard, w.Type, w.Vintage))
            .ToListAsync();

        return Ok(wines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WineContract>> GetWine(Guid id)
    {
        var userId = GetCurrentUserId();

        var userCellars = await _context.Cellars
            .Where(c => c.UserId == userId)
            .Select(c => c.CellarId)
            .ToListAsync();
        if(userCellars is null || !userCellars.Any())
        {
            return NotFound("No cellars found for the user.");
        }

        var storageUnits = await _context.StorageUnits
            .Where(su => userCellars.Contains(su.CellarId))
            .Select(su => su.StorageUnitId)
            .ToListAsync();
        if(storageUnits is null || !storageUnits.Any())
        {
            return NotFound("No storage units found for the user's cellars.");
        }

        var wine = await _context.Wines
            .Where(w => w.WineId == id && storageUnits.Contains(w.StorageUnitId ?? Guid.Empty))
            .FirstOrDefaultAsync();
        if (wine is null)
        {
            return NotFound("Wine not found in the user's storage units.");
        }

        return Ok(new WineContract(wine.WineId, wine.StorageUnitId ?? Guid.Empty, string.Empty, wine.Name, wine.Wineyard, wine.Type, wine.Vintage));
    }

    [HttpGet("by-storage-unit/{storageUnitId}")]
    public async Task<ActionResult<List<WineContract>>> GetWinesByStorageUnit(Guid storageUnitId)
    {
        var userId = GetCurrentUserId();

        var userCellars = await _context.Cellars
            .Where(c => c.UserId == userId)
            .Select(c => c.CellarId)
            .ToListAsync();
        if (userCellars is null || !userCellars.Any())
        {
            return NotFound("No cellars found for the user.");
        }
        var storageUnit = await _context.StorageUnits
            .Where(su => su.StorageUnitId == storageUnitId && userCellars.Contains(su.CellarId))
            .FirstOrDefaultAsync();
        if (storageUnit is null)
        {
            return NotFound("Storage unit does not belong to the user.");
        }

        var wines = await _context.Wines
            .Where(w => w.StorageUnitId == storageUnitId)
            .Select(w => new WineContract(w.WineId, w.StorageUnitId ?? Guid.Empty, string.Empty, w.Name, w.Wineyard, w.Type, w.Vintage))
            .ToListAsync();

        return Ok(wines);
    }


    //Helperfunction to get userId
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim is null ? Guid.Empty : Guid.Parse(userIdClaim.Value);
    }

}
