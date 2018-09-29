namespace FaceZoomBot.Configuration
{
    public class RabbitMQ
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public RabbitMQ(string ip, int port, string user, string password)
        {
            Ip = ip;
            Port = port;
            User = user;
            Password = password;
        }
    }
}