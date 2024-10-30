namespace TutorHelper.Models.DtoModels.UpdateModels
{
    public class UpdateLessonWithoutStudentDto
    {
        public int? Duration { get; set; }
        public float? Price { get; set; }

        public DateTime? Date { get; set; }

        public string? PhoneNumber { get; set; }

        public string? EduStageId { get; set; }

        public string? SubjectId { get; set; }

        public string? LessonPlaceId { get; set; }

        public string? ContactTips { get; set; }
        public string? StudentFirstName { get; set; }
        public string? StudentLastName { get; set; }
    }
}