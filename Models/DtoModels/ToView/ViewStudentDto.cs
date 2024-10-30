namespace TutorHelper.Models.DtoModels.ToView
{
    public class ViewStudentDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SubjectName { get; set; }  // Nazwa tematu (Subject)
        public string EduStageName { get; set; }  // Nazwa poziomu edukacji (EduStage)
        public string LessonPlaceName { get; set; }  // Nazwa miejsca lekcji (LessonPlace)
        public string StudentConditionName { get; set; }
        public float PricePerHour { get; set; }
        public float? PricePerDrive { get; set; }

        public int CountOflessons { get; set; }

        public string ContactTips { get; set; }
    }
}