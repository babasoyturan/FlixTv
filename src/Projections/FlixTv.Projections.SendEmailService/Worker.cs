using FlixTv.Common;
using FlixTv.Common.Infrastructure;
using FlixTv.Common.Models.DTOs;
using System.Net;
using System.Net.Mail;

namespace FlixTv.Projections.SendEmailService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            QueueFactory.CreateBasicConsumer()
                .EnsureExchange(FlixTvConstants.MovieExchangeName)
                .EnsureQueue(FlixTvConstants.SendEmailQueueName, FlixTvConstants.MovieExchangeName)
                .Receive<EmailMessageDto>(async (message) =>
                {
                    await SendEmail(message.To, message.Subject, message.Body);
                })
                .StartConsuming(FlixTvConstants.SendEmailQueueName);
        }

        private async Task SendEmail(string to, string subject, string body)
        {
            try
            {
                string senderEmail = "flixtv.movies.platform@gmail.com";
                string senderPassword = "zzuu jmam aard arfx";

                string recipientEmail = to;


                MailMessage message = new MailMessage();
                message.From = new MailAddress(senderEmail);
                message.Subject = subject;
                message.To.Add(new MailAddress(recipientEmail));
                message.Body = body;
                message.IsBodyHtml = true;

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + $" to {to}");
            }
        }
    }
}
