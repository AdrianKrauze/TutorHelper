namespace TutorHelper.Models.MoneyReport
{
    public class MonthlySummary
    {
        public DateTime Date { get; set; }
        public float MonthlyIncome { get; set; }
        public float MonthlyIncomePrivate { get; set; }
        public float MonthlyIncomeTutorSchool { get; set; }

        public List<DailySummary> DailySummary { get; set; } = new List<DailySummary>();
    }
}