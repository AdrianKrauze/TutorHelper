using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Models.GoogleCalendarModels;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarAppService _calendarService;
        private readonly IGoogleCalendarApi _googleCalendarApi;

        public CalendarController(ICalendarAppService calendarService, IGoogleCalendarApi googleCalendarApi)
        {
            _calendarService = calendarService;
            _googleCalendarApi = googleCalendarApi;
        }

        [HttpGet]
        [Route("get-google-data")]
        public async Task<List<PlaceholderEvent>> getGoogleData()
        {
            return await _googleCalendarApi.GetCalendarData();
        }

        [HttpGet("refresh-calendar-data")]
        public async Task<IActionResult> RefreshCalendarData()
        {
            var refreshedCalendarData = await _googleCalendarApi.GetCalendarData(true);
            return Ok(refreshedCalendarData);
        }

        [HttpGet]
        [Route("month/{year}/{month}")]
        public async Task<IActionResult> GetLessonInMonth([FromRoute] int year, [FromRoute] int month)
        {
            var result = await _calendarService.GetLessonInMonthCount(year, month);

            return Ok(result);
        }

        [HttpGet]
        [Route("month/{year}/{month}/{day}")]
        public async Task<IActionResult> GetLessonInDay([FromRoute] int year, [FromRoute] int month, [FromRoute] int day)
        {
            var result = await _calendarService.GetLessonListInDay(year, month, day);

            return Ok(result);
        }

        [HttpGet]
        [Route("weekly/{year}/{month}/{day}")]
        public async Task<IActionResult> GetLessonsInWeek([FromRoute] int year, [FromRoute] int month, [FromRoute] int day)
        {
            var lessons = await _calendarService.GetLessonListInWeek(year, month, day);

            return Ok(lessons);
        }

        [HttpGet]
        [Route("lessonplaceholders")]
        public async Task<IActionResult> GetPlaceholderLessons()
        {
            var result = await _calendarService.GetPlaceholderData();
            return Ok(result);
        }
    }
}