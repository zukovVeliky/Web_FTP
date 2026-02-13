using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Hosting;
using MimeKit.Text;
using MailKit.Security;
using System.Net.Mail;

namespace Code
{

    public class EmailSettings
    {
        public string MailServer { get; set; }
        public int MailPort { get; set; }
        public string SenderName { get; set; }
        public string Sender { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
    public class EmailSender : IEmailSender
    {

        private readonly EmailSettings _emailSettings;
        private readonly IHostEnvironment _env;

        public EmailSender(
            IOptions<EmailSettings> emailSettings,
            IHostEnvironment env)
        {
            _emailSettings = emailSettings.Value;
            _env = env;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {

                // create message

                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
                mimeMessage.To.Add(MailboxAddress.Parse(email));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart("html")
                {
                    Text = message
                };

                // send email
                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                {
                    
                    await smtp.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, true);
                    await smtp.AuthenticateAsync(_emailSettings.User, _emailSettings.Password);
                    await smtp.SendAsync(mimeMessage);
                    await smtp.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

        }

    }
}

