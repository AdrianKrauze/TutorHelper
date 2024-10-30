using Microsoft.EntityFrameworkCore;
using TutorHelper.Models.MoneyReport;

namespace TutorHelper.Services
{
    public interface ISummaryServices
    {
        Task<List<YearlySummary>> GetYearlySummariesAsync();
    }

    public class SummaryServices : ISummaryServices
    {
        private readonly TutorHelperDb _db;
        private readonly IUserContextService _userContextService;

        public SummaryServices(TutorHelperDb db, IUserContextService usc)
        {
            _db = db;
            _userContextService = usc;
        }

        public async Task<List<YearlySummary>> GetYearlySummariesAsync()
        {
            string userId = _userContextService.GetAuthenticatedUserId;

            // Asynchronicznie pobierz dane z tabeli Lessons dla konkretnego nauczyciela
            var lessons = await _db.Lessons
                .Where(l => l.CreatedById == userId)
                .ToListAsync();

            // Grupowanie danych według roku i sortowanie
            var yearlySummaries = lessons
                .GroupBy(l => l.Date.Year)
                .OrderByDescending(g => g.Key)  // Sortowanie po roku
                .Select(g => new YearlySummary
                {
                    Date = new DateTime(g.Key, 1, 1), // Ustaw rok w Date
                    YearlyIncome = g.Sum(l => l.Price),
                    YearlyIncomePrivate = g.Where(l => l.StudentConditionId == "1" || l.StudentConditionId == "3" || l.StudentConditionId == "5").Sum(l => l.Price),
                    YearlyIncomeTutorSchool = g.Where(l => l.StudentConditionId == "2" || l.StudentConditionId == "4").Sum(l => l.Price),
                    MonthlySummary = g.GroupBy(l => new { l.Date.Year, l.Date.Month })
                        .OrderByDescending(mg => mg.Key.Month) // Sortowanie miesięcy wewnątrz roku
                        .Select(mg => new MonthlySummary
                        {
                            Date = new DateTime(mg.Key.Year, mg.Key.Month, 1),
                            MonthlyIncome = mg.Sum(l => l.Price),
                            MonthlyIncomePrivate = mg.Where(l => l.StudentConditionId == "1" || l.StudentConditionId == "3" || l.StudentConditionId == "5").Sum(l => l.Price),
                            MonthlyIncomeTutorSchool = mg.Where(l => l.StudentConditionId == "2" || l.StudentConditionId == "4").Sum(l => l.Price),
                            DailySummary = mg.GroupBy(d => d.Date.Date)
                                .OrderByDescending(dg => dg.Key) // Sortowanie dni wewnątrz miesiąca
                                .Select(dg => new DailySummary
                                {
                                    Date = dg.Key,
                                    DailyIncome = dg.Sum(l => l.Price),
                                    DailyIncomePrivate = dg.Where(l => l.StudentConditionId == "1" || l.StudentConditionId == "3" || l.StudentConditionId == "5").Sum(l => l.Price),
                                    DailyIncomeTutorSchool = dg.Where(l => l.StudentConditionId == "2" || l.StudentConditionId == "4").Sum(l => l.Price)
                                }).ToList()
                        }).ToList()
                }).ToList();

            return yearlySummaries;
        }
    }
}