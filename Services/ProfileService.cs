using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities.DbContext;
using TutorHelper.Models.DtoModels.Profile;

namespace TutorHelper.Services
{
    public interface IProfileService
    {
        Task UpdateTeacherSubjectsAsync(List<string> subjectIds);
        Task DeleteUserSubjectsAsync();
        Task<string> GetEmailStateAsync();
        Task<List<UserSubTaughtDto>> GetUserSubTaughtByUserIdAsync();
    }

    public class ProfileService : IProfileService
    {
        private readonly TutorHelperDb _db;
        private readonly IUserContextService _ucr;

        public ProfileService(TutorHelperDb db, IUserContextService userContextService)
        {
            _db = db;
            _ucr = userContextService;
        }

        public async Task UpdateTeacherSubjectsAsync(List<string> subjectIds)
        {
            string userId = _ucr.GetAuthenticatedUserId;

            var userExists = await _db.Users.FindAsync(userId);
            if (userExists == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var validSubjectIds = Validators.ValidationConstants.SubjectIds.ToList();
            if (!subjectIds.All(id => validSubjectIds.Contains(id)))
            {
                throw new ArgumentException("One or more provided subject IDs are invalid.");
            }

            var existingSubjects = await _db.UserSubTaughts
                .Where(usb => usb.UserId == userId)
                .ToListAsync();

            _db.UserSubTaughts.RemoveRange(existingSubjects);

            var newSubjects = subjectIds.Select(subjectId => new UserSubTaught
            {
                UserId = userId,
                SubjectId = subjectId
            });

            await _db.UserSubTaughts.AddRangeAsync(newSubjects);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteUserSubjectsAsync()
        {
            string userId = _ucr.GetAuthenticatedUserId;

            var existingSubjects = await _db.UserSubTaughts
                .Where(usb => usb.UserId == userId)
                .ToListAsync();

            if (existingSubjects == null || !existingSubjects.Any())
            {
                throw new KeyNotFoundException("No subjects found for the user.");
            }

            _db.UserSubTaughts.RemoveRange(existingSubjects);
            await _db.SaveChangesAsync();
        }

        public async Task<string> GetEmailStateAsync()
        {
            string userId = _ucr.GetAuthenticatedUserId;

            var user = await _db.Users.FirstOrDefaultAsync(usb => usb.Id == userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            return user.EmailConfirmed ? "active" : "unactive";
        }

        public async Task<List<UserSubTaughtDto>> GetUserSubTaughtByUserIdAsync()
        {
            string userId = _ucr.GetAuthenticatedUserId;

            var subjects = await _db.UserSubTaughts
                .Where(ust => ust.UserId == userId)
                .Select(ust => new UserSubTaughtDto
                {
                    SubjectId = ust.SubjectId,
                    SubjectName = ust.Subject.Topic
                })
                .ToListAsync();

            if (subjects == null || !subjects.Any())
            {
                throw new KeyNotFoundException("No subjects found for the user.");
            }

            return subjects;
        }
    }
}
