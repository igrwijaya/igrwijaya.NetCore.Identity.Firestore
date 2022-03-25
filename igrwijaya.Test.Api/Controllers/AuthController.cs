using igrwijaya.NetCore.Identity.Firestore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace igrwijaya.Test.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<FirestoreIdentityRole> _roleManager;

    // GET
    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<FirestoreIdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Register()
    {
        var result = await _userManager.CreateAsync(new ApplicationUser("igrwijaya", "igrwijaya@test.com"), "Temp123*");

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> CreateRole()
    {
        await _roleManager.CreateAsync(new FirestoreIdentityRole("ADMIN"));
        
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