using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;

namespace FaceZoomBot.Workers
{
    public class TextMessageReceivedWorker : Worker
    {
        private TextMessageReceivedJob Job { get; }
        private QueueClient QueueClient { get; }

        public TextMessageReceivedWorker(TextMessageReceivedJob job) : base(job)
        {
            Job = job;
            QueueClient = Factory.CreateQueueClient();
        }

        public override void DoWork()
        {
            using (QueueClient)
            {
                if (Job.Message != "/start")
                    return;
            
                var queue = Factory.CreateQueue(QueueClient);
                var sendTextMessageJob = new SendTextMessageJob(
                    Job.ChatId,
                    @"Hey. I'm the FaceZoomBot. Send me your photos and I will zoom on all faces, if I can find any. Currently, I'm in an early alpha state, so expect a few bugs. If you have any questions about me ask my creator @Melun. Just send me a Photo to start. (Not as a file please). If I don't answer, I have found nothing.");
                queue.AddJobToQueue(sendTextMessageJob);
            }
        }
    }
}