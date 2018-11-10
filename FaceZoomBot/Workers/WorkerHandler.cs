using System;
using System.Threading.Tasks;
using FaceZoomBot.Jobs;
using FaceZoomBot.MessageQueue;
using RabbitMQ.Client;

namespace FaceZoomBot.Workers
{
    public class WorkerHandler
    {
        private Queue Queue { get; }

        public WorkerHandler(Queue queue)
        {
            Queue = queue;
        }
        
        public async void HandleJob(Job job, IModel channel, ulong deliveryTag, bool redelivered)
        {
            var worker = job.GetWorker();
            var workerExitedSuccessfully = await Task<bool>.Factory.StartNew(worker.DoWork);
            if (workerExitedSuccessfully)
            {
                channel.BasicAck(deliveryTag, false);
                return;
            }
   
            Console.WriteLine("Job " + job.GetType() + " failed... moving back to queue...");
            
            if (!redelivered)
            {
                channel.BasicNack(deliveryTag, false, true);
                return;
            }

            channel.BasicNack(deliveryTag, false, false);
            Queue.AddJobToQueue(job, QueueClient.QueueDead);
            
            Console.WriteLine("Job " + job.GetType() + " failed again... moving to dead queue...");
        }
    }
}