using TutorHelper.Entities.OwnershipChecker;

namespace TutorHelper.Entities
{
    public class Note : IOwner
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; }
        public DateTime CreateTime { get; set; }

        public string StudentId { get; set; }
        public Student Student { get; set; }

        public string CreatedById { get; set; }
        public User CreatedBy { get; set; }
        
    }
}