using FaceZoomBot.Models;
using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class SendFacesJob : Job
    {
        public TelegramChat TelegramChat { get; set; }
        public string ImagePath { get; set; }

        public SendFacesJob(TelegramChat telegramChat, string imagePath)
        {
            TelegramChat = telegramChat;
            ImagePath = imagePath;
        }

        public override Worker GetWorker()
        {
            return new SendFacesWorker(this);
        }
    }
}