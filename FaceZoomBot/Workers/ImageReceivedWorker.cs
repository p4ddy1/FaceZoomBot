using System;
using System.IO;
using System.Threading.Tasks;
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
        private TelegramClient TelegramClient { get; set; }
        private IStorage Storage { get; set; }
        private QueueClient QueueClient { get; set; }

        public ImageReceivedWorker(ImageReceivedJob job) : base(job)
        {
            Job = job;
        }

        public override bool DoWork()
        {
            try
            {
                TelegramClient = Factory.CreateTelegramClient();
                Storage = Factory.CreateStorage();
                QueueClient = Factory.CreateQueueClient();
                DownloadImage().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
            return true;
        }

        private async Task DownloadImage()
        {
            var imageFile = await TelegramClient.BotClient.GetFileAsync(Job.ImageFileId);
            using (var imageStream = new MemoryStream())
            {
                await TelegramClient.BotClient.DownloadFileAsync(imageFile.FilePath, imageStream);
                var identifier = Storage.GetRandomIdentifier();
                using (var image = Image.Load<Rgb24>(imageStream, new JpegDecoder()))
                {
                    Storage.SaveImage(image, identifier);
                }
                using (QueueClient)
                {
                    var queue = Factory.CreateQueue(QueueClient);
                    var zoomSeFacesJob = new ZoomSeFacesJob(Job.TelegramChat, identifier);
                    queue.AddJobToQueue(zoomSeFacesJob);
                }
            }
        }
    }
}