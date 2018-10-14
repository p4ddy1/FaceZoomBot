namespace FaceZoomBot.Configuration
{
    public class MongoDB
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string AuthDatabase { get; set; }
        public string Database { get; set; }

        public MongoDB(string host, int port, string user, string password, string authDatabase, string database)
        {
            Host = host;
            Port = port;
            User = user;
            Password = password;
            Database = database;
            AuthDatabase = authDatabase;
        }
    }
}