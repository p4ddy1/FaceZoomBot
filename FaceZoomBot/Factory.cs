using System;
using FaceZoomBot.Configuration;
using FaceZoomBot.DataStorage;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Telegram;
using FaceZoomBot.Workers;

namespace FaceZoomBot
{
    public class Factory
    {
        public QueueClient CreateQueueClient()
        {
            return new QueueClient();
        }

        public WorkerHandler CreateWorkerHandler(Queue queue)
        {
            return new WorkerHandler(queue);
        }

        public Queue CreateQueue(QueueClient client)
        {
            return new Queue(client);
        }

        public IStorage CreateStorage()
        {
            var config = LoadConfig();
            switch (config.General.StorageType)
            {
                case StorageType.FileSystem:
                    return new FileSystemStorage();
                case StorageType.MongoDB:
                    return new MongoDBStorage();
            }
            throw new Exception("Wrong storage type set");
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