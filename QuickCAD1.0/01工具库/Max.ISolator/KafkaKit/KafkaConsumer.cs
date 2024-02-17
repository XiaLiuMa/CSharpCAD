using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.ISolator.RabbitMqKit;
using Confluent.Kafka;
using Max.BaseKit;
using System.Threading;

namespace Max.ISolator.KafkaKit
{
    /// <summary>
    /// Kafka消费者
    /// </summary>
    public class KafkaConsumer
    {
        /// <summary>
        /// 消费者配置
        /// </summary>
        private readonly KafkaConsumerMod _Config;
        public KafkaConsumer(KafkaConsumerMod config) { _Config = config; }


        /// <summary>
        /// 初始化消费者
        /// </summary>
        public void Subscribe(Action<KafkaConsumer, string> callBack)
        {
            //var factory = new ConnectionFactory()
            //{
            //    VirtualHost = _Config.Vhost,
            //    HostName = _Config.Ip,
            //    Port = _Config.Port,
            //    UserName = _Config.Use,
            //    Password = _Config.Pwd
            //};
            //using (var connection = factory.CreateConnection())
            //{
            //    using (var channel = connection.CreateModel())
            //    {
            //        channel.ExchangeDeclare(exchange: "topicdemo.exchange", type: ExchangeType.Topic, durable: true);
            //        channel.QueueDeclare(queue: "topic_que", durable: true, exclusive: false, autoDelete: false, arguments: null);
            //        channel.QueueBind(queue: "topic_que", exchange: "topicdemo.exchange", routingKey: "#.log");
            //        var consumer = new EventingBasicConsumer(channel);
            //        consumer.Received += (s, e) =>
            //        {
            //            var body = e.Body;
            //            var data = Encoding.UTF8.GetString(body.ToArray());
            //            callBack?.Invoke(data);
            //        };
            //        channel.BasicConsume(queue: "topic_que", autoAck: true, consumer: consumer);
            //    }
            //}
        }

        /// <summary>
        /// 消费者注册监听
        /// </summary>
        public void RegisterConsumers()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    var conf = new ConsumerConfig
            //    {
            //        BootstrapServers = csr.BootstrapServers,
            //        GroupId = csr.GroupId,
            //        AutoOffsetReset = AutoOffsetReset.Earliest,
            //        EnableAutoOffsetStore = false
            //    };
            //    Action<IConsumer<Ignore, string>, Error> BuildErrorHandler = (s, e) =>
            //    {
            //        NLogger.Error($"创建Kafka消费者异常: {e.Reason}");
            //    };
            //    using (var c = new ConsumerBuilder<Ignore, string>(conf).SetErrorHandler(BuildErrorHandler).Build())
            //    {
            //        c.Subscribe(csr.Topic);//注册监听topic
            //        var cts = new CancellationTokenSource();
            //        try
            //        {
            //            while (true)
            //            {
            //                try
            //                {
            //                    var consumeResult = c.Consume(cts.Token);
            //                    string data = consumeResult.Message.Value;//接收到的数据
            //                    //TODO:kafka消息接收处理，暂时未用到                                
            //                    c.StoreOffset(consumeResult);
            //                }
            //                catch (ConsumeException e)
            //                {
            //                    NLogger.Error($"Kafka消费数据异常: {e.Error.Reason}");
            //                }
            //            }
            //        }
            //        catch (OperationCanceledException)
            //        {
            //            c.Close();
            //        }
            //    }
            //});
        }
    }
}
