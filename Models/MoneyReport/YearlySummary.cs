namespace TutorHelper.Models.MoneyReport
{
    public class YearlySummary
    {
        public DateTime Date { get; set; }
        public float YearlyIncome { get; set; }
        public float YearlyIncomePrivate { get; set; }
        public float YearlyIncomeTutorSchool { get; set; }

        public List<MonthlySummary> MonthlySummary { get; set; } = new List<MonthlySummary>();
    }
}