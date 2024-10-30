using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TutorHelper.Entities;
using TutorHelper.Models.ConfigureClasses;
using TutorHelper.Services;

public interface IGoogleAuthService
{
    Task<string> AuthenticateGoogleUserAsync(ExternalLoginInfo loginInfo);
    Task UpdateUserTokensAsync(User user, string accessToken, string refreshToken, DateTime expiration);
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IAccountService _accountService;
    private readonly AuthenticationSettings _authenticationSettings;

    public GoogleAuthService(UserManager<User> userManager, SignInManager<User> signInManager, IAccountService accountService, AuthenticationSettings authenticationSettings)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _accountService = accountService;
        _authenticationSettings = authenticationSettings;

    }

    public async Task<string> AuthenticateGoogleUserAsync(ExternalLoginInfo loginInfo)
    {
        // Sprawdzenie, czy loginInfo nie jest null
        if (loginInfo == null)
        {
            throw new ArgumentNullException("Login information cannot be null.");
        }

        // Pobieranie adresu email z Claims
        var email = loginInfo.Principal?.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            throw new Exception("Email not found in the login information.");
        }

        // Sprawdzenie, czy istnieje użytkownik o podanym e-mailu
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            // Sprawdzenie, czy Google login jest już powiązany
            var userLogins = await _userManager.GetLoginsAsync(existingUser);
            var googleLogin = userLogins.FirstOrDefault(x => x.LoginProvider == "Google");

            // Jeśli Google login jest już powiązany, generuj nowy token i zwróć go
            if (googleLogin != null)
            {
                var accessToken = await _accountService.GenerateJwtTokenAsync(existingUser);
                var refreshToken = GenerateRefreshToken();
                var expiration = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays);

                await UpdateUserTokensAsync(existingUser, accessToken, refreshToken, expiration);
                return accessToken; // Zwróć istniejący token dostępu
            }

            // Jeśli użytkownik istnieje, ale nie ma Google loginu, dodaj go
            var addLoginResult = await _userManager.AddLoginAsync(existingUser, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                var errors = string.Join(", ", addLoginResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to add Google login to user. Errors: {errors}");
            }

            // Generowanie tokenu dla istniejącego użytkownika
            var token = await _accountService.GenerateJwtTokenAsync(existingUser);
            return token; // Zwróć nowy token
        }

        // Jeśli użytkownik nie istnieje, stwórz nowego użytkownika
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

        // Dodanie Google loginu do nowego użytkownika
        var addLoginResultNewUser = await _userManager.AddLoginAsync(newUser, loginInfo);
        if (!addLoginResultNewUser.Succeeded)
        {
            var errors = string.Join(", ", addLoginResultNewUser.Errors.Select(e => e.Description));
            throw new Exception($"Failed to add Google login to new user. Errors: {errors}");
        }

        // Generowanie tokenu dla nowego użytkownika
        var newAccessToken = await _accountService.GenerateJwtTokenAsync(newUser);
        var newRefreshToken = GenerateRefreshToken();
        var newExpiration = DateTime.UtcNow.AddDays(_authenticationSettings.RefreshTokenExpireDays); // Czas ważności refresh tokena

        await UpdateUserTokensAsync(newUser, newAccessToken, newRefreshToken, newExpiration);

        return newAccessToken; // Zwróć token dla nowego użytkownika
    }



    public async Task UpdateUserTokensAsync(User user, string accessToken, string refreshToken, DateTime expiration)
    {
        // Zapisz tokeny w bazie danych
        await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", refreshToken);
        await _userManager.SetAuthenticationTokenAsync(user, "Default", "AccessToken", accessToken);

        // Zaktualizuj datę wygaśnięcia
        user.TokenExpiration = expiration;
        await _userManager.UpdateAsync(user);
    }

    private string GenerateRefreshToken()
    {
        // Prosta metoda do generowania refresh tokena, można to rozbudować
        var randomBytes = new byte[32];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
