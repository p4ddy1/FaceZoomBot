using System.Linq;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Models;
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
            var isPrivateChat = message.Chat.Type == ChatType.Private;

            switch (message.Type)
            {
                case MessageType.Text when isPrivateChat:
                    HandleNewPrivateText(message.Chat.Id, message.Text);
                    break;
                case MessageType.Photo:
                    HandleNewImage(message.Chat.Id, isPrivateChat, message.Photo.LastOrDefault()?.FileId);
                    break;
            }
        }

        private void HandleNewPrivateText(long chatId, string message)
        {
            var telegramChat = new TelegramChat(chatId, true, message);
            var messageReceivedJob = new TextMessageReceivedJob(telegramChat);
            Queue.AddJobToQueue(messageReceivedJob);
        }

        private void HandleNewImage(long chatId, bool isPrivateChat, string photoFileId)
        {
            var telegramChat = new TelegramChat(chatId, isPrivateChat);
            var imageReceivedJob = new ImageReceivedJob(telegramChat, photoFileId);
            Queue.AddJobToQueue(imageReceivedJob);
        }
    }
}