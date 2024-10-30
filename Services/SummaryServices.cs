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

            
            var lessons = await _db.Lessons
                .Where(l => l.CreatedById == userId)
                .ToListAsync();

           
            var yearlySummaries = lessons
                .GroupBy(l => l.Date.Year)
                .OrderByDescending(g => g.Key)  
                .Select(g => new YearlySummary
                {
                    Date = new DateTime(g.Key, 1, 1),
                    YearlyIncome = g.Sum(l => l.Price),
                    YearlyIncomePrivate = g.Where(l => l.StudentConditionId == "1" || l.StudentConditionId == "3" || l.StudentConditionId == "5").Sum(l => l.Price),
                    YearlyIncomeTutorSchool = g.Where(l => l.StudentConditionId == "2" || l.StudentConditionId == "4").Sum(l => l.Price),
                    MonthlySummary = g.GroupBy(l => new { l.Date.Year, l.Date.Month })
                        .OrderByDescending(mg => mg.Key.Month)
                        .Select(mg => new MonthlySummary
                        {
                            Date = new DateTime(mg.Key.Year, mg.Key.Month, 1),
                            MonthlyIncome = mg.Sum(l => l.Price),
                            MonthlyIncomePrivate = mg.Where(l => l.StudentConditionId == "1" || l.StudentConditionId == "3" || l.StudentConditionId == "5").Sum(l => l.Price),
                            MonthlyIncomeTutorSchool = mg.Where(l => l.StudentConditionId == "2" || l.StudentConditionId == "4").Sum(l => l.Price),
                            DailySummary = mg.GroupBy(d => d.Date.Date)
                                .OrderByDescending(dg => dg.Key) 
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