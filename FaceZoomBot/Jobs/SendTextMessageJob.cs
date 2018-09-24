using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class SendTextMessageJob : Job
    {
        public long ChatId { get; set; }
        public string Message { get; set; }

        public SendTextMessageJob(long chatId, string message)
        {
            ChatId = chatId;
            Message = message;
        }

        public override Worker GetWorker()
        {
            return new SendTextMessageWorker(this);
        }
    }
}