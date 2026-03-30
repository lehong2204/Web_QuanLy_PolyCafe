using System.Net;
using System.Net.Mail;

namespace Web_QuanLy_PolyCafe.Services
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
            var s = _config.GetSection("EmailSettings");

            var client = new SmtpClient(s["Host"])
            {
                Port = int.Parse(s["Port"]!),
                Credentials = new NetworkCredential(s["UserName"], s["Password"]),
                EnableSsl = bool.Parse(s["EnableSsl"]!),
                UseDefaultCredentials = false   // ← thêm dòng này
            };

            var mail = new MailMessage
            {
                From = new MailAddress(s["UserName"]!, s["FromName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}