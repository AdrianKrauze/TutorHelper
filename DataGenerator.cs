using AutoMapper;
using Bogus;
using TutorHelper.Entities.DbContext;
using TutorHelper.Entities;
using TutorHelper.Services;
using TutorHelper.Validators;
using Microsoft.EntityFrameworkCore;

public interface IDataGenerator
{
    Task GenerateDataAsync();
    Task GenerateStudentAndLessonForLoggedUser();
}

public class DataGenerator : IDataGenerator
{
    private readonly TutorHelperDb _dbContext;
    private readonly IUserContextService _userContextService;

    private const string locale = "pl";

    public DataGenerator(TutorHelperDb dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;

    }

    public async Task GenerateStudentAndLessonForLoggedUser()
    {
        // Pobranie identyfikatora zalogowanego użytkownika
        string userId = _userContextService.GetAuthenticatedUserId;

        // Generator dla obiektu Student
        var studentGenerator = new Faker<Student>("pl")
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.PricePerHour, f => f.Random.Int(30, 180))
            .RuleFor(s => s.EduStageId, f => f.Random.ArrayElement(ValidationConstants.EduStageIds))
            .RuleFor(s => s.SubjectId, f => f.Random.ArrayElement(ValidationConstants.SubjectIds))
            .RuleFor(s => s.LessonPlaceId, f => f.Random.ArrayElement(ValidationConstants.LessonPlaceIds))
            .RuleFor(s => s.StudentConditionId, f => f.Random.ArrayElement(ValidationConstants.StudentConditionIds))
            .RuleFor(s => s.ContactTips, f => f.Lorem.Sentence(5))
            .RuleFor(s => s.PricePerDrive, (f, s) => s.LessonPlaceId == "1" ? f.Random.Int(0, 100) : 0)
            .RuleFor(s => s.CreatedById, f => userId)
            .RuleFor(s => s.CreatedAt, f => f.Date.Past())
            .RuleFor(s => s.PlaceholderCourseData, f => new PlaceholderCourseData
            {
                LessonTime = TimeOnly.FromDateTime(f.Date.Future()),
                DayOfLesson = f.PickRandom<DayOfWeek>(),
                Duration = f.PickRandom(new[] { 60f, 90f })
            });

        // Generowanie i zapisanie 100 studentów
        var students = studentGenerator.Generate(100);
        await _dbContext.Students.AddRangeAsync(students);
        await _dbContext.SaveChangesAsync();

        // Lista lekcji do zapisania w bazie danych
        var lessonListToDb = new List<LessonWithStudent>();

        // Pobranie identyfikatorów studentów powiązanych z zalogowanym użytkownikiem
        var studentIds = await _dbContext.Students
            .Where(s => s.CreatedById == userId)
            .Select(s => s.Id)
            .ToListAsync();

        foreach (var studentId in studentIds)
        {
            // Pobranie danych studenta
            var student = await _dbContext.Students
                .Include(s => s.EduStage)
                .Include(s => s.Subject)
                .Include(s => s.LessonPlace)
                .Include(s => s.StudentCondition)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            // Generator dla obiektu LessonWithStudent
            var lessonGenerator = CreateLessonWithStudentFaker(locale, student,userId);

            // Generowanie 10 lekcji dla każdego studenta i dodanie ich do listy
            lessonListToDb.AddRange(lessonGenerator.Generate(10));
        }

        // Zapisanie lekcji do bazy danych
        await _dbContext.Lessons.AddRangeAsync(lessonListToDb);
        await _dbContext.SaveChangesAsync();
    }


    public async Task GenerateDataAsync()
    {
        // Generate 10 users
        var userGenerator = new Faker<User>(locale)
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.LastName, f => f.Name.LastName())
            .RuleFor(r => r.EmailConfirmed, f => true);

        var users = userGenerator.Generate(10);


        // Add users to the DbContext and save them
        await _dbContext.Users.AddRangeAsync(users);
        await _dbContext.SaveChangesAsync(); // Ensure users are saved to populate their Ids
        var userIds = await _dbContext.Users.Select(u => u.Id).ToListAsync();

        var studentGenerator = new Faker<Student>(locale)
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.PricePerHour, f => f.Random.Int(30, 180))
            .RuleFor(s => s.EduStageId, f => f.Random.ArrayElement(ValidationConstants.EduStageIds))
            .RuleFor(s => s.SubjectId, f => f.Random.ArrayElement(ValidationConstants.SubjectIds))
            .RuleFor(s => s.LessonPlaceId, f => f.Random.ArrayElement(ValidationConstants.LessonPlaceIds))
            .RuleFor(s => s.StudentConditionId, f => f.Random.ArrayElement(ValidationConstants.StudentConditionIds))
            .RuleFor(s => s.ContactTips, f => f.Lorem.Sentence(5))
            .RuleFor(s => s.PricePerDrive, (f, s) => s.LessonPlaceId == "1" ? f.Random.Int(0, 100) : 0)
            .RuleFor(s => s.CreatedById, f => f.PickRandom(userIds)) // Randomly pick a CreatedById from the list of user IDs
            .RuleFor(s => s.CreatedAt, f => f.Date.Past()) // Set CreatedAt to a past date
            .RuleFor(s => s.PlaceholderCourseData, f => new PlaceholderCourseData
            {
                LessonTime = TimeOnly.FromDateTime(f.Date.Future()),
                DayOfLesson = f.PickRandom<DayOfWeek>(),
                Duration = f.PickRandom(new[] { 60f, 90f })
            });



        var students = studentGenerator.Generate(100);
        await _dbContext.Students.AddRangeAsync(students);
        await _dbContext.SaveChangesAsync();

        var studentIds = await _dbContext.Students.Select(s => s.Id).ToListAsync();
        var lessonListToDb = new List<LessonWithStudent>();

        foreach (var studentId in studentIds)
        {
            var student = await _dbContext.Students
               .Include(s => s.EduStage)
               .Include(s => s.Subject)
               .Include(s => s.LessonPlace)
               .Include(s => s.StudentCondition)
               .FirstOrDefaultAsync(s => s.Id == studentId);



            var lessonGenerator = CreateLessonWithStudentFaker(locale, student);

            lessonListToDb.AddRange(lessonGenerator.Generate(10));

        }
        await _dbContext.Lessons.AddRangeAsync(lessonListToDb);
        await _dbContext.SaveChangesAsync();
    }

    private Faker<LessonWithStudent> CreateLessonWithStudentFaker(string locale, Student student, string? userId = null)
    {
        return new Faker<LessonWithStudent>(locale)
            .RuleFor(l => l.StudentId, f => student.Id)
            .RuleFor(l => l.CreatedById, f => userId ?? student.CreatedById) // Use provided userId or student's CreatedById
            .RuleFor(l => l.EduStageId, f => student.EduStageId)
            .RuleFor(l => l.SubjectId, f => student.SubjectId)
            .RuleFor(l => l.LessonPlaceId, f => student.LessonPlaceId)
            .RuleFor(l => l.StudentConditionId, f => student.StudentConditionId)
            .RuleFor(l => l.ContactTips, f => student.ContactTips)
            .RuleFor(l => l.StudentFirstName, f => student.FirstName)
            .RuleFor(l => l.StudentLastName, f => student.LastName)
            .RuleFor(l => l.HasStudent, f => true)
            .RuleFor(l => l.PhoneNumber, f => student.PhoneNumber)
            .RuleFor(l => l.Date, (f, s) =>
                s.StudentConditionId == "1" || s.StudentConditionId == "3" ? f.Date.Future() : f.Date.Past())
            .RuleFor(l => l.Duration, f => f.Random.Int(15, 180))
            .RuleFor(l => l.Price, f => f.Random.Float(10, 300));
            
    }

}

