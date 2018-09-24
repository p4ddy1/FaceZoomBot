using System.Linq;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace FaceZoomBot.Telegram
{
    public class TelegramHandler
    {
        private TelegramClient TelegramClient { get; }
        private QueueClient QueueClient { get; }
        private Queue Queue { get; }

        public TelegramHandler()
        {
            var factory = new Factory();
            TelegramClient = factory.CreateTelegramClient();
            QueueClient = factory.CreateQueueClient();
            Queue = new Queue(QueueClient);
        }

        public void Listen()
        {
            TelegramClient.BotClient.OnMessage += OnMessageReceived;
            TelegramClient.BotClient.StartReceiving();
        }

        private void OnMessageReceived(object sender, MessageEventArgs eventArgs)
        {
            var message = eventArgs.Message;

            if (message.Type == MessageType.Text
                && message.Chat.Type == ChatType.Private)
            {
                HandleNewText(message.Chat.Id, message.Text);
            }

            if (message.Type == MessageType.Photo)
            {
                HandleNewImage(message.Chat.Id, message.Photo.LastOrDefault()?.FileId);
            }
        }

        private void HandleNewText(long chatId, string message)
        {
            var messageReceivedJob = new TextMessageReceivedJob(chatId, message);
            Queue.AddJobToQueue(messageReceivedJob);
        }

        private void HandleNewImage(long chatId, string photoFileId)
        {
            var imageReceivedJob = new ImageReceivedJob(chatId, photoFileId);
            Queue.AddJobToQueue(imageReceivedJob);
        }
    }
}