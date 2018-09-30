using System;
using FaceZoomBot.Jobs;
using FaceZoomBot.Telegram;

namespace FaceZoomBot.Workers
{
    public class SendTextMessageWorker : Worker
    {
        private SendTextMessageJob Job { get; }
        private TelegramClient TelegramClient { get; }

        public SendTextMessageWorker(SendTextMessageJob job) : base(job)
        {
            Job = job;
            TelegramClient = Factory.CreateTelegramClient();
        }

        public override bool DoWork()
        {
            try
            {
                TelegramClient.SendMessage(Job.TelegramChat.ChatId, Job.TelegramChat.Message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}