namespace TutorHelper.Models.DtoModels
{
    public class PlaceholderLesson
    {
        public string studentId { get; set; }

        public int Duration { get; set; }
        public string Summary { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
