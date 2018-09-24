using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class ZoomSeFacesJob : Job
    {
        public long ChatId { get; set; }
        public string ImagePath { get; set; }

        public ZoomSeFacesJob(long chatId, string imagePath)
        {
            ChatId = chatId;
            ImagePath = imagePath;
        }

        public override Worker GetWorker()
        {
            return new ZoomSeFacesWorker(this);
        }
    }
}