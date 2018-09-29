using System;
using System.IO;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.Telegram;
using SixLabors.ImageSharp.Formats.Jpeg;
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
            Storage = Factory.CreateStorage();
        }

        public override void DoWork()
        {
            SendAllFacesForImage(Job.ImagePath);
            //Storage.DeleteImage(Job.ImagePath);
        }

        private async void SendAllFacesForImage(string imagePath)
        {
            try
            {
                foreach (var face in Storage.GetAllFaceIdentifiersForImage(imagePath))
                {
                    using (var faceImage = Storage.LoadFace(imagePath, face))
                    using (var faceImageStream = new MemoryStream())
                    {
                        faceImage.Save(faceImageStream, new JpegEncoder());
                        faceImageStream.Seek(0, SeekOrigin.Begin);
                        var inputOnlineFile = new InputOnlineFile(faceImageStream);
                        await TelegramClient.BotClient.SendPhotoAsync(Job.TelegramChat.ChatId, inputOnlineFile);
                    }
                }
                Storage.DeleteAllFacesForImage(Job.ImagePath);
                Storage.DeleteImage(Job.ImagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}