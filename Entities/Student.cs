
using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities.OwnershipChecker;
using TutorHelper.Entities.ToSeed;

namespace TutorHelper.Entities
{
    public class Student : IOwner
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public float PricePerHour { get; set; }
        public float PricePerDrive { get; set; }
        public string ContactTips { get; set; }

        public ICollection<LessonWithStudent> Lessons { get; set; } = new List<LessonWithStudent>();

        public ICollection<Note> Notes { get; set; } = new List<Note>();

        public DateTime CreatedAt { get; set; }

        public string EduStageId { get; set; }
        public EduStage EduStage { get; set; }

        public string SubjectId { get; set; }
        public Subject Subject { get; set; }

        public string LessonPlaceId { get; set; }
        public LessonPlace LessonPlace { get; set; }

        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public string StudentConditionId { get; set; }
        public StudentCondition StudentCondition { get; set; }

        public PlaceholderCourseData? PlaceholderCourseData { get; set; }
        public string ReturnFullName()
        {
            return $"{FirstName} {LastName}";
        }

      
    }

    [Owned] 
    public class PlaceholderCourseData
    {
        public DayOfWeek DayOfLesson { get; set; }
        public TimeOnly LessonTime { get; set; }

        public float Duration { get; set; }
    }

    

   

}