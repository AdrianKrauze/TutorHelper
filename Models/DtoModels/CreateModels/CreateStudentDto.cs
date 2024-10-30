namespace TutorHelper.Models.DtoModels.CreateModels
{
    public class CreateStudentDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public string LessonPlaceId { get; set; } // Pole na identyfikator miejsca lekcji
        public string? ContactTips { get; set; }

        public float PricePerHour { get; set; }
        public float? PricePerDrive { get; set; }

        public string EduStageId { get; set; } // Pole na identyfikator EduStage
        public string SubjectId { get; set; }  // Pole na identyfikator przedmiotu
        public string StudentConditionId { get; set; }
    }
}