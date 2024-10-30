using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TutorHelper.Entities;
using TutorHelper.Models.ConfigureClasses;
using TutorHelper.Models.IdentityModels;

namespace TutorHelper.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model, string userId);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string code);
        Task<IdentityResult> DeleteUserAsync();
        Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordModel model);
        Task<string> GenerateJwtTokenAsync(User user);
        Task<string> RefreshAccessTokenAsync(string userId);
        Task<IdentityResult> RegisterAsync(RegisterModel model);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model);
        Task<string> SignInAsync(LoginModel model);
        Task SignOutAsync();
    }

    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserContextService _userContextService;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, AuthenticationSettings authenticationSettings, IUserContextService userContextService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _authenticationSettings = authenticationSettings;
            _userContextService = userContextService;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
               
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> RefreshAccessTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.TokenExpiration <= DateTime.UtcNow)
            {
                throw new Exception("Refresh token expired.");
            }

            string newAccessToken = await GenerateJwtTokenAsync(user);
            string newRefreshToken = GenerateRefreshToken();
            user.TokenExpiration = DateTime.UtcNow.AddDays(_authenticationSettings.RefreshTokenExpireDays);
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", newRefreshToken);
            await _userManager.UpdateAsync(user);

            return newAccessToken;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public async Task<string> SignInAsync(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                return await GenerateJwtTokenAsync(user);
            }

            throw new Exception("Invalid login attempt.");
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<IdentityResult> DeleteUserAsync()
        {
            var userId = _userContextService.GetAuthenticatedUserId;
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourdomain.com/reset-password?token={token}&email={model.Email}";

            await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: {resetLink}");
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);
        }

        
        


        public async Task<IdentityResult> RegisterAsync(RegisterModel model)
        {
            var user = new User { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"https://yourdomain.com/confirm-email?userId={user.Id}&token={token}";
                await _emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your email by clicking here: {confirmationLink}");
            }

            return result;
        }
    }
}
