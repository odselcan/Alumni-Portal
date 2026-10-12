using System.Net;
using System.Net.Mail;

namespace Alumni_Portal.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var smtpHost    = _config["Email:SmtpHost"]!;
            var smtpPort    = int.Parse(_config["Email:SmtpPort"]!);
            var smtpUser    = _config["Email:Username"]!;
            var smtpPass    = _config["Email:Password"]!;
            var fromDisplay = _config["Email:FromDisplay"] ?? "Alumni Portal";

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl   = true
            };

            var mail = new MailMessage
            {
                From       = new MailAddress(smtpUser, fromDisplay),
                Subject    = subject,
                Body       = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}