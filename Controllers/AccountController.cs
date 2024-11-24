using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Entities;
using TutorHelper.Models.GoogleCalendarModels;
using TutorHelper.Models.IdentityModels;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager;


        public AccountController(IAccountService accountService, UserManager<User> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _accountService.RegisterAsync(model);
            if (result.Succeeded)
                return Ok(new { Message = "User registered successfully. Please check your email to confirm your account." });

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Invalid login attempt.");
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized("Invalid login attempt. Email Dto");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCheck)
            {
                return Unauthorized("Invalid login attempt. Password");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Unauthorized("Invalid login attempt. Not Email Confirmed");
            }

            var token = await _accountService.GenerateJwtTokenAsync(user);
            return Ok(new { Token = token });
        }
        

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync();
            Response.Cookies.Delete("refreshToken");
            return Ok(new { Message = "Logged out successfully." });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("User not found.");

            var result = await _accountService.ChangePasswordAsync(model, userId);
            if (result.Succeeded)
                return Ok(new { Message = "Password changed successfully." });

            return BadRequest(result.Errors);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var result = await _accountService.ForgotPasswordAsync(model);
            if (result.Succeeded)
                return Ok(new { Message = "Password reset instructions sent to your email." });

            return BadRequest("Error sending password reset instructions.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _accountService.ResetPasswordAsync(model);
            if (result.Succeeded)
                return Ok(new { Message = "Password has been reset successfully." });

            return BadRequest(result.Errors);
        }
        [Authorize]
        [HttpGet("secure-data")]
        public async Task<IActionResult> GetSecureData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByIdAsync(userId);


            if (user.TokenExpiration < DateTime.UtcNow)
            {
                return Unauthorized("Token has expired.");
            }

          
            return Ok(new { Data = "This is secure data.", Time = user.TokenExpiration });
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var decodedCode = WebUtility.UrlDecode(token);
           decodedCode = decodedCode.Replace(" ", "+");
            var result = await _accountService.ConfirmEmailAsync(userId, decodedCode);

            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }

            return BadRequest("Email confirmation failed.");
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            var result = await _accountService.DeleteUserAsync();
            if (result.Succeeded)
                return Ok(new { Message = "Account deleted successfully." });

            return BadRequest();
        }

        [Authorize]
        [HttpPost("generate-refresh-token")]
        public async Task<IActionResult> GenerateRefreshToken()
        {

            var token = _accountService.RefreshAccessTokenAsync();
            return Ok(token);

        }
        
    }
}
