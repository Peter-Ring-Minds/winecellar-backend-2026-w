using Api.Auth;
using Api.Contracts.Auth;
using Api.Contracts.Cellar;
using Api.Contracts.Cellars;
using Domain;
using Infrastructure;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Api.Controllers;


[Route("api/[controller]")]
[ApiController] 

[Authorize]
public class CellarController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    public CellarController(UserManager<ApplicationUser> userManager, AppDbContext dbContext)
    {
        _context = dbContext;
        _userManager = userManager;
    }
    
  [HttpPost("add-wine")]
    public async Task<ActionResult> AddWine(AddCellarItemRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok();
    }

        //Get: api/Cellars
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CellarSummary>>> GetAllCellars()
    {
        var user = await _userManager.GetUserAsync(User);
        
        /*if (user is null)
                {
            return Unauthorized();
        }*/
    return await _context.Cellars
        .Where(c => c.UserId == user.Id)
        .Select(c => new CellarSummary(c))
        .ToListAsync();

    }


}