using System;
using FaceZoomBot.Configuration;
using FaceZoomBot.Workers;
using FaceZoomBot.Telegram;

namespace FaceZoomBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("FaceZoomBot ALPHA - by p4ddy");
            if (args.Length < 1)
            {
                Console.WriteLine("Please start with work or send");
                return;
            }

            var factory = new Factory();

            switch (args[0])
            {
                case "work":
                    using (var client = factory.CreateQueueClient())
                    {
                        var workerHandler = new WorkerHandler();
                        var queue = factory.CreateQueue(client);
                        queue.ListenToQueue(workerHandler);
                        Console.WriteLine("Listening to queue...");
                        Console.ReadLine();
                    }

                    break;

                case "send":
                    Console.WriteLine("Staring as Telegram Handler!");
                    var telegramHandler = new TelegramHandler();
                    telegramHandler.Listen();
                    Console.ReadLine();
                    break;

                default:
                    Console.WriteLine("No valid mode given");
                    return;
            }
        }
    }
}