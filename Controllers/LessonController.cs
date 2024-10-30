using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Models;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Models.DtoModels.ToView;
using TutorHelper.Models.DtoModels.UpdateModels;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly TutorHelperDb _db;
        private readonly IUserContextService _userContextService;

        public LessonController(ILessonService lessonService, TutorHelperDb db, IUserContextService userContextService)
        {
            _lessonService = lessonService;
            _db = db;
            _userContextService = userContextService;
        }

        // -------------------- POST ENDPOINTS -------------------- //

        [HttpPost("create/with-student")]
        public async Task<IActionResult> CreateLessonByStudentId([FromBody] CreateLessonDtoWithStudent dto)
        {
            await _lessonService.CreateLessonWithStudentAsync(dto, false);
            return Ok();
        }

        [HttpPost("create/without-student")]
        public async Task<IActionResult> CreateLessonWithoutStudent([FromBody] CreateLessonDtoWoStudent dto)
        {
            await _lessonService.CreateLessonWithoutStudentAsync(dto, false);
            return Ok();
        }

        [HttpPost("create/with-student/google")]
        public async Task<IActionResult> CreateLessonByStudentIdGoogle([FromBody] CreateLessonDtoWithStudent dto)
        {
            await _lessonService.CreateLessonWithStudentAsync(dto, true);
            return Ok();
        }

        [HttpPost("create/without-student/google")]
        public async Task<IActionResult> CreateLessonByStudentIdGoogle([FromBody] CreateLessonDtoWoStudent dto)
        {
            await _lessonService.CreateLessonWithoutStudentAsync(dto, true);
            return Created();
        }

        // -------------------- PATCH ENDPOINTS -------------------- //

        [HttpPatch("{lessonId}/update/with-student")]
        public async Task<IActionResult> EditLessonWithStudent([FromBody] UpdateLessonWithStudentDto dto, [FromRoute] string lessonId)
        {
            await _lessonService.UpdateLessonWithStudentAsync(lessonId, dto);
            return Ok();
        }

        [HttpPatch("{lessonId}/update/with-student/group")]
        public async Task<IActionResult> EditLessonWithStudentGroup([FromBody] UpdateLessonWithStudentDto dto, [FromRoute] string lessonId)
        {
            await _lessonService.UpdateLessonWithStudentGroupAsync(lessonId, dto);
            return Ok("Lesson updated successfully.");
        }

        [HttpPatch("{lessonId}/update/without-student/group")]
        public async Task<IActionResult> EditLessonWithoutStudentGroup([FromBody] UpdateLessonWithoutStudentDto dto, [FromRoute] string lessonId)
        {
            await _lessonService.UpdateLessonWithoutStudentGroupAsync(lessonId, dto);
            return Ok("Lessons updated successfully.");
        }

        [HttpPatch("{lessonId}/update/without-student")]
        public async Task<IActionResult> EditLessonWithoutStudent([FromBody] UpdateLessonWithoutStudentDto dto, [FromRoute] string lessonId)
        {
            await _lessonService.UpdateLessonWithoutStudentAsync(lessonId, dto);
            return Ok();
        }

        // -------------------- DELETE ENDPOINTS -------------------- //

        [HttpDelete("{lessonId}")]
        public async Task<IActionResult> DeleteLesson(string lessonId)
        {
            await _lessonService.DeleteLesson(lessonId);
            return NoContent(); // Zwróć 204 No Content, co oznacza sukces
        }

        [HttpDelete("{lessonId}/group")]
        public async Task<IActionResult> DeleteLessonGroup(string lessonId)
        {
            await _lessonService.DeleteLessonGroup(lessonId);
            return NoContent(); // Zwróć 204 No Content, co oznacza sukces
        }
    }
}