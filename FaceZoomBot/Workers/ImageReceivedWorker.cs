using System;
using System.IO;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Telegram;

namespace FaceZoomBot.Workers
{
    public class ImageReceivedWorker : Worker
    {
        private ImageReceivedJob Job { get; }
        private TelegramClient TelegramClient { get; }
        private IStorage Storage { get; }
        private QueueClient QueueClient { get; }

        public ImageReceivedWorker(ImageReceivedJob job) : base(job)
        {
            Job = job;
            TelegramClient = Factory.CreateTelegramClient();
            Storage = Factory.CreateStorageFactory().CreateStorage();
            QueueClient = Factory.CreateQueueClient();
        }

        public override void DoWork()
        {
            DownloadImage();
        }

        private async void DownloadImage()
        {
            try
            {
                var image = await TelegramClient.BotClient.GetFileAsync(Job.ImageFileId);
                using (var imageStream = new MemoryStream())
                {
                    await TelegramClient.BotClient.DownloadFileAsync(image.FilePath, imageStream);
                    var filename = Storage.SaveImage(imageStream, Storage.GenerateFileName());
                    using (QueueClient)
                    {
                        var queue = Factory.CreateQueue(QueueClient);
                        var zoomSeFacesJob = new ZoomSeFacesJob(Job.ChatId, filename);
                        queue.AddJobToQueue(zoomSeFacesJob);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}