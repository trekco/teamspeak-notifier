using System;
using System.Threading.Tasks;

namespace TeamSpeakNotifier.Senders
{
    public class ConsoleSender : IMessageSender
    {
        private readonly AppSettings _settings;

        public ConsoleSender(AppSettings settings)
        {
            _settings = settings;
        }

        public async Task SendMessageAsync(string message)
        {
            await Task.Run(() => { Console.WriteLine($"[{DateTime.Now:dd-MM-yyyy HH:mm:ss}] {message}"); });
        }
    }
}