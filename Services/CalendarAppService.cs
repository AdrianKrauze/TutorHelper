using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities.OwnershipChecker;
using TutorHelper.Entities;
using TutorHelper.Middlewares.Exceptions;
using TutorHelper.Models;
using TutorHelper.Models.DtoModels.ToView;
using TutorHelper.Entities.DbContext;
using TutorHelper.Models.DtoModels;
using System.Linq;

namespace TutorHelper.Services
{
    public interface ICalendarAppService
    {
        Task<List<LessonObjectDto>> GetLessonListInDay(int year, int month, int day);

        Task<List<LessonObjectDto>> GetLessonListInWeek(int year, int month, int day);

        Task<List<PlaceholderLesson>> GetPlaceholderData();
        Task<LessonObjectDto> GetOneLessonData(string lessonid);
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


        public async Task<LessonObjectDto> GetOneLessonData(string lessonid)
        {
            string userId =  _userContextService.GetAuthenticatedUserId;
            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonid);
            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);

            var result = _mapper.Map<LessonObjectDto>(lesson);
            return result;
        }

        public async Task<List<PlaceholderLesson>> GetPlaceholderData()
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var listOfStudents = await _tutorHelperDb.Students
                .Where(x => x.CreatedById == userId && x.PlaceholderCourseData != null)
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.PlaceholderCourseData
                })
                .ToListAsync();

            DateTime startOfWeek = ReturnMonday();
            DateTime endOfWeek = startOfWeek.AddDays(7);

            var placeholderLessons = new List<PlaceholderLesson>();

            foreach (var student in listOfStudents)
            {
                bool hasLessonThisWeek = await BoolStudentLessonInThisWeek(startOfWeek, endOfWeek, student.Id);

                if (!hasLessonThisWeek)
                {
                    DateTime startDateTime = CalculateStartDate(student.PlaceholderCourseData.DayOfLesson, student.PlaceholderCourseData.LessonTime);
                    DateTime endDateTime = startDateTime.AddMinutes((int)student.PlaceholderCourseData.Duration);

                    placeholderLessons.Add(new PlaceholderLesson
                    {
                        studentId = student.Id,
                        Duration = (int)student.PlaceholderCourseData.Duration,
                        Summary = $"Tu powienien mieć lekcje uczeń {student.FirstName} {student.LastName}",
                        StartDate = startDateTime,
                        EndDate = endDateTime
                    });
                }

                bool hasLessonNextWeek = await BoolStudentLessonInThisWeek(startOfWeek.AddDays(7), endOfWeek.AddDays(7), student.Id);

                if (!hasLessonNextWeek)
                {
                    DateTime startDateTimeNextWeek = CalculateStartDate(student.PlaceholderCourseData.DayOfLesson, student.PlaceholderCourseData.LessonTime).AddDays(7);
                    DateTime endDateTimeNextWeek = startDateTimeNextWeek.AddMinutes((int)student.PlaceholderCourseData.Duration);

                    placeholderLessons.Add(new PlaceholderLesson
                    {
                        studentId = student.Id,
                        Duration = (int)student.PlaceholderCourseData.Duration,
                        Summary = $"Tu powienien mieć lekcje uczeń {student.FirstName} {student.LastName}",
                        StartDate = startDateTimeNextWeek,
                        EndDate = endDateTimeNextWeek
                    });
                }
            }

            return placeholderLessons;
        }


        public async Task<List<LessonObjectDto>> GetLessonListInWeek(int year, int month, int day)
        {
            ValidateData(year, month, day);

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

        public async Task<List<LessonObjectDto>> GetLessonListInDay(int year, int month, int day)
        {
            ValidateData(year, month, day);
            string userId = _userContextService.GetAuthenticatedUserId;

            var lessons = await _tutorHelperDb.Lessons
            .Where(x => x.Date.Year == year && x.Date.Month == month && x.Date.Day == day && x.CreatedById == userId)
                .Include(x => x.EduStage)
     .Include(x => x.LessonPlace)
     .Include(x => x.Subject)
     .Include(x => x.StudentCondition)
     .OrderBy(x => x.Date)
     .ToListAsync();

            var listToReturn = _mapper.Map<List<LessonObjectDto>>(lessons);

            return listToReturn;
        }
        #region private
        private DateTime ReturnMonday()
        {
            DateTime today = DateTime.Today;

            DateTime startOfThisWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            return startOfThisWeek;
        }

        private async Task<bool> BoolStudentLessonInThisWeek(DateTime start, DateTime end, string studentId)
        {
            return await _tutorHelperDb.Lessons
                            .OfType<LessonWithStudent>()
                            .AnyAsync(lesson => lesson.StudentId == studentId
                                             && lesson.Date >= start
                                             && lesson.Date <= end);
        }

        private DateTime CalculateStartDate(DayOfWeek day, TimeOnly timeSpan)
        {
            DateTime startOfWeek = ReturnMonday();

            DateTime lessonDate = startOfWeek.AddDays((double)day);

            DateTime startDateTime = lessonDate.Add(timeSpan.ToTimeSpan());

            return startDateTime;
        }

        private void ValidateData(int year, int month,  int day)
        {
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
            {
                throw new BadRequestException("The provided date parameters are not valid.");
            }
           
        }
        #endregion
    }
}