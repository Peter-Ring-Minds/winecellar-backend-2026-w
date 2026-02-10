using Api.Contracts.StorageUnit;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace Api.Controllers;


[ApiController] 
[Route("api/[controller]")]
[Authorize]
public class StorageUnitController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public StorageUnitController(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<ActionResult<StorageUnitContract>> PostStorageUnit(RegisterStorageUnitRequest request)
    {
        var storageUnit = new Domain.StorageUnit
        {
            StorageUnitId = Guid.NewGuid(),
            CellarId = request.CellarId,
            StorageUnitName = request.Name
        };

        _context.StorageUnits.Add(storageUnit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetStorageUnit),
            new { id = storageUnit.StorageUnitId },
            new StorageUnitContract(storageUnit.StorageUnitId, storageUnit.CellarId, storageUnit.StorageUnitName));
    }

    [HttpGet]
    public async Task<ActionResult<List<StorageUnitContract>>> GetStorageUnits()
    {
        var userId = GetCurrentUserId();
        var userCellars = await _context.Cellars
            .Where(c => c.UserId == userId)
            .Select(c => c.CellarId)
            .ToListAsync();
        var storageUnits = await _context.StorageUnits
            .Where(su => userCellars.Contains(su.CellarId))
            .Select(su => new StorageUnitContract(su.StorageUnitId, su.CellarId, su.StorageUnitName))
            .ToListAsync();

        return Ok(storageUnits);
    }

    //Get StorageUnits by CellarId
    [HttpGet("GetStorageUnitsByCellar/{cellarId}")]
    public async Task<ActionResult<List<StorageUnitContract>>> GetStorageUnitsByCellar(Guid cellarId)
    {
        var userId = GetCurrentUserId();
        var cellar = await _context.Cellars
            .Where(c => c.CellarId == cellarId && c.UserId == userId)
            .FirstOrDefaultAsync();
        if (cellar is null)        {
            return NotFound();
        }
        var storageUnits = await _context.StorageUnits
            .Where(su => su.CellarId == cellarId)
            .Select(su => new StorageUnitContract(su.StorageUnitId, su.CellarId, su.StorageUnitName))
            .ToListAsync();

        return Ok(storageUnits);
    }

    //Get StorageUnit by StorageUnitId
    [HttpGet("GetStorageUnit/{id}")]
    public async Task<ActionResult<StorageUnitContract>> GetStorageUnit(Guid id)
    {
        var storageUnit = await _context.StorageUnits
        .Where(x => x.StorageUnitId == id)
        .FirstOrDefaultAsync();
        if (storageUnit is null)
        {
            return NotFound();
        }

        return Ok(new StorageUnitContract(storageUnit.StorageUnitId, storageUnit.CellarId, storageUnit.StorageUnitName));
    }       

 
    //Helperfunction to get userId
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim is null ? Guid.Empty : Guid.Parse(userIdClaim.Value);
    }
    
}