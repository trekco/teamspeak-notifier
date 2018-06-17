using System.Collections.Generic;

namespace TeamSpeakNotifier.Senders
{
    public static class MessageSender
    {
        public static List<IMessageSender> _senders;

        static MessageSender()
        {
            _senders = new List<IMessageSender>();
        }

        public static void RegisterSender(IMessageSender sender)
        {
            _senders.Add(sender);
        }

        public static void SendMessage(string message)
        {
            foreach (var sender in _senders)
            {
                sender.SendMessageAsync(message);
            }
        }

        public static void SendMessage<T>(string message)
        {
            foreach (var sender in _senders)
            {
                if (sender.GetType() == typeof(T))
                {
                    sender.SendMessageAsync(message);
                }
            }
        }
    }
}