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

        public override void DoWork()
        {
            TelegramClient.SendMessage(Job.TelegramChat.ChatId, Job.TelegramChat.Message);
        }
    }
}