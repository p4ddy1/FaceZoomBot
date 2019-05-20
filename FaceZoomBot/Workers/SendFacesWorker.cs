using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.Telegram;
using SixLabors.ImageSharp.Formats.Jpeg;
using Telegram.Bot.Types;

namespace FaceZoomBot.Workers
{
    public class SendFacesWorker : Worker
    {
        private SendFacesJob Job { get; }
        private TelegramClient TelegramClient { get; set; }
        private IStorage Storage { get; set; }

        public SendFacesWorker(SendFacesJob job) : base(job)
        {
            Job = job;
        }

        public override bool DoWork()
        {
            try
            {
                TelegramClient = Factory.CreateTelegramClient();
                Storage = Factory.CreateStorage();
                SendAllFacesForImage(Job.ImagePath).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private async Task SendAllFacesForImage(string imagePath)
        {
            try
            {
                var faceImageList = new List<IAlbumInputMedia>();
                
                foreach (var face in Storage.GetAllFaceIdentifiersForImage(imagePath))
                {
                    using (var faceImage = Storage.LoadFace(imagePath, face))
                    {
                        var faceImageStream = new MemoryStream();
                        faceImage.Save(faceImageStream, new JpegEncoder());
                        faceImageStream.Seek(0, SeekOrigin.Begin);
                        faceImageList.Add(new InputMediaPhoto(new InputMedia(faceImageStream, GenerateRandomFilename())));
                    }
                }
                
                await TelegramClient.BotClient.SendMediaGroupAsync(faceImageList, Job.TelegramChat.ChatId);
                
                Storage.DeleteAllFacesForImage(Job.ImagePath);
                Storage.DeleteImage(Job.ImagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string GenerateRandomFilename()
        {
            var random = new Random();
            return "face" + random.Next(1, 1000000);
        }
    }
}