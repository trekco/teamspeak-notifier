using System.Threading.Tasks;

namespace TeamSpeakNotifier.Senders
{
    public interface IMessageSender
    {
        Task SendMessageAsync(string message);
    }
}