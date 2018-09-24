using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class ImageReceivedJob : Job
    {
        public long ChatId { get; set; }
        public string ImageFileId { get; set; }

        public ImageReceivedJob(long chatId, string imageFileId)
        {
            ChatId = chatId;
            ImageFileId = imageFileId;
        }

        public override Worker GetWorker()
        {
            return new ImageReceivedWorker(this);
        }
    }
}