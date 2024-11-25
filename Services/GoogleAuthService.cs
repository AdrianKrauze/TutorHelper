using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TutorHelper.Entities;
using TutorHelper.Models.ConfigureClasses;
using TutorHelper.Services;

public interface IGoogleAuthService
{
    Task<string> AuthenticateGoogleUserAsync(ExternalLoginInfo loginInfo);
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;

    public GoogleAuthService(UserManager<User> userManager, SignInManager<User> signInManager, IAccountService accountService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
    }

    public async Task<string> AuthenticateGoogleUserAsync(ExternalLoginInfo loginInfo)
    {
        if (loginInfo == null)
        {
            throw new ArgumentNullException("Login information cannot be null.");
        }

        var email = loginInfo.Principal?.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            throw new Exception("Email not found in the login information.");
        }

        // Check if the user already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            // Check if the Google login is already associated with the user
            var userLogins = await _userManager.GetLoginsAsync(existingUser);
            var googleLogin = userLogins.FirstOrDefault(x => x.LoginProvider == "Google");

            if (googleLogin != null)
            {
                // Generate JWT for the existing user
                return await _accountService.GenerateJwtTokenAsync(existingUser);
            }

            // Add the Google login to the existing user
            var addLoginResult = await _userManager.AddLoginAsync(existingUser, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to add Google login to user. Errors: {errors}");
            }

            // Generate JWT for the existing user
            return await _accountService.GenerateJwtTokenAsync(existingUser);
        }

        // Create a new user if it does not exist
        var newUser = new User
        {
            UserName = email,
            Email = email,
            FirstName = loginInfo.Principal?.FindFirstValue(ClaimTypes.GivenName),
            LastName = loginInfo.Principal?.FindFirstValue(ClaimTypes.Surname),
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"Failed to create user. Errors: {errors}");
        }

        // Add Google login to the new user
        var addLoginResultNewUser = await _userManager.AddLoginAsync(newUser, loginInfo);
        if (!addLoginResultNewUser.Succeeded)
        {
            var errors = string.Join(", ", addLoginResultNewUser.Errors.Select(e => e.Description));
            throw new Exception($"Failed to add Google login to new user. Errors: {errors}");
        }

        // Generate JWT for the new user
        return await _accountService.GenerateJwtTokenAsync(newUser);
    }
}
