using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace igrwijaya.Test.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    // GET
    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var result = await _userManager.CreateAsync(new ApplicationUser("igrwijaya"), "Temp123*");

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var user = await _userManager.FindByNameAsync("igrwijaya");

        var result = await _signInManager.PasswordSignInAsync(user, "Temp123*", false, false);

        return Ok(result);
    }
}