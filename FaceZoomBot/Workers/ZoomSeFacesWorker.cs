using System;
using DlibDotNet;
using FaceZoomBot.Configuration;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using FaceZoomBot.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace FaceZoomBot.Workers
{
    public class ZoomSeFacesWorker : Worker
    {
        private ZoomSeFacesJob Job { get; }
        private IStorage Storage { get; }
        private QueueClient QueueClient { get; }
        private Config Config { get; }

        public ZoomSeFacesWorker(ZoomSeFacesJob job) : base(job)
        {
            Job = job;
            Storage = Factory.CreateStorage();
            QueueClient = Factory.CreateQueueClient();
            Config = Factory.LoadConfig();
        }

        public override bool DoWork()
        {
            try
            {
                int facesFound;

                using (var detector = Dlib.GetFrontalFaceDetector())
                using (var shapePredictor = ShapePredictor.Deserialize(Config.General.ShapePredictorPath))
                using (var image = Storage.LoadImage(Job.ImagePath))
                {
                    var convertedImage = ImageToArray2D(image);
                    var detections = detector.Operator(convertedImage);
                    facesFound = detections.Length;
                    for (int i = 0; i < facesFound; i++)
                    {
                        using (var shape = shapePredictor.Detect(convertedImage, detections[i]))
                        using (var faceChipDetail = Dlib.GetFaceChipDetails(shape, 700))
                        using (var faceChip = Dlib.ExtractImageChip<RgbPixel>(convertedImage, faceChipDetail))
                        {
                            var face = Array2DToImage(faceChip);
                            Storage.SaveFace(face, Job.ImagePath);
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
                    return true;
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
                return false;
            }

            return true;
        }

        private Array2D<RgbPixel> ImageToArray2D(Image<Rgb24> image)
        {
            var width = image.Width;
            var height = image.Height;

            var array2D = new Array2D<RgbPixel>(height, width);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    array2D[row][col] = new RgbPixel(
                        image[col, row].R,
                        image[col, row].G,
                        image[col, row].B
                    );
                }
            }

            return array2D;
        }

        private Image<Rgb24> Array2DToImage(Array2D<RgbPixel> array2D)
        {
            var width = array2D.Columns;
            var height = array2D.Rows;

            var image = new Image<Rgb24>(width, height);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    image[col, row] = new Rgb24(
                        array2D[row][col].Red,
                        array2D[row][col].Green,
                        array2D[row][col].Blue
                    );
                }
            }

            return image;
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