using Api.Auth;
using Api.Contracts.Auth;
using Api.Contracts.Cellar;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;


[ApiController] 
[Route("api/[controller]")]
[Authorize]
public class CellarController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CellarController(UserManager<ApplicationUser> userManager)
    {
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

        // ToDo: Implement adding a wine to a cellar's storage unit

        
    }

}