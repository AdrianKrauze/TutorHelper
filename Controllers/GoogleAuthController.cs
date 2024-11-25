using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Entities;

[Route("api/[controller]")]
[ApiController]
public class GoogleAuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly SignInManager<User> _signInManager;

    public GoogleAuthController(IGoogleAuthService googleAuthService, SignInManager<User> signInManager)
    {
        _googleAuthService = googleAuthService;
        _signInManager = signInManager;
    }

    [HttpGet("signin-google")]
    public IActionResult SignInGoogle()
    {
        var redirectUrl = Url.Action(nameof(HandleExternalLogin), "GoogleAuth");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(properties, "Google");
    }

    [HttpGet("signin-google-callback")]
    public async Task<IActionResult> HandleExternalLogin()
    {
        var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            return RedirectToAction(nameof(SignInGoogle));
        }

        var jwtToken = await _googleAuthService.AuthenticateGoogleUserAsync(loginInfo);

       
        return Ok(new { Token = jwtToken });
    }
}