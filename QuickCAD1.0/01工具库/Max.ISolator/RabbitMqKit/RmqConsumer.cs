using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;

namespace Max.ISolator.RabbitMqKit
{
    /// <summary>
    /// RabbitMq消费者
    /// </summary>
    public class RmqConsumer
    {
        /// <summary>
        /// 消费者配置
        /// </summary>
        public RmqConsumerMod Mod { get; private set; }
        public RmqConsumer(RmqConsumerMod mod) { this.Mod = mod; }

        /// <summary>
        /// 初始化消费者
        /// </summary>
        public void Subscribe(Action<RmqConsumer, byte[]> callBack)
        {
            var factory = new ConnectionFactory()
            {
                VirtualHost = Mod.Vhost,
                HostName = Mod.Ip,
                Port = Mod.Port,
                UserName = Mod.UserName,
                Password = Mod.Password
            };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //声明Exchange(Topic模式，持久化，不自动删除)
                    channel.ExchangeDeclare(exchange: Mod.ExchangeName, type: Mod.ExchangeType, durable: true, autoDelete: false, null);

                    IDictionary<string, object> queueParams = new Dictionary<string, object>();
                    if (Mod.Priority >= 0)//设置优先级
                    {
                        queueParams.Add("x-max-priority", Mod.Priority);
                    }
                    if (Mod.ExpiryTime > 0) //过期时间
                    {
                        queueParams.Add("x-message-ttl", (Mod.ExpiryTime * 1000).ToString());
                    }

                    channel.QueueDeclare(Mod.QueueName, true, false, false, queueParams);//声明队列，标记为持久性。
                    channel.QueueBind(Mod.QueueName, Mod.ExchangeName, Mod.RoutingKey);//绑定队列

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (s, e) =>
                    {
                        callBack?.Invoke(this, e.Body);
                        if (Mod.IsAck)
                        {
                            channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                        }
                    };
                    channel.BasicConsume(queue: Mod.QueueName, autoAck: Mod.IsAck, consumer: consumer);
                }
            }
        }
    }
}
