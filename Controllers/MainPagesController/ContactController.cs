using Microsoft.AspNetCore.Mvc;
using TutorHelper.Models.DtoModels.CreateModels;
using TutorHelper.Services;

namespace TutorHelper.Controllers.MainPagesController
{
    [Route("api/home")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("contact/send-email")]
        public async Task<IActionResult> SendEmail(CreateEmailDto dto)
        {


            await _emailService.SendEmailAsync(dto);
           
            return Ok(new {message = "Email sent succesfully"});
        }
    }
}
