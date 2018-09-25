using FaceZoomBot.Models;
using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class SendTextMessageJob : Job
    {
        public TelegramChat TelegramChat { get; set; }

        public SendTextMessageJob(TelegramChat telegramChat)
        {
            TelegramChat = telegramChat;
        }

        public override Worker GetWorker()
        {
            return new SendTextMessageWorker(this);
        }
    }
}