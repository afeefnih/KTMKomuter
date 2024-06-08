using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;

namespace KTMKomuter.MailSettings
{
    public class Mail
    {
        private readonly IConfiguration _configuration;

        public Mail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Send(string from, string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(to);

                using (var smtpClient = new SmtpClient(_configuration["Gmail:Host"], int.Parse(_configuration["Gmail:Port"])))
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(_configuration["Gmail:Username"], _configuration["Gmail:Password"]);
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                return false;
            }
        }
    }
}
