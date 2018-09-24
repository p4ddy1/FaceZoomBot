using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public class TextMessageReceivedJob : Job
    {
        public long ChatId { get; set; }
        public string Message { get; set; }

        public TextMessageReceivedJob(long chatId, string message)
        {
            ChatId = chatId;
            Message = message;
        }

        public override Worker GetWorker()
        {
            return new TextMessageReceivedWorker(this);
        }
    }
}