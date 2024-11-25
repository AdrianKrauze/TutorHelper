using Microsoft.AspNetCore.Identity;

namespace TutorHelper.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<Student> Students { get; set; } = new List<Student>();
        public List<Lesson> Lessons { get; set; } = new List<Lesson>();

        public List<Note> Notes { get; set; } = new List<Note>();

        public ICollection<UserSubTaught> UserSubTaughts { get; set; } = new HashSet<UserSubTaught>();
        
    }
}