
using TutorHelper.Entities.OwnershipChecker;
using TutorHelper.Entities.ToSeed;

namespace TutorHelper.Entities
{
    public class Lesson : IOwner
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public float Duration { get; set; }
        public float Price { get; set; }
        public bool PushBoolean { get; set; }
        public double? PushTimeBeforeLesson { get; set; }
        public DateTime Date { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? WhenPushBeforeLesson { get; set; }

        public string PhoneNumber { get; set; }

        public string EduStageId { get; set; }

        public EduStage EduStage { get; set; }

        public string SubjectId { get; set; }
        public Subject Subject { get; set; }

        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }

        public string LessonPlaceId { get; set; }
        public LessonPlace LessonPlace { get; set; }
        public string StudentConditionId { get; set; }
        public StudentCondition StudentCondition { get; set; }

        public string ContactTips { get; set; }

        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string? LessonGroupId { get; set; }

        public bool HasStudent { get; set; }
        public bool IsGrouped { get; set; }
        public bool IsSyncedWithGoogle { get; set; }
        public string? GoogleEventId { get; set; }

    }
    public class LessonWithStudent : Lesson
    {
        public string StudentId { get; set; }

        public Student Student { get; set; }
    }
}