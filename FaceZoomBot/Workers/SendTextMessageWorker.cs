using System;
using FaceZoomBot.Jobs;
using FaceZoomBot.Telegram;

namespace FaceZoomBot.Workers
{
    public class SendTextMessageWorker : Worker
    {
        private SendTextMessageJob Job { get; }

        public SendTextMessageWorker(SendTextMessageJob job) : base(job)
        {
            Job = job;   
        }

        public override bool DoWork()
        {
            try
            {
                var telegramClient = Factory.CreateTelegramClient();
                telegramClient.SendMessage(Job.TelegramChat.ChatId, Job.TelegramChat.Message);
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