namespace TutorHelper.Models.DtoModels.ToView
{
    public class LessonListByStudentIdDto
    {
        public DateTime Date { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? WhenPushBeforeLesson { get; set; }
        public int Price { get; set; }
    }
}