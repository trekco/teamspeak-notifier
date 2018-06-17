using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TeamSpeakNotifier.Senders
{
    public class EmailSender : IMessageSender
    {
        private readonly string _fromAddress;
        private readonly AppSettings _settings;
        private readonly List<string> _toAddresses;

        public EmailSender(AppSettings settings)
        {
            _settings = settings;
            _fromAddress = settings.FromEmail;
            _toAddresses = settings.ToEmails.ToList();
        }

        public async Task SendMessageAsync(string message)
        {
            await Task.Run(() =>
            {
                var subject = $"[TeamTalk Server ({DateTime.Now:dd/MM/yyyy HH:mm:ss})] {message}";

                SendEmail(_fromAddress, _toAddresses, subject, message);
            });
        }

        public void SendEmail(string fromAddress, List<string> toAddress, string subject, string body)
        {
            try
            {

                if (!_settings.EmailEnabled)
                {
                    return;
                }

                using (var client = CreateClient())
                {
                    var message = new MailMessage
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                        From = new MailAddress(fromAddress)
                    };

                    foreach (var address in toAddress)
                    {
                        message.To.Add(address);
                    }

                    client.Send(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private SmtpClient CreateClient()
        {
            var client = new SmtpClient(_settings.EmailServer);
            client.UseDefaultCredentials = false;
            client.Timeout = 60000;
            client.Port = _settings.EmailServerPort;

            if (string.IsNullOrEmpty(_settings.EmailServerUsername) || string.IsNullOrEmpty(_settings.EmailServerPassword))
            {
                return client;
            }

            client.Credentials = new NetworkCredential(_settings.EmailServerUsername, _settings.EmailServerPassword);
            client.EnableSsl = _settings.EmailServerEnableSsl;

            return client;
        }
    }
}