using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities;
using TutorHelper.Entities.OwnershipChecker;
using TutorHelper.Middlewares.Exceptions;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Models.DtoModels.UpdateModels;
using TutorHelper.Models.GoogleCalendarModels;

namespace TutorHelper.Services
{
    public interface ILessonService
    {
        Task CreateLessonWithoutStudentAsync(CreateLessonDtoWoStudent dto, bool addToGoogle);

        Task CreateLessonWithStudentAsync(CreateLessonDtoWithStudent dto, bool addToGoogle);

        Task DeleteLesson(string lessonId);

        Task DeleteLessonGroup(string lessonId);


        Task UpdateLessonWithoutStudentAsync(string lessonId, UpdateLessonWithoutStudentDto dto);

        Task UpdateLessonWithoutStudentGroupAsync(string lessonId, UpdateLessonWithoutStudentDto dto);

        Task UpdateLessonWithStudentAsync(string lessonId, UpdateLessonWithStudentDto dto);

        Task UpdateLessonWithStudentGroupAsync(string lessonId, UpdateLessonWithStudentDto dto);
    }

    public class LessonService : ILessonService
    {
        private readonly TutorHelperDb _tutorHelperDb;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;
        private readonly IGoogleCalendarApi _googleCalendarApi;
        private readonly IAccountService _accountService;

        public LessonService(TutorHelperDb tutorHelperDb,
            IUserContextService userContextService,
            IMapper mapper,
            IAccountService accountService,
            IGoogleCalendarApi googleCalendarApi)
        {
            _tutorHelperDb = tutorHelperDb;
            _userContextService = userContextService;
            _mapper = mapper;
            _accountService = accountService;
            _googleCalendarApi = googleCalendarApi;
        }

        public async Task CreateLessonWithStudentAsync(CreateLessonDtoWithStudent dto, bool addToGoogle)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var student = await _tutorHelperDb.Students
                .Include(s => s.EduStage)
                .Include(s => s.Subject)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId);

            DataValidationMethod.OwnershipAndNullChecker(student, userId);

            var listToAdd = new List<LessonWithStudent>();
            var lessonGroupId = Guid.NewGuid().ToString();

            for (int i = 0; i < (dto.Repeat ? dto.RepeatCount : 1); i++)
            {
                var lesson = _mapper.Map<LessonWithStudent>(dto);

                lesson.Date = dto.Date.AddDays(7 * i);

                lesson.StudentFirstName = student.FirstName;
                lesson.StudentLastName = student.LastName;
                if (dto.Repeat)
                {
                    lesson.LessonGroupId = lessonGroupId;
                }

                lesson.StudentId = student.Id;
                lesson.EduStageId = student.EduStageId;
                lesson.SubjectId = student.SubjectId;
                lesson.CreatedById = userId;
                lesson.LessonPlaceId = student.LessonPlaceId;
                lesson.StudentConditionId = student.StudentConditionId;
                lesson.ContactTips = student.ContactTips;
                lesson.PhoneNumber = student.PhoneNumber;
                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration);
                lesson.IsGrouped = dto.Repeat;
                lesson.HasStudent = true;
                lesson.IsSyncedWithGoogle = addToGoogle;

                var calculatedPrice = student.PricePerHour * (dto.Duration / 60) + (float)student.PricePerDrive;

                if (dto.PushBoolean)
                {
                    lesson.PushBoolean = dto.PushBoolean;
                    lesson.WhenPushBeforeLesson = lesson.Date.AddMinutes(-dto.PushTimeBeforeLesson);
                }
                if (dto.Price == null)
                {
                    lesson.Price = calculatedPrice;
                }
                else
                {
                    lesson.Price = (float)dto.Price;
                }

                listToAdd.Add(lesson);
            }

            await SaveLessonToDbAndCalendar(listToAdd, addToGoogle);
        }

        public async Task CreateLessonWithoutStudentAsync(CreateLessonDtoWoStudent dto, bool addToGoogle)
        {
            string userId = _userContextService.GetAuthenticatedUserId;
            var lessonGroupId = Guid.NewGuid().ToString();
            var listToAdd = new List<Lesson>();

            for (int i = 0; i < (dto.Repeat ? dto.RepeatCount : 1); i++)
            {
                var lesson = _mapper.Map<Lesson>(dto);

                lesson.CreatedById = userId;

                lesson.StudentConditionId = "5";
                lesson.PhoneNumber = dto.PhoneNumber; 
                lesson.Price = dto.Price;
                if (dto.ContactTips == null || dto.ContactTips.Length == 0)
                {
                    lesson.ContactTips = "nie podano szczegółów";
                }
                lesson.IsGrouped = dto.Repeat;
                lesson.HasStudent = false;

                lesson.Date = dto.Date.AddDays(7 * i);
                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration);
                lesson.IsSyncedWithGoogle = addToGoogle;

                if (dto.PushBoolean)
                {
                    lesson.WhenPushBeforeLesson = lesson.Date.AddMinutes(-(double)dto.PushTimeBeforeLesson);
                }
                if (dto.Repeat)
                {
                    lesson.LessonGroupId = lessonGroupId;
                }

                listToAdd.Add(lesson);
            }
            await SaveLessonToDbAndCalendar(listToAdd, addToGoogle);
        }
        //==========================SingleLessonUpdate===========================
        public async Task UpdateLessonWithoutStudentAsync(string lessonId, UpdateLessonWithoutStudentDto dto)
        {
            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonId);

            string userId = _userContextService.GetAuthenticatedUserId;

            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);

            CheckStudentExistance(lesson, false);

            UpdatePropertiesWithoutStudent(lesson, dto);

            await _tutorHelperDb.SaveChangesAsync();

            await _googleCalendarApi.UpdateGoogleEvent(lesson);
        }

        public async Task UpdateLessonWithStudentAsync(string lessonId, UpdateLessonWithStudentDto dto)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonId);

            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);
            CheckStudentExistance(lesson, true);

            if (lesson is LessonWithStudent lessonWithStudent)
            {
                await UpdatePropertiesWithStudent(lessonWithStudent, dto);
            }
            else
            {
                throw new InvalidCastException("Lesson is not of type LessonWithStudent");
            }

            await _tutorHelperDb.SaveChangesAsync();
            await _googleCalendarApi.UpdateGoogleEvent(lesson);
        }
        //==========================UpdateGroupMethods===========================
        public async Task UpdateLessonWithStudentGroupAsync(string lessonId, UpdateLessonWithStudentDto dto)
        {
            string userId = _userContextService.GetAuthenticatedUserId;
            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonId);
            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);

            CheckStudentExistance(lesson, true);

            var groupId = lesson.LessonGroupId ?? throw new Exception("there is no group");

            List<LessonWithStudent> groupToUpdate = await _tutorHelperDb.Lessons
                .OfType<LessonWithStudent>()
                .Where(x => x.LessonGroupId == groupId && x.Date >= lesson.Date)
                .OrderBy(x => x.Date)
                .ToListAsync();
            TimeSpan timeShift;
            if (dto.Date.HasValue)
            {
                timeShift = dto.Date.Value - lesson.Date;
            }
            else
            {
                timeShift = new TimeSpan(0);
            }
            List<LessonWithStudent> lessonsToUpdateInGoogle = new List<LessonWithStudent>();
            foreach (var lessonGroupItem in groupToUpdate)
            {
                await UpdatePropertiesWithStudentGroupAsync(lessonGroupItem, dto, timeShift);
                lessonsToUpdateInGoogle.Add(lessonGroupItem);
            }
           
            await _tutorHelperDb.SaveChangesAsync();
            await SyncListOfLessonsToCalGoogle(lessonsToUpdateInGoogle);
        }

        public async Task UpdateLessonWithoutStudentGroupAsync(string lessonId, UpdateLessonWithoutStudentDto dto)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonId);
            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);

            CheckStudentExistance(lesson, false);

            var groupId = lesson.LessonGroupId ?? throw new Exception("Lesson is not part of a group");

            // Fetch lessons in the same group
            var lessonsToUpdate = await _tutorHelperDb.Lessons
               .Where(x => x.LessonGroupId == groupId && x.Date >= lesson.Date)
                .OrderBy(x => x.Date)
                .ToListAsync();
            TimeSpan timeShift;
            if (dto.Date.HasValue)
            {
                timeShift = dto.Date.Value - lesson.Date;
            }
            else
            {
                timeShift = new TimeSpan(0);
            }
            List<Lesson> lessonsToUpdateInGoogle = new List<Lesson>();
            foreach (var lessonItem in lessonsToUpdate)
            {
                UpdatePropertiesWithoutStudentGroup(lessonItem, dto, timeShift);
                lessonsToUpdateInGoogle.Add(lessonItem);
            }
            await SyncListOfLessonsToCalGoogle(lessonsToUpdateInGoogle);

            await _tutorHelperDb.SaveChangesAsync();
        }
        //=========================================DeleteLessons=======================================================
        public async Task DeleteLesson(string lessonId)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonId);
            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);

            _tutorHelperDb.Lessons.Remove(lesson);
            await _tutorHelperDb.SaveChangesAsync();
            DeletObjectFromCalGoogle(lesson);
        }

        public async Task DeleteLessonGroup(string lessonId)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var lesson = await _tutorHelperDb.Lessons.FindAsync(lessonId);

            DataValidationMethod.OwnershipAndNullChecker(lesson, userId);

            var groupId = lesson.LessonGroupId ?? throw new InvalidOperationException("Lesson is not part of a group");

            var lessonsToDelete = await _tutorHelperDb.Lessons
                .Where(x => x.LessonGroupId == groupId && x.Date >= lesson.Date)
                .OrderBy(x => x.Date)
                .ToListAsync();

            _tutorHelperDb.Lessons.RemoveRange(lessonsToDelete);
            await _tutorHelperDb.SaveChangesAsync();
            foreach(var x in lessonsToDelete)
            {
                if (x.IsSyncedWithGoogle)
                {
                    DeletObjectFromCalGoogle(x);

                }
            }
        }
        //==================================================PrivateMethods=============================================================
        private async Task UpdatePropertiesWithStudent(LessonWithStudent lesson, UpdateLessonWithStudentDto dto)
        {
            var student = await _tutorHelperDb.Students.FindAsync(lesson.StudentId);

            if (student == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            if (dto.Date.HasValue && !dto.Duration.HasValue)
            {
                lesson.Date = dto.Date.Value;
                lesson.EndDate = lesson.Date.AddMinutes(lesson.Duration);
            }

            if (dto.Duration.HasValue && !dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;

                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration.Value);
            }

            if (dto.Duration.HasValue && dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;
                lesson.Date = dto.Date.Value;


                lesson.EndDate = dto.Date.Value.AddMinutes(dto.Duration.Value);
            }

            var calculatedPrice = student.PricePerHour * (lesson.Duration / 60) + (float)student.PricePerDrive;
            if (dto.Price == null)
            {
                lesson.Price = (float)calculatedPrice;
            }
            else
            {
                lesson.Price = (float)dto.Price;
            }
        }

        private async Task UpdatePropertiesWithStudentGroupAsync(LessonWithStudent lesson, UpdateLessonWithStudentDto dto, TimeSpan timeShift)
        {
            var student = await _tutorHelperDb.Students.FindAsync(lesson.StudentId);

            if (student == null)
            {
                throw new KeyNotFoundException("Student not found");
            }

            if (dto.Date.HasValue && !dto.Duration.HasValue)
            {
                lesson.Date = lesson.Date.Add(timeShift);
                lesson.EndDate = lesson.Date.AddMinutes(lesson.Duration);
            }

            if (dto.Duration.HasValue && !dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;

                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration.Value);
            }
            if (dto.Duration.HasValue && dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;
                lesson.Date = lesson.Date.Add(timeShift);

                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration.Value);
            }

            var calculatedPrice = student.PricePerHour * (lesson.Duration / 60) + (float)student.PricePerDrive;
            if (dto.Price == null)
            {
                lesson.Price = (float)calculatedPrice;
            }
            else
            {
                lesson.Price = (float)dto.Price;
            }
        }

        private void UpdatePropertiesWithoutStudent(Lesson lesson, UpdateLessonWithoutStudentDto dto)
        {
            if (dto.Price.HasValue) lesson.Price = dto.Price.Value;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) lesson.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.EduStageId)) lesson.EduStageId = dto.EduStageId;
            if (!string.IsNullOrEmpty(dto.SubjectId)) lesson.SubjectId = dto.SubjectId;
            if (!string.IsNullOrEmpty(dto.LessonPlaceId)) lesson.LessonPlaceId = dto.LessonPlaceId;
            if (!string.IsNullOrEmpty(dto.ContactTips)) lesson.ContactTips = dto.ContactTips;
            if (!string.IsNullOrEmpty(dto.StudentFirstName)) lesson.StudentFirstName = dto.StudentFirstName;
            if (!string.IsNullOrEmpty(dto.StudentLastName)) lesson.StudentLastName = dto.StudentLastName;

            if (dto.Date.HasValue && !dto.Duration.HasValue)
            {
                lesson.Date = dto.Date.Value;
                lesson.EndDate = lesson.Date.AddMinutes(lesson.Duration);
            }

            if (dto.Duration.HasValue && !dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;

                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration.Value);
            }

            if (dto.Duration.HasValue && dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;
                lesson.Date = dto.Date.Value;

                lesson.EndDate = dto.Date.Value.AddMinutes(dto.Duration.Value);
            }
        }

        private void UpdatePropertiesWithoutStudentGroup(Lesson lesson, UpdateLessonWithoutStudentDto dto, TimeSpan timeShift)
        {
            if (dto.Price.HasValue) lesson.Price = dto.Price.Value;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) lesson.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.EduStageId)) lesson.EduStageId = dto.EduStageId;
            if (!string.IsNullOrEmpty(dto.SubjectId)) lesson.SubjectId = dto.SubjectId;
            if (!string.IsNullOrEmpty(dto.LessonPlaceId)) lesson.LessonPlaceId = dto.LessonPlaceId;
            if (!string.IsNullOrEmpty(dto.ContactTips)) lesson.ContactTips = dto.ContactTips;
            if (!string.IsNullOrEmpty(dto.StudentFirstName)) lesson.StudentFirstName = dto.StudentFirstName;
            if (!string.IsNullOrEmpty(dto.StudentLastName)) lesson.StudentLastName = dto.StudentLastName;

            if (dto.Date.HasValue && !dto.Duration.HasValue)
            {
                lesson.Date = lesson.Date.Add(timeShift);
                lesson.EndDate = lesson.Date.AddMinutes(lesson.Duration);
            }

            if (dto.Duration.HasValue && !dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;

                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration.Value);
            }
            if (dto.Duration.HasValue && dto.Date.HasValue)
            {
                lesson.Duration = dto.Duration.Value;
                lesson.Date = lesson.Date.Add(timeShift);

                lesson.EndDate = lesson.Date.AddMinutes(dto.Duration.Value);
            }
        }

        private void CheckStudentExistance(Lesson lesson, bool haveStudent)
        {
            if (lesson.HasStudent == true && haveStudent == false)
            {
                throw new BadRequestException("This lesson have a student");
            }
            if (lesson.HasStudent == false && haveStudent == true)
            {
                throw new BadRequestException("This lesson have a student");
            }
        }

        private async Task SaveLessonToDbAndCalendar(List<LessonWithStudent> lessons, bool addToGoogle)
        {

            if (addToGoogle)
            {
                foreach (var lesson in lessons)
                {
                    var googleCalendarEvent = _mapper.Map<GoogleCalendarEvent>(lesson);

                    var googleEventId = await _googleCalendarApi.AddEventToGoogleCalendar(googleCalendarEvent);
                    lesson.GoogleEventId = googleEventId;
                }
            }
            await _tutorHelperDb.AddRangeAsync(lessons);
            await _tutorHelperDb.SaveChangesAsync();
        }

        private async Task SaveLessonToDbAndCalendar(List<Lesson> lessons, bool addToGoogle)
        {

            if (addToGoogle)
            {
                foreach (var lesson in lessons)
                {
                    var googleCalendarEvent = _mapper.Map<GoogleCalendarEvent>(lesson);

                    var googleEventId = await _googleCalendarApi.AddEventToGoogleCalendar(googleCalendarEvent);
                    lesson.GoogleEventId = googleEventId;
                }
                await _tutorHelperDb.AddRangeAsync(lessons);
                await _tutorHelperDb.SaveChangesAsync();
            }
        }
        private async Task SyncListOfLessonsToCalGoogle(List<LessonWithStudent> lessons)
        {
            foreach (var x in lessons.Where(lesson => lesson.IsSyncedWithGoogle))
            {
                await _googleCalendarApi.UpdateGoogleEvent(x);
            }
        }
        private async Task SyncListOfLessonsToCalGoogle(List<Lesson> lessons)
        {
            foreach (var x in lessons.Where(lesson => lesson.IsSyncedWithGoogle))
            {
                await _googleCalendarApi.UpdateGoogleEvent(x);
            }
        }
        private async Task DeletObjectFromCalGoogle(Lesson lesson)
        {
            _googleCalendarApi.DeleteGoogleEvent(lesson);
        }
    }
}