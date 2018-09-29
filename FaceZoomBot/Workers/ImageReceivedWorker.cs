using System;
using System.IO;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Telegram;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

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
            Storage = Factory.CreateStorage();
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
                var imageFile = await TelegramClient.BotClient.GetFileAsync(Job.ImageFileId);
                using (var imageStream = new MemoryStream())
                {
                    await TelegramClient.BotClient.DownloadFileAsync(imageFile.FilePath, imageStream);
                    var identifier = Storage.GetRandomIdentifier();
                    var image = Image.Load<Rgb24>(imageStream, new JpegDecoder());
                    Storage.SaveImage(image, identifier);
                    using (QueueClient)
                    {
                        var queue = Factory.CreateQueue(QueueClient);
                        var zoomSeFacesJob = new ZoomSeFacesJob(Job.TelegramChat, identifier);
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