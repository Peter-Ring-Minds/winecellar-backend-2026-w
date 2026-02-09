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
    public async Task<ActionResult<StorageUnitContract>> PostStorageUnit(StorageUnitContract storageUnitContract)
    {
        var storageUnit = new Domain.StorageUnit
        {
            StorageUnitId = Guid.NewGuid(),
            CellarId = storageUnitContract.CellarId,
            StorageUnitName = storageUnitContract.Name
        };

        _context.StorageUnits.Add(storageUnit);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetStorageUnit),
            new { id = storageUnit.StorageUnitId },
            storageUnitContract);
    }

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