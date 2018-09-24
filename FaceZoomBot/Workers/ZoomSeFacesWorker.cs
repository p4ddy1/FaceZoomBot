using System;
using System.IO;
using DlibDotNet;
using FaceZoomBot.DataStorage;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;

namespace FaceZoomBot.Workers
{
    public class ZoomSeFacesWorker : Worker
    {
        private ZoomSeFacesJob Job { get; }
        private string ShapePath { get; }
        private IStorage Storage { get; }
        private QueueClient QueueClient { get; }

        public ZoomSeFacesWorker(ZoomSeFacesJob job) : base(job)
        {
            Job = job;
            ShapePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "shape_predictor_5_face_landmarks.dat");
            Storage = Factory.CreateStorageFactory().CreateStorage();
            QueueClient = Factory.CreateQueueClient();
        }

        public override void DoWork()
        {
            try
            {
                int facesFound;
                using (var detector = Dlib.GetFrontalFaceDetector())
                using (var shapePredictor = ShapePredictor.Deserialize(ShapePath))
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
                    return;
                
                using (QueueClient)
                {
                    var sendFacesJob = new SendFacesJob(Job.ChatId, Job.ImagePath);
                    var queue = Factory.CreateQueue(QueueClient);
                    queue.AddJobToQueue(sendFacesJob);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}