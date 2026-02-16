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
            CreateStorageUnitResponse(storageUnit));
    }

    [HttpGet]
    public async Task<ActionResult<List<StorageUnitResponse>>> GetStorageUnits()
    {

        var userId = GetCurrentUserId();
        var userStorageUnits = await _context.StorageUnits
            .Where(su => su.UserId == userId)
            .ToListAsync();

        return Ok(userStorageUnits.Select(su => CreateStorageUnitResponse(su)));
    }

    //Get StorageUnits by CellarId
    [HttpGet("by-cellar/{cellarId}")]
    public async Task<ActionResult<List<StorageUnitResponse>>> GetStorageUnitsByCellar(Guid cellarId)
    {
        var userId = GetCurrentUserId();
        var storageUnits = await _context.StorageUnits
            .Where(su => su.CellarId == cellarId && su.UserId == userId)
            .ToListAsync();
        return Ok(storageUnits.Select(su => CreateStorageUnitResponse(su)));
    }

    //Get StorageUnit by StorageUnitId
    [HttpGet("{id}")]
    public async Task<ActionResult<StorageUnitResponse>> GetStorageUnit(Guid id)
    {
        var userId = GetCurrentUserId();
        var storageUnit = await _context.StorageUnits
        .Where(x => x.Id == id && x.UserId == userId)
        .FirstOrDefaultAsync();
        if (storageUnit is null)
        {
            return NotFound();
        }

        return Ok(CreateStorageUnitResponse(storageUnit));
    }       
    // Put EndPoint to change name of storage unit. Maybe could also add functionality to move storage unit to another cellar by changing CellarId?
    [HttpPut("{id}")]
    public async Task<ActionResult<StorageUnitResponse>> UpdateStorageUnit(Guid id, StorageUnitContract request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Name is required.");
        }

        if (request.CellarId == Guid.Empty)
        {
            return BadRequest("CellarId is required.");
        }

        var trimmedName = request.Name.Trim();
        if (trimmedName.Length > 30)
        {
            return BadRequest("Name can not be longer than 30 characters.");
        }

        var userId = GetCurrentUserId();
        var normalizedName = trimmedName.ToLowerInvariant();
        var hasDuplicateName = await _context.StorageUnits
            .Where(su => su.UserId == userId && su.CellarId == request.CellarId && su.Id != id)
            .AnyAsync(su => su.Name.ToLower() == normalizedName);

        if (hasDuplicateName)
        {
            return Conflict("A storage unit with this name already exists in the cellar.");
        }

        var storageUnit = await _context.StorageUnits
            .Where(x => x.Id == id && x.UserId == userId && x.CellarId == request.CellarId)
            .FirstOrDefaultAsync();

        if (storageUnit is null)
        {
            return NotFound();
        }

        storageUnit.Name = trimmedName;
        await _context.SaveChangesAsync();

        return Ok(CreateStorageUnitResponse(storageUnit));
    }

 
    //Helper function to build StorageUnitResponse from StorageUnit
    private StorageUnitResponse CreateStorageUnitResponse(Domain.StorageUnit storageUnit)
    {        return new StorageUnitResponse
        {
            Name = storageUnit.Name,
            StorageUnitId = storageUnit.Id,
            CellarId = storageUnit.CellarId
        };
    }
    
    //Helperfunction to get userId
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim is null ? Guid.Empty : Guid.Parse(userIdClaim.Value);
    }
    
}