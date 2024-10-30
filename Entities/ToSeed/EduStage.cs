namespace TutorHelper.Entities.ToSeed
{
    public class EduStage
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}