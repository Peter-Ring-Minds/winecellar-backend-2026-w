using Api.Contracts.Cellar;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using System.Security.Claims;

namespace Api.Controllers;


[ApiController] 
[Route("api/[controller]")]
[Authorize]
public class CellarController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public CellarController(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _context = context;
        _userManager = userManager;
    }
    


    [HttpPost]
    public async Task<ActionResult<CellarContract>> PostCellar(CellarContract cellarContract)
    {
        var cellar = new Domain.Cellar
        {
            CellarId = cellarContract.CellarId,
            UserId = cellarContract.UserId,

        };

        _context.Cellars.Add(cellar);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCellar),
            new { id = cellar.CellarId },
            cellarContract);
    }


    [HttpGet("GetCellars")]
    public async Task<ActionResult<IEnumerable<CellarContract>>> GetCellars()
    {
        var userId = GetCurrentUserId();
        var allUserCellars = await _context.Cellars
            .Where(x => x.UserId == userId)
            .Select(x => new CellarContract(x.CellarId, x.UserId))
            .ToListAsync();
        if (allUserCellars is null || !allUserCellars.Any())
        {
            return NotFound();
        }
        return Ok(allUserCellars);
    }

        [HttpGet("GetCellar/{id}")]
    public async Task<ActionResult<CellarContract>> GetCellar(Guid id)
    {
        var userId = GetCurrentUserId();
        var cellar = await _context.Cellars
            .Where(x => x.CellarId == id && x.UserId == userId)
            .FirstOrDefaultAsync();
        if (cellar is null )
        {
            return NotFound();
        }
        return Ok(new CellarContract(cellar.CellarId, cellar.UserId));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCellar(Guid id, CellarContract cellarContract)
    {
        var userId = GetCurrentUserId();
        var cellar = await _context.Cellars
            .Where(x => x.CellarId == id && x.UserId == userId)
            .FirstOrDefaultAsync();

        if (cellar is null)
        {
            return NotFound();
        }
        if (cellar.CellarId != id)
        {
            return BadRequest();
        }
        cellar.UserId = cellarContract.UserId;
        cellar.CellarId = cellarContract.CellarId;

        try
        {
            _context.Entry(cellar).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!CellarExists(id))
        {
            return NotFound();
        }
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCellar(Guid id)
    {
        var userId = GetCurrentUserId();
        var cellar = await _context.Cellars
            .Where(x => x.CellarId == id && x.UserId == userId)
            .FirstOrDefaultAsync();
        try
        {
            if (cellar is null)
            {
                return NotFound();
            }
             _context.Cellars.Remove(cellar);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (CellarExists(id))
        {
            return NotFound("The cellar wasn't deleted. Consult a developer. Or a wizard");
        }

        return NoContent();    
    }    



    //Helper method to get the current user's ID from Claims
    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new InvalidOperationException("Invalid or missing user ID in claims.");
        }
        return userId;
    }

    //Helper method to check if a cellar exists user
    private bool CellarExists(Guid id)
    {
        return _context.Cellars.Any(e => e.CellarId == id);
    }
}