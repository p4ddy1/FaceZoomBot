using FaceZoomBot.Workers;

namespace FaceZoomBot.Jobs
{
    public abstract class Job
    {
        public abstract Worker GetWorker();
    }
}