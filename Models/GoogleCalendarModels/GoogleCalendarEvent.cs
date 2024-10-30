namespace TutorHelper.Models.GoogleCalendarModels
{
    public class GoogleCalendarEvent
    {
        public string Summary { get; set; }
        public string Location { get; set; } = null;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeZone { get; set; } = "Europe/Warsaw"; // Domyślna strefa czasowa
        public bool PushBoolean { get; set; }
        public double? PushTimeBeforeLesson { get; set; }
        public string Id { get; set; }
        public string? LessonGroupId { get; set; }
    }

}
