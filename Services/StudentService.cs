﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities;
using TutorHelper.Entities.OwnershipChecker;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Models.DtoModels.ToView;
using TutorHelper.Models.DtoModels.UpdateModels;
using TutorHelper.Middlewares.Exceptions;
using TutorHelper.Models;
using TutorHelper.Entities.DbContext;
using LtDB = LinqToDB;

namespace TutorHelper.Services
{
    public interface IStudentService
    {
        Task<string> CreateStudentAsync(CreateStudentDto dto);

        Task UpdateStudent(string id, UpdateStudentDto dto);

        Task<List<ViewStudentDtoToList>> ViewActiveStudentList();

        Task<List<ViewStudentDtoToList>> ViewActiveStudentListBySearchPhrase(string searchPhrase);

        Task<List<ViewStudentDtoToList>> ViewEndedStudentList();

        Task<List<ViewStudentDtoToList>> ViewEndedStudentListBySearchPhrase(string searchPhrase);

        Task<ViewStudentDto> ViewStudentById(string studentId);

        Task DeleteStudentAsync(string studentId);
        Task<PageResult<LessonListByStudentIdDto>> GetLessonsByStudentId(string studentId, SearchQuery searchQuery);

    }

    public class StudentService : IStudentService
    {
        private readonly IMapper _mapper;
        private readonly TutorHelperDb _tutorHelperDb;
        private readonly IUserContextService _userContextService;

        public StudentService(TutorHelperDb tutorHelperDb,
                              IUserContextService userContextService,
                              IMapper mapper)
        {
            _tutorHelperDb = tutorHelperDb;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<string> CreateStudentAsync(CreateStudentDto dto)
        {
            var student = _mapper.Map<Student>(dto);
            string userId = _userContextService.GetAuthenticatedUserId;
            student.CreatedById = userId;
            student.CreatedAt = DateTime.UtcNow;

            if (dto.ContactTips == null || dto.ContactTips.Length == 0)
            {
                student.ContactTips = "nie podano szczegółów";
            }

            if (dto.PricePerDrive == null)
            {
                student.PricePerDrive = 0;
            }

            await _tutorHelperDb.Students.AddAsync(student);
            await _tutorHelperDb.SaveChangesAsync();

            return student.Id;
        }

        public async Task<ViewStudentDto> ViewStudentById(string studentId)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

           
            var student = await _tutorHelperDb.Students
         .Include(s => s.Subject)
         .Include(s => s.EduStage)
         .Include(s => s.LessonPlace)
         .Include(s => s.Lessons) 
         .Include(s => s.StudentCondition)
         .FirstOrDefaultAsync(s => s.Id == studentId);

            
            DataValidationMethod.OwnershipAndNullChecker(student, userId);

            var studentDto = _mapper.Map<ViewStudentDto>(student);

          
            studentDto.CountOflessons = student.Lessons?.Count ?? 0;

            return studentDto;
        }

        #region WidokiStudentow  
        public async Task<List<ViewStudentDtoToList>> ViewActiveStudentList()
        {
            var userId = _userContextService.GetAuthenticatedUserId;

            var studentList = await _tutorHelperDb.Students
                .Include(s => s.Subject)
                .Include(s => s.EduStage)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .Where(s => s.CreatedById == userId
                    && (s.StudentConditionId == "1" || s.StudentConditionId == "3"))
                .ToListAsync();

            return _mapper.Map<List<ViewStudentDtoToList>>(studentList);
        }

        public async Task<List<ViewStudentDtoToList>> ViewActiveStudentListBySearchPhrase(string searchPhrase)
        {
            var searchPhraseLower = string.IsNullOrWhiteSpace(searchPhrase) ? string.Empty : searchPhrase.ToLower();
            var userId = _userContextService.GetAuthenticatedUserId;

            var studentList = await _tutorHelperDb.Students
                .Include(s => s.Subject)
                .Include(s => s.EduStage)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .Where(s => s.CreatedById == userId
                    && (s.StudentConditionId == "1" || s.StudentConditionId == "3")
                     && EF.Functions.Like((s.FirstName + " " + s.LastName), $"%{searchPhrase}%"))
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<ViewStudentDtoToList>>(studentList);
        }

       
        public async Task<List<ViewStudentDtoToList>> ViewEndedStudentList()
        {
            var userId = _userContextService.GetAuthenticatedUserId;

            var studentList = await _tutorHelperDb.Students
                .Include(s => s.Subject)
                .Include(s => s.EduStage)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .Where(s => s.CreatedById == userId
                    && (s.StudentConditionId == "2" || s.StudentConditionId == "4"))
                  .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<ViewStudentDtoToList>>(studentList);
        }

        public async Task<List<ViewStudentDtoToList>> ViewEndedStudentListBySearchPhrase(string searchPhrase)
        {
            var searchPhraseLower = string.IsNullOrWhiteSpace(searchPhrase) ? string.Empty : searchPhrase.ToLower();
            var userId = _userContextService.GetAuthenticatedUserId;

            var studentList = await _tutorHelperDb.Students
                .Include(s => s.Subject)
                .Include(s => s.EduStage)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .Where(s => s.CreatedById == userId
                    && (s.StudentConditionId == "2" || s.StudentConditionId == "4")
                     && EF.Functions.Like((s.FirstName + " " + s.LastName), $"%{searchPhrase}%"))
                  .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<List<ViewStudentDtoToList>>(studentList);
        }
        #endregion

        public async Task DeleteStudentAsync(string studentId)
        {
            Student student = await _tutorHelperDb.Students.FindAsync(studentId);

            string userId = _userContextService.GetAuthenticatedUserId;

             DataValidationMethod.OwnershipAndNullChecker(student, userId);

            _tutorHelperDb.Students.Remove(student);
            await _tutorHelperDb.SaveChangesAsync();
        }

        public async Task UpdateStudent(string id, UpdateStudentDto dto)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var student = await _tutorHelperDb.Students
                .Include(s => s.Subject)
                .Include(s => s.EduStage)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .Include(s => s.CreatedBy)
                .FirstOrDefaultAsync(s => s.Id == id);

             DataValidationMethod.OwnershipAndNullChecker(student, userId);

            var futureLessonForStudent = await _tutorHelperDb.Lessons
        .OfType<LessonWithStudent>()
        .Include(s => s.Subject)
        .Include(s => s.EduStage)
        .Include(s => s.LessonPlace)
        .Include(s => s.CreatedBy)
        .Include(s => s.Student)
        .Where(s => s.StudentId == id && s.Date > DateTime.Now)
        .ToListAsync();

            var allLessonForStudent = await _tutorHelperDb.Lessons
                .OfType<LessonWithStudent>()
                .Include(s => s.CreatedBy)
                .Include(s => s.Student)
                .Where(s => s.StudentId == id)
                .ToListAsync();


            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                student.FirstName = dto.FirstName;
                
            }

            if (!string.IsNullOrEmpty(dto.LastName))
            {
                student.LastName = dto.LastName;
               
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                student.PhoneNumber = dto.PhoneNumber;
                
            }


            if (dto.PricePerHour.HasValue)
            {
                student.PricePerHour = (float)dto.PricePerHour;
                
            }

            if (!string.IsNullOrEmpty(dto.ContactTips))
            {
                student.ContactTips = dto.ContactTips;
                
            }

            if (!string.IsNullOrEmpty(dto.EduStageId))
            {
                student.EduStageId = dto.EduStageId;
               
            }

            if (!string.IsNullOrEmpty(dto.SubjectId))
            {
                student.SubjectId = dto.SubjectId;
               
            }

            if (!string.IsNullOrEmpty(dto.LessonPlaceId))
            {
                student.LessonPlaceId = dto.LessonPlaceId;
                
            }

           

           
                foreach(var lesson in allLessonForStudent)
                {
                    if (!string.IsNullOrEmpty(dto.FirstName)) lesson.StudentFirstName = dto.FirstName;
                    if (!string.IsNullOrEmpty(dto.LastName)) lesson.StudentLastName = dto.LastName;
                    if (!string.IsNullOrEmpty(dto.PhoneNumber)) lesson.PhoneNumber = dto.PhoneNumber;
                }
            

            foreach (var lesson in futureLessonForStudent)
            {
                // Zastosowanie zmian tylko wtedy, gdy wartości są dostępne w DTO
                lesson.ContactTips = !string.IsNullOrEmpty(dto.ContactTips) ? dto.ContactTips : lesson.ContactTips;
                lesson.EduStageId = !string.IsNullOrEmpty(dto.EduStageId) ? dto.EduStageId : lesson.EduStageId;
                lesson.SubjectId = !string.IsNullOrEmpty(dto.SubjectId) ? dto.SubjectId : lesson.SubjectId;
                lesson.LessonPlaceId = !string.IsNullOrEmpty(dto.LessonPlaceId) ? dto.LessonPlaceId : lesson.LessonPlaceId;
            }



            _tutorHelperDb.Lessons.UpdateRange(allLessonForStudent);
            _tutorHelperDb.Lessons.UpdateRange(futureLessonForStudent);
            _tutorHelperDb.Students.Update(student);


            await _tutorHelperDb.SaveChangesAsync();
        }
        public async Task<PageResult<LessonListByStudentIdDto>> GetLessonsByStudentId(string studentId, SearchQuery searchQuery)
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            var student = await _tutorHelperDb.Students.FindAsync(studentId);

            DataValidationMethod.OwnershipAndNullChecker(student, userId);

            var query = _tutorHelperDb.Lessons
                .OfType<LessonWithStudent>()
                .Include(x => x.EduStage)
                .Include(x => x.LessonPlace)
                .Include(x => x.Subject)
                .Where(l => l.StudentId == studentId && l.CreatedById == userId)
                .OrderBy(l => l.Date)
                .AsQueryable();

            var totalItemsCount = await query.CountAsync();
            var lessons = await query
                .Skip((searchQuery.PageNumber - 1) * searchQuery.PageSize)
                .Take(searchQuery.PageSize)
                .ToListAsync();

            var lessonsListDto = _mapper.Map<List<LessonListByStudentIdDto>>(lessons);

            return new PageResult<LessonListByStudentIdDto>(lessonsListDto, totalItemsCount, searchQuery.PageSize, searchQuery.PageNumber);
        }

    }
}