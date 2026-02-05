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


}