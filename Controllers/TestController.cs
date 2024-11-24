using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [Authorize]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestController(ITestService Its)
        {
            _testService = Its;
        }

        [HttpGet]
        [Route("getId")]
        public IActionResult GetId()
        {
            return Ok(_testService.returnLoggedId());
        }

        [HttpGet]
        [Route("checkAuth")]
        public IActionResult CheckAuth()
        {
            var isAuthenticated = User.Identity.IsAuthenticated;
            return Ok(isAuthenticated);
        }

        [HttpGet]
        [Route("getClaims")]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);

        }


        [AllowAnonymous]
        [HttpGet]
        [Route("generateUsers")]
        public async Task<IActionResult> GenerateUsers()
        {
            try
            {
                // Wywołanie metody generującej dane
                await _testService.UseDataGenerator();

                // Zwracanie odpowiedzi po pomyślnym wykonaniu metody
                return Ok("Users generated successfully.");
            }
            catch (Exception ex)
            {
                // Obsługa błędów i zwrócenie informacji o błędzie
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("googledata")]
        public IActionResult GetGoogleData()
        {
            string result = _testService.ReturnGoogleData();
            return Ok(result);
        }
        
       
        [HttpGet]
        [Route("generateUsersFLI")]
        public async Task<IActionResult> GenerateUsersFLI()
        {
            try
            {
                // Wywołanie metody generującej dane
                await _testService.GenerateDataForLoggedUser();

                // Zwracanie odpowiedzi po pomyślnym wykonaniu metody
                return Ok("Users generated successfully.");
            }
            catch (Exception ex)
            {
                // Obsługa błędów i zwrócenie informacji o błędzie
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}