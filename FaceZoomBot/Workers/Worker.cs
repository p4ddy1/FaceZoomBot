using FaceZoomBot.Jobs;

namespace FaceZoomBot.Workers
{
    public abstract class Worker
    {
        private Job Job { get; }
        protected Factory Factory { get; }

        protected Worker(Job job)
        {
            Job = job;
            Factory = new Factory();
        }

        public abstract void DoWork();
    }
}