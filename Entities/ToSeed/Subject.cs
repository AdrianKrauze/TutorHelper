namespace TutorHelper.Entities.ToSeed
{
    public class Subject
    {
        public string Id { get; set; }
        public string Topic { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        public ICollection<UserSubTaught> UserSubTaughts { get; set; } = new HashSet<UserSubTaught>();
    }
}