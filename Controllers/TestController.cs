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
    }
}