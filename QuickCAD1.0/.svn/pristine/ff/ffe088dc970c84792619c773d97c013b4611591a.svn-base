using RabbitMQ.Client;

namespace Max.ISolator.RabbitMqKit
{
    /// <summary>
    /// RabbitMq生产者
    /// </summary>
    public class RmqProducer
    {
        /// <summary>
        /// 生产者配置
        /// </summary>
        public RmqProducerMod Mod { get; private set; }
        public RmqProducer(RmqProducerMod mod) { this.Mod = mod; }

        /// <summary>
        /// 发布数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="msgId">消息Id，本项目通常指的任务名</param>
        /// <returns></returns>
        public bool Publich(byte[] data, string msgId = "")
        {
            bool flag = default;

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
                    var properties = channel.CreateBasicProperties();
                    if (Mod.Priority >= 0) //优先级
                    {
                        properties.Priority = (byte)Mod.Priority;
                    }
                    if (Mod.Persistence) //持久化
                    {
                        properties.DeliveryMode = 2;
                    }
                    if (Mod.ExpiryTime > 0) //过期时间
                    {
                        properties.Expiration = (Mod.ExpiryTime * 1000).ToString();
                    }
                    if (!string.IsNullOrEmpty(msgId)) //消息Id
                    {
                        properties.MessageId = msgId;
                    }
                    channel.BasicPublish(exchange: Mod.ExchangeName, routingKey: Mod.RoutingKey, basicProperties: properties, body: data);
                }
            }

            return flag;
        }
    }
}
