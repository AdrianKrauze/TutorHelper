namespace TutorHelper.Models.DtoModels.CreateModels
{
    public class CreateLessonDtoWithStudent
    {
        public string StudentId { get; set; }
        public float Duration { get; set; }
        public float? Price { get; set; }

        public DateTime Date { get; set; }

        public bool PushBoolean { get; set; }
        public double PushTimeBeforeLesson { get; set; }
        public bool Repeat { get; set; }
        public int? RepeatCount { get; set; }
    }
}