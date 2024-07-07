using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace Travel_Website_System_API_.Repositories
{
    public class EmailSender : IEmailSender
    {
        private readonly string smtpServer;
        private readonly int smtpPort;
        private readonly string smtpUser;
        private readonly string smtpPass;
        private readonly bool enableSsl;

        public EmailSender(IConfiguration configuration)
        {
            smtpServer = configuration["Smtp:Server"];
            smtpPort = int.Parse(configuration["Smtp:Port"]);
            smtpUser = configuration["Smtp:Username"];
            smtpPass = configuration["Smtp:Password"];
            enableSsl = bool.Parse(configuration["Smtp:EnableSsl"]);
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var client = new SmtpClient(smtpServer)
            {
                UseDefaultCredentials = false,
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = enableSsl,

            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);

            /*catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Exception: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }*/
        }

    }
}
