using System;
using System.Collections.Generic;
using System.Text;
using FaceZoomBot.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FaceZoomBot.MessageQueue
{
    public class QueueClient : IDisposable
    {
        public const string QueuePrioMid = "mid";
        public const string QueueDead = "dead";

        private ConnectionFactory ConnectionFactory { get; }
        private IConnection Connection { get; }
        private Config Config { get; }

        private Dictionary<string, IModel> ChannelDictionary { get; }
        private Dictionary<string, EventingBasicConsumer> ConsumerDictionary { get; }

        public QueueClient()
        {
            var factory = new Factory();
            Config = factory.LoadConfig();

            ConnectionFactory = new ConnectionFactory
            {
                HostName = Config.RabbitMQ.Host,
                Port = Config.RabbitMQ.Port,
                UserName = Config.RabbitMQ.User,
                Password = Config.RabbitMQ.Password
            };

            Connection = ConnectionFactory.CreateConnection();
            ChannelDictionary = new Dictionary<string, IModel>();
            ConsumerDictionary = new Dictionary<string, EventingBasicConsumer>();

            var channelMid = Connection.CreateModel();
            channelMid.QueueDeclare(
                queue: QueuePrioMid,
                durable: false,
                autoDelete: false,
                exclusive: false,
                arguments: null);

            var consumerMid = new EventingBasicConsumer(channelMid);
            
            var channelDead = Connection.CreateModel();
            channelDead.QueueDeclare(
                queue: QueueDead,
                durable: false,
                autoDelete: false,
                exclusive: false,
                arguments: null);

            var consumerDead = new EventingBasicConsumer(channelDead);
            
            ChannelDictionary.Add(QueuePrioMid, channelMid);
            ChannelDictionary.Add(QueueDead, channelDead);
            ConsumerDictionary.Add(QueuePrioMid, consumerMid);
            ConsumerDictionary.Add(QueueDead, consumerDead);
        }

        public void Publish(string message, string queue)
        {
            var body = Encoding.UTF8.GetBytes(message);
            ChannelDictionary[queue].BasicPublish(
                exchange: string.Empty,
                routingKey: queue,
                basicProperties: null,
                body: body);
        }

        public void RegisterListener(EventHandler<BasicDeliverEventArgs> listenerFunc, string queue)
        {
            ConsumerDictionary[queue].Received += listenerFunc;
            ChannelDictionary[queue].BasicConsume(
                queue, 
                false, 
                ConsumerDictionary[queue]
            );
        }

        public void Dispose()
        {
            foreach (var channel in ChannelDictionary)
            {
                channel.Value.Dispose();
            }

            Connection.Dispose();
        }
    }
}