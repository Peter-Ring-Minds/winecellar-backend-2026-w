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
    public  async Task<ActionResult<WineContract>> PostWine(WineContract request)
    {
        var wine = new Domain.Wine
        {
            Id = Guid.NewGuid(),
            StorageUnitId = request.StorageUnitId,
            Name = request.Name,
            Wineyard = request.Wineyard,
            Type = request.Type,
            Vintage = request.Vintage,
            UserId = GetCurrentUserId()
        };

        _context.Wines.Add(wine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetWine),
            new { id = wine.Id },
            CreateWineContract(wine));
    }

    [HttpGet]
    public async Task<ActionResult<List<WineContract>>> GetWines()
    {
        var userId = GetCurrentUserId();
        var wines = await _context.Wines
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return Ok(wines);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WineContract>> GetWine(Guid id)
    {
        var userId = GetCurrentUserId();
        var wine = await _context.Wines
            .Where(w => w.Id == id && w.UserId == userId)
            .FirstOrDefaultAsync();
        if (wine is null)
        {
            return NotFound("Wine not found for the current user.");
        }
        return Ok(CreateWineContract(wine));
    }



    //Helper function for building WineContract from Domain.Wine
    private WineContract CreateWineContract(Domain.Wine wine)
    {
        return new WineContract
        {
            WineId = wine.Id,
            StorageUnitId = wine.StorageUnitId ?? Guid.Empty,
            Name = wine.Name,
            Wineyard = wine.Wineyard,
            Type = wine.Type,
            Vintage = wine.Vintage
        };
    }

    //Helperfunction to get userId
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return userIdClaim is null ? Guid.Empty : Guid.Parse(userIdClaim.Value);
    }

}
