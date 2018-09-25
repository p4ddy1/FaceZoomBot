using FaceZoomBot.Models;
using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class ZoomSeFacesJob : Job
    {
        public TelegramChat TelegramChat { get; set; }
        public string ImagePath { get; set; }

        public ZoomSeFacesJob(TelegramChat telegramChat, string imagePath)
        {
            TelegramChat = telegramChat;
            ImagePath = imagePath;
        }

        public override Worker GetWorker()
        {
            return new ZoomSeFacesWorker(this);
        }
    }
}