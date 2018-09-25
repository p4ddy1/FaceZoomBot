using FaceZoomBot.Models;
using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class TextMessageReceivedJob : Job
    {
        public TelegramChat TelegramChat { get; set; }

        public TextMessageReceivedJob(TelegramChat telegramChat)
        {
            TelegramChat = telegramChat;
        }

        public override Worker GetWorker()
        {
            return new TextMessageReceivedWorker(this);
        }
    }
}