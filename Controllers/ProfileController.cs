using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Models.DtoModels.Profile;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpPost]
        [Route("update-subjects")]
        public async Task<ActionResult> UpdateSubjects([FromBody] List<string> subjects)
        {
            await _profileService.UpdateTeacherSubjectsAsync(subjects);
            return Ok("Subjects updated successfully.");
        }

        [HttpGet]
        [Route("get-email-state")]
        public async Task<ActionResult<string>> GetEmailState()
        {
            var result = await _profileService.GetEmailStateAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("get-user-subjects")]
        public async Task<ActionResult<List<UserSubTaughtDto>>> GetUserSubTaught()
        {
            var result = await _profileService.GetUserSubTaughtByUserIdAsync();
            return Ok(result);
        }
    }
}
