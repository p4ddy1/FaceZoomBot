using System;
using System.IO;
using DlibDotNet;
using FaceZoomBot.Configuration;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Models;

namespace FaceZoomBot.Workers
{
    public class ZoomSeFacesWorker : Worker
    {
        private ZoomSeFacesJob Job { get; }
        private FileSystemStorage Storage { get; }
        private QueueClient QueueClient { get; }
        private Config Config { get; }

        public ZoomSeFacesWorker(ZoomSeFacesJob job) : base(job)
        {
            Job = job;
            Storage = Factory.CreateFileSystemStorage();
            QueueClient = Factory.CreateQueueClient();
            Config = Factory.LoadConfig();
        }

        public override void DoWork()
        {
            try
            {
                int facesFound;
                using (var detector = Dlib.GetFrontalFaceDetector())
                using (var shapePredictor = ShapePredictor.Deserialize(Config.General.ShapePredictorPath))
                using (var image = Dlib.LoadImage<RgbPixel>(Path.Combine(Storage.GetBasePath(), Job.ImagePath)))
                {
                    var detections = detector.Operator(image);
                    facesFound = detections.Length;
                    for (int i = 0; i < facesFound; i++)
                    {
                        using (var shape = shapePredictor.Detect(image, detections[i]))
                        using (var faceChipDetail = Dlib.GetFaceChipDetails(shape, 700))
                        using (var faceChip = Dlib.ExtractImageChip<RgbPixel>(image, faceChipDetail))
                        {
                            Dlib.SavePng(faceChip, Storage.GenerateFacePath(Job.ImagePath));
                        }
                    }
                }

                if (facesFound < 1)
                {
                    if (Job.TelegramChat.IsPrivate)
                    {
                        SendNoFacesFoundResponse();
                    }

                    Storage.DeleteImage(Job.ImagePath);
                    return;
                }

                using (QueueClient)
                {
                    var sendFacesJob = new SendFacesJob(Job.TelegramChat, Job.ImagePath);
                    var queue = Factory.CreateQueue(QueueClient);
                    queue.AddJobToQueue(sendFacesJob);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SendNoFacesFoundResponse()
        {
            using (QueueClient)
            {
                var telegramChat = new TelegramChat(Job.TelegramChat.ChatId, true,
                    "Sorry, I was not able to find any faces in your picture.");
                var sendTestMessageJob = new SendTextMessageJob(telegramChat);
                var queue = Factory.CreateQueue(QueueClient);
                queue.AddJobToQueue(sendTestMessageJob);
            }
        }
    }
}