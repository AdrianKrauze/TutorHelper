namespace TutorHelper.Models.DtoModels.ToView
{
    public class LessonObjectDto
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public DateTime EndDate { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public int Price { get; set; }
        public string SubjectName { get; set; }
        public string EduStage { get; set; }
        public string LessonPlaceName { get; set; }
        public string ConditionName { get; set; }

        public string Address { get; set; }

        public bool IsRepeated { get; set; }
        public string LessonGroupId { get; set; }
        public bool HasStudent { get; set; }
    }
}