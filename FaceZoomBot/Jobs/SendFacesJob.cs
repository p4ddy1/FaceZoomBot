using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class SendFacesJob : Job
    {
        public long ChatId { get; set; }
        public string ImagePath { get; set; }

        public SendFacesJob(long chatId, string imagePath)
        {
            ChatId = chatId;
            ImagePath = imagePath;
        }

        public override Worker GetWorker()
        {
            return new SendFacesWorker(this);
        }
    }
}