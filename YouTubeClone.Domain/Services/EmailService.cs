using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using YouTubeClone.Shared.Constants;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Shared.Models;

namespace YouTubeClone.Domain.Services
{
    public class EmailService(EmailSettings emailSettings) : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(emailSettings.Host, emailSettings.Port)
            {
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(AuthMessages.EmailDefaults.FromAddress, AuthMessages.EmailDefaults.EmailSenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(AuthMessages.EmailDefaults.EmailSendFailed, ex);
            }
        }
    }
}
