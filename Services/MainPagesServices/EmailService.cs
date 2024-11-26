using TutorHelper.EmailStrategy;
using TutorHelper.Models.DtoModels.CreateModels;
using System;
using System.Threading.Tasks;
using TutorHelper.Models;
using TutorHelper.Models.ConfigureClasses;

namespace TutorHelper.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(CreateEmailDto dto);
    }

    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly EmailStrategyFactory _emailStrategyFactory;
        private readonly DeveloperInfo _developerInfo;


        public EmailService(IEmailSender emailSender, EmailStrategyFactory emailStrategyFactory, DeveloperInfo developerInfo)
        {
            _emailSender = emailSender;
            _emailStrategyFactory = emailStrategyFactory;
            _developerInfo = developerInfo;
        }

        public async Task SendEmailAsync(CreateEmailDto dto)
        {
            var strategy = _emailStrategyFactory.GetStrategy(dto.EmailCase);

            var subject = _emailStrategyFactory.GetSubject(dto.EmailCase);

            var body = strategy.MakeBodyContent(dto);

            await _emailSender.SendEmailAsync(_developerInfo.Email, subject, body);

            
        }
    }
}
