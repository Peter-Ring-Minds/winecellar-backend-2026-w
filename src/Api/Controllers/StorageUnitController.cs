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
    public async Task<ActionResult<StorageUnitContract>> PostStorageUnit(StorageUnitContract request)
    {
        var storageUnit = new Domain.StorageUnit
        {
            UserId = GetCurrentUserId(),
            Id = Guid.NewGuid(),
            CellarId = request.CellarId,
            Name = request.Name
        };

        _context.StorageUnits.Add(storageUnit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetStorageUnit),
            new { id = storageUnit.Id },
            CreateStorageUnitContract(storageUnit));
    }

    [HttpGet]
    public async Task<ActionResult<List<StorageUnitContract>>> GetStorageUnits()
    {

        var userId = GetCurrentUserId();
        var userStorageUnits = await _context.StorageUnits
            .Where(su => su.UserId == userId)
            .ToListAsync();

        return Ok(userStorageUnits);
    }

    //Get StorageUnits by CellarId
    [HttpGet("by-cellar-id/{cellarId}")]
    public async Task<ActionResult<List<StorageUnitContract>>> GetStorageUnitsByCellar(Guid cellarId)
    {
        var userId = GetCurrentUserId();
        var storageUnits = await _context.StorageUnits
            .Where(su => su.CellarId == cellarId && su.UserId == userId)
            .ToListAsync();
        return Ok(storageUnits);
    }

    //Get StorageUnit by StorageUnitId
    [HttpGet("{id}")]
    public async Task<ActionResult<StorageUnitContract>> GetStorageUnit(Guid id)
    {
        var userId = GetCurrentUserId();
        var storageUnit = await _context.StorageUnits
        .Where(x => x.Id == id && x.UserId == userId)
        .FirstOrDefaultAsync();
        if (storageUnit is null)
        {
            return NotFound();
        }

        return Ok(CreateStorageUnitContract(storageUnit));
    }       

    //Helper function for building StorageUnitContract from StorageUnit
    private StorageUnitContract CreateStorageUnitContract(Domain.StorageUnit storageUnit)
    {
        return new StorageUnitContract
        {
            StorageUnitId = storageUnit.Id,
            CellarId = storageUnit.CellarId,
            Name = storageUnit.Name
        };
    }


 
    //Helperfunction to get userId
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim is null ? Guid.Empty : Guid.Parse(userIdClaim.Value);
    }
    
}