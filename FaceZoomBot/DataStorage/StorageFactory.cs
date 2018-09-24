namespace FaceZoomBot.DataStorage
{
    public class StorageFactory
    {
        public IStorage CreateStorage()
        {
            return new FileSystemStorage();
        }
    }
}