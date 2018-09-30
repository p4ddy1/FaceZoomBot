using System;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Models;

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

        public override bool DoWork()
        {
            try
            {
                using (QueueClient)
                {
                    if (Job.TelegramChat.Message != "/start" || !Job.TelegramChat.IsPrivate)
                        return true;

                    var queue = Factory.CreateQueue(QueueClient);
                    var telegramChat = new TelegramChat(Job.TelegramChat.ChatId, true,
                        @"Hey. I'm the FaceZoomBot. Send me your photos and I will zoom on all faces, if I can find any. Currently, I'm in an early alpha state, so expect a few bugs. If you have any questions about me ask my creator @Melun. Just send me a Photo to start. (Not as a file please)");
                    var sendTextMessageJob = new SendTextMessageJob(telegramChat);
                    queue.AddJobToQueue(sendTextMessageJob);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}