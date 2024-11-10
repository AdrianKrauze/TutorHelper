using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities.OwnershipChecker;
using TutorHelper.Entities;
using TutorHelper.Middlewares.Exceptions;
using TutorHelper.Models;
using TutorHelper.Models.DtoModels.ToView;
using TutorHelper.Entities.DbContext;
using TutorHelper.Models.DtoModels;

namespace TutorHelper.Services
{
    public interface ICalendarAppService
    {
        Task<Dictionary<DateTime, int>> GetLessonInMonthCount(int year, int month);
        Task<List<LessonObjectDto>> GetLessonListInDay(int year, int month, int day);
        Task<List<LessonObjectDto>> GetLessonListInWeek(int year, int month, int day);
      
    }

    public class CalendarAppService : ICalendarAppService
    {
        private readonly IMapper _mapper;
        private readonly TutorHelperDb _tutorHelperDb;
        private readonly IUserContextService _userContextService;

        public CalendarAppService(TutorHelperDb tutorHelperDb, IUserContextService userContextService, IMapper mapper)
        {
            _tutorHelperDb = tutorHelperDb;
            _userContextService = userContextService;
            _mapper = mapper;
        }


        public async Task<List<PlaceholderLesson>> GetPlaceholderData()
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var data = await _tutorHelperDb.Students
                .Select(x => new {x.Id, x.PlaceholderCourseData, x.FirstName, x.LastName})
                .ToListAsync();  

            var result = data.Select(x =>
            {
                DateTime startDate = ReturnStartOfWeekOrStartOfNextWeek(x.PlaceholderCourseData.DayOfLesson)
                    .AddHours(x.PlaceholderCourseData.DateTime.Hour)
                    .AddMinutes(x.PlaceholderCourseData.DateTime.Minute);

                DateTime endDate = startDate.AddMinutes(x.PlaceholderCourseData.Duration);

                return new PlaceholderLesson
                {
                    studentId = x.Id,
                    Duration = (int)x.PlaceholderCourseData.Duration,
                    Summary = $"Tu powinien mieć lekcje {x.FirstName} {x.LastName}",
                    StartDate = startDate,
                    EndDate = endDate
                };
            }).ToList();  // Konwertujemy na listę

            return result;
        }





        public async Task<Dictionary<DateTime, int>> GetLessonInMonthCount(int year, int month)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            // Walidacja roku i miesiąca
            if (year < 1900 || year > DateTime.Now.Year || month < 1 || month > 12)
            {
                throw new BadRequestException("The provided date parameters are not valid.");
            }

            var query = await _tutorHelperDb.Lessons
                .Where(l => l.CreatedById == userId && l.Date.Year == year && l.Date.Month == month)
                .ToListAsync();

            var lessonsGroupedByDay = query
                .GroupBy(l => l.Date.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            return lessonsGroupedByDay;
        }

        public async Task<List<LessonObjectDto>> GetLessonListInDay(int year, int month, int day)
        {
            // Walidacja daty
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
            {
                throw new BadRequestException("The provided date parameters are not valid.");
            }

            string userId = _userContextService.GetAuthenticatedUserId;

            var date = new DateTime(year, month, day, 0, 0, 0);

            // Pobranie lekcji z danego dnia
            var lessonsListInDay = await _tutorHelperDb.Lessons
                .Where(l => l.CreatedById == userId && l.Date.Year == year && l.Date.Month == month && l.Date.Day == day)
                .Include(x => x.EduStage)
                .Include(x => x.LessonPlace)
                .Include(x => x.Subject)
                .Include(x => x.StudentCondition)
                .ToListAsync();


            // Sortowanie lekcji, np. z flagą HasStudent, jeśli to potrzebne
            var sortedLessons = lessonsListInDay.OrderBy(x => x.HasStudent).ThenBy(x => x.Date).ToList();

            // Mapowanie list na DTO
            var mappedList = _mapper.Map<List<LessonObjectDto>>(sortedLessons);

            return mappedList;
        }

        public async Task<List<LessonObjectDto>> GetLessonListInWeek(int year, int month, int day)
        {
            // Validate year, month, and day to ensure they form a valid date
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
            {
                throw new BadRequestException("The provided date parameters are not valid.");
            }

            string userId = _userContextService.GetAuthenticatedUserId;

            var date = new DateTime(year, month, day, 0, 0, 0);

            var dayOfWeek = (int)date.DayOfWeek;

            var difference = dayOfWeek == 0 ? -6 : 1 - dayOfWeek;
            var startOfWeek = date.AddDays(difference).Date;
            var endOfWeek = startOfWeek.AddDays(7);

            var lessons = await _tutorHelperDb.Lessons
                .Where(x => x.Date >= startOfWeek && x.Date < endOfWeek && x.CreatedById == userId)
                .Include(x => x.EduStage)
                .Include(x => x.LessonPlace)
                .Include(x => x.Subject)
                .Include(x => x.StudentCondition)
                .OrderBy(x => x.Date)
                .ToListAsync();

            var listToReturn = _mapper.Map<List<LessonObjectDto>>(lessons);

            return listToReturn;
        }

        private DateTime ReturnStartOfWeekOrStartOfNextWeek(DayOfWeek dayOfWeek)
        {
            DateTime today = DateTime.Today;
            
            DateTime startOfThisWeek = today.AddDays(-(int)today.DayOfWeek+(int)DayOfWeek.Monday);

            int daysUntilTargetDay = (int)dayOfWeek - (int)today.DayOfWeek;

            if (daysUntilTargetDay >= 0)
            {
                return startOfThisWeek.AddDays(daysUntilTargetDay);
            }
            else
            {

                return startOfThisWeek.AddDays(7+daysUntilTargetDay);


            }
        }
       
    }
}