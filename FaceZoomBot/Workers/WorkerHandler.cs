using System.Threading.Tasks;
using FaceZoomBot.Jobs;

namespace FaceZoomBot.Workers
{
    public class WorkerHandler
    {
        public void HandleJob(Job job)
        {
            var worker = job.GetWorker();
            Task.Factory.StartNew(worker.DoWork);
        }
    }
}