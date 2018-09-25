using FaceZoomBot.Configuration;
using FaceZoomBot.DataStorage;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Telegram;

namespace FaceZoomBot
{
    public class Factory
    {
        public QueueClient CreateQueueClient()
        {
            return new QueueClient();
        }

        public Queue CreateQueue(QueueClient client)
        {
            return new Queue(client);
        }

        public FileSystemStorage CreateFileSystemStorage()
        {
            return new FileSystemStorage();
        }

        public TelegramClient CreateTelegramClient()
        {
            return new TelegramClient();
        }

        public Config LoadConfig()
        {
            return Config.Load("config/config.json");
        }
    }
}