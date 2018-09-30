using System.Text;
using FaceZoomBot.Jobs;
using FaceZoomBot.Workers;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace FaceZoomBot.MessageQueue
{
    public class Queue
    {
        private QueueClient QueueClient { get; }
        private WorkerHandler WorkerHandler { get; set; }

        private JsonSerializerSettings JsonSerializerSettings { get; }

        public Queue(QueueClient queueClient)
        {
            QueueClient = queueClient;

            JsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }

        public void AddJobToQueue(Job job, string queue = QueueClient.QueuePrioMid)
        {
            var json = SerializeJob(job);
            QueueClient.Publish(json, queue);
        }

        public void ListenToQueue(WorkerHandler workerHandler)
        {
            WorkerHandler = workerHandler;
            QueueClient.RegisterListener(ReceiveFromQueue, QueueClient.QueuePrioMid);
        }

        public void ReceiveFromQueue(object sender, BasicDeliverEventArgs eventArgs)
        {
            var message = Encoding.UTF8.GetString(eventArgs.Body);
            var consumer = (EventingBasicConsumer) sender;
            var job = DeserializeJob(message);
            WorkerHandler.HandleJob(
                job, 
                consumer.Model,
                eventArgs.DeliveryTag,
                eventArgs.Redelivered
            );
        }

        private string SerializeJob(Job job)
        {
            return JsonConvert.SerializeObject(job, JsonSerializerSettings);
        }

        public Job DeserializeJob(string json)
        {
            return JsonConvert.DeserializeObject<Job>(json, JsonSerializerSettings);
        }
    }
}