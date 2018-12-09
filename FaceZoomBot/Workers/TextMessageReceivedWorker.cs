using System;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Models;

namespace FaceZoomBot.Workers
{
    public class TextMessageReceivedWorker : Worker
    {
        private TextMessageReceivedJob Job { get; }

        public TextMessageReceivedWorker(TextMessageReceivedJob job) : base(job)
        {
            Job = job;         
        }

        public override bool DoWork()
        {
            try
            {
                var queueClient = Factory.CreateQueueClient();
                using (queueClient)
                {
                    if (Job.TelegramChat.Message != "/start" || !Job.TelegramChat.IsPrivate)
                        return true;

                    var queue = Factory.CreateQueue(queueClient);
                    var telegramChat = new TelegramChat(Job.TelegramChat.ChatId, true,
                        @"Hey. I'm the FaceZoomBot. Send me your photos and I will zoom on all faces, if I can find any. 
You can also add me to a group chat.");
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