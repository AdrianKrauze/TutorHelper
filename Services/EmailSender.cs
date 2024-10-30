using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using TutorHelper.Models.ConfigureClasses;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class EmailSender : IEmailSender
{
    private readonly SmtpSettings _smtpSettings;

    public EmailSender(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.FromEmail), // Adres nadawcy
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        using (var smtpClient = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
        {
            smtpClient.Credentials = new NetworkCredential(_smtpSettings.User, _smtpSettings.Password);
            smtpClient.EnableSsl = true; // Użyj SSL, jeśli jest wymagane przez serwer SMTP
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}