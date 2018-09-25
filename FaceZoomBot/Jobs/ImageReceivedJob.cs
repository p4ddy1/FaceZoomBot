using FaceZoomBot.Models;
using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class ImageReceivedJob : Job
    {
        public TelegramChat TelegramChat { get; set; }
        public string ImageFileId { get; set; }

        public ImageReceivedJob(TelegramChat telegramChat, string imageFileId)
        {
            TelegramChat = telegramChat;
            ImageFileId = imageFileId;
        }

        public override Worker GetWorker()
        {
            return new ImageReceivedWorker(this);
        }
    }
}