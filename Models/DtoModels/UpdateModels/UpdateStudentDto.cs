namespace TutorHelper.Models.DtoModels.UpdateModels
{
    public class UpdateStudentDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ContactTips { get; set; }
        public float? PricePerHour { get; set; }
        public float? PricePerDrive { get; set; }
        public string? EduStageId { get; set; }
        public string? SubjectId { get; set; }
        public string? LessonPlaceId { get; set; }
        public string? StudentConditionId { get; set; }
    }
}
