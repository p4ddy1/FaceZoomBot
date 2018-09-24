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
                HostName = Config.RabbitMq.Ip,
                Port = Config.RabbitMq.Port,
                UserName = Config.RabbitMq.User,
                Password = Config.RabbitMq.Password
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

            ChannelDictionary.Add(QueuePrioMid, channelMid);
            ConsumerDictionary.Add(QueuePrioMid, consumerMid);
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
            ChannelDictionary[queue].BasicConsume(queue, true, ConsumerDictionary[queue]);
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