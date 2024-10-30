using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TutorHelper.Models.MoneyReport;
using TutorHelper.Services;

namespace TutorHelper.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryServices _sS;

        public SummaryController(ISummaryServices sS)
        {
            _sS = sS;
        }

        [HttpGet]
        public async Task<ActionResult<YearlySummary>> GetSummary()
        {
            var result = await _sS.GetYearlySummariesAsync();
            return Ok(result);
        }
    }
}