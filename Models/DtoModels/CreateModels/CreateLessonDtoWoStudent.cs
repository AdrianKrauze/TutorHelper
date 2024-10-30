namespace TutorHelper.Models.DtoModels.CreateModels
{
    public class CreateLessonDtoWoStudent
    {
        public int Duration { get; set; }
        public float Price { get; set; }
        public bool PushBoolean { get; set; }
        public double? PushTimeBeforeLesson { get; set; }
        public DateTime Date { get; set; }

        public string PhoneNumber { get; set; }

        public string EduStageId { get; set; }

        public string SubjectId { get; set; }

        public string LessonPlaceId { get; set; }

        public string? ContactTips { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public bool Repeat { get; set; }
        public int? RepeatCount { get; set; }
    }
}