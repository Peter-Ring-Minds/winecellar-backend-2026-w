using Api.Contracts.Cellar;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
    public async Task<ActionResult<CellarContract>> PostTodoItem(CellarContract cellarContract)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

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
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

            return await _context.Cellars
            .Select(x => new CellarContract(x.CellarId, x.UserId))
            .ToListAsync();
    }

        [HttpGet("GetCellar/{id}")]
    public async Task<ActionResult<CellarContract>> GetCellar(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)        {
            return Unauthorized();
        }
        var cellar = await _context.Cellars.FindAsync(id);
        if (cellar is null)
        {
            return NotFound();
        }
        return Ok(new CellarContract(cellar.CellarId, cellar.UserId));
    }

}