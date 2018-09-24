using System;
using System.IO;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.Telegram;
using Telegram.Bot.Types.InputFiles;

namespace FaceZoomBot.Workers
{
    public class SendFacesWorker : Worker
    {
        private SendFacesJob Job { get; }
        private TelegramClient TelegramClient { get; }
        private IStorage Storage { get; }

        public SendFacesWorker(SendFacesJob job) : base(job)
        {
            Job = job;
            TelegramClient = Factory.CreateTelegramClient();
            Storage = Factory.CreateStorageFactory().CreateStorage();
        }

        public override void DoWork()
        {
            SendAllFacesForImage(Job.ImagePath);
        }

        private async void SendAllFacesForImage(string imagePath)
        {
            try
            {
                var facesBasePath = Storage.GetFaceBasePath(imagePath);
                foreach (var facePath in Directory.GetFiles(facesBasePath))
                {
                    using (var faceFileStream = File.OpenRead(facePath))
                    {
                        var inputOnlineFile = new InputOnlineFile(faceFileStream);
                        await TelegramClient.BotClient.SendPhotoAsync(Job.ChatId, inputOnlineFile);
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