using Gestionador.Interfaces;
using Gestionador.Model;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Configuration;
using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Gestionador.Services
{
    public class EmailService : BaseServices, IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _smtpName;

        public EmailService(Entities dbContext, IMapperService mapperService) : base(dbContext, mapperService)
        {
            _smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
            _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
            _smtpName = ConfigurationManager.AppSettings["SmtpName"];
        }

        public async Task SendEmailAsync(string subject, string content, string to)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpName, _smtpUsername));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = content };

            try
            {
                using (var client = new SmtpClient())
                {
                    client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

                    await client.ConnectAsync(_smtpHost, _smtpPort, MailKit.Security.SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/smtp_errors.log");
                File.AppendAllText(logPath, DateTime.Now + " | " + ex.Message + Environment.NewLine);
                System.Diagnostics.Debug.WriteLine("SMTP Error: " + ex.Message);
                throw;
            }
        }
    }
}