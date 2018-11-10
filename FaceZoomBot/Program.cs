using System;
using FaceZoomBot.Telegram;

namespace FaceZoomBot
{
    class Program
    {
        static int Main(string[] args)
        { 
            Console.WriteLine("FaceZoomBot - by p4ddy");
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: FaceZoomBot.dll <work> <telegram>");
                Console.WriteLine();
                Console.WriteLine("work: Listen to RabbitMQ and work on incoming jobs");
                Console.WriteLine("telegram: Listen to TelegramBotAPI and handle incoming messages");
                return 1;
            }

            var factory = new Factory();

            switch (args[0])
            {
                case "work":
                    StartWorkerMode(factory);
                    break;
                case "telegram":
                    StartTelegramMode();
                    break;
                default:
                    Console.WriteLine("No valid mode given");
                    return 1;
            }

            return 0;
        }

        private static void StartWorkerMode(Factory factory)
        {
            using (var client = factory.CreateQueueClient())
            {
                var queue = factory.CreateQueue(client);
                var workerHandler = factory.CreateWorkerHandler(queue);
                queue.ListenToQueue(workerHandler);
                Console.WriteLine("Listening to queue...");
                Console.ReadLine();
            }
        }

        private static void StartTelegramMode()
        {
            Console.WriteLine("Listening to Telegram Bot API...");
            var telegramHandler = new TelegramHandler();
            telegramHandler.Listen();
            Console.ReadLine();
        }
    }
}