namespace TutorHelper.Models.ConfigureClasses
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
    }
}