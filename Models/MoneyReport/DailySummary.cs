namespace TutorHelper.Models.MoneyReport
{
    public class DailySummary
    {
        public DateTime Date { get; set; }
        public float DailyIncome { get; set; }
        public float DailyIncomePrivate { get; set; }
        public float DailyIncomeTutorSchool { get; set; }
    }
}