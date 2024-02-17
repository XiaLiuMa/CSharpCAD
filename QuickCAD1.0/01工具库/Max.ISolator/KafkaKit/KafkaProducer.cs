using Confluent.Kafka;
using Max.BaseKit;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Max.ISolator.KafkaKit
{
    /// <summary>
    /// Kafka生产者
    /// </summary>
    public class KafkaProducer
    {
        /// <summary>
        /// 消费者配置
        /// </summary>
        private readonly KafkaProducerMod _Config;
        public KafkaProducer(KafkaProducerMod config) { _Config = config; }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="idata"></param>
        /// <returns></returns>
        public bool Publich(string data)
        {
            bool flag = default;

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
            //        channel.ExchangeDeclare("topicdemo.exchange", ExchangeType.Topic, true, false, null);
            //        Console.WriteLine("输入需要传输的消息，输入Exit退出");
            //        var message = Console.ReadLine();
            //        while (message != "Exit")
            //        {
            //            Console.WriteLine("输入RouteKey");
            //            var routekey = Console.ReadLine();
            //            var body = Encoding.UTF8.GetBytes(message);

            //            channel.BasicPublish(exchange: "topicdemo.exchange", routingKey: routekey, basicProperties: null, body: body);
            //            Console.WriteLine(" 发送消息 {0} Routekey {1}", message, routekey);
            //            message = Console.ReadLine();
            //        }
            //    }
            //    Console.WriteLine("按回车退出");
            //    Console.ReadLine();
            //}

            return flag;
        }


        /// <summary>
        /// 发布数据
        /// </summary>
        /// <param name="data"></param>
        public void PublichData(string data)
        {
            List<bool> flags = new List<bool>();
            for (int i = 0; i < 3; i++)
            {
                bool flag = PublichDataBALL(data);
                flags.Add(flag);
                if (flag) break;
                Thread.Sleep(200);
                string sbms = (i != 0) ? "，尝试重新发布到Kafka。" : "。";
                NLogger.Warn($"Kafka>>发布到Kafka失败{sbms}");
            }
            var rflag = flags.Contains(true) ? true : false;
            if (!rflag) NLogger.Error($"Kafka>>发布到Kafka结果{rflag}");
        }


        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="idata"></param>
        /// <param name="dbcmd"></param>
        /// <param name="sqllst"></param>
        /// <returns></returns>
        private bool PublichDataBALL(string data)
        {
            bool flag = default;
            //var config = new ProducerConfig
            //{
            //    BootstrapServers = url,
            //    MessageMaxBytes = maxsize * 1024 * 1024,
            //    BatchSize = maxsize * 1024 * 1024
            //};

            //IProducer<Null, string> p = default;
            //try
            //{
            //    using (p = new ProducerBuilder<Null, string>(config).Build())
            //    {
            //        try
            //        {
            //            p.Produce(sctopic, new Message<Null, string> { Value = data });
            //            p.Flush(TimeSpan.FromSeconds(5));
            //            flag = true;
            //        }
            //        catch (ProduceException<Null, string> e)
            //        {
            //            NLogger.Warn($"Kafka发布数据异常: {e.Error.Reason}，数据大小为{Encoding.UTF8.GetBytes(data).Length}");
            //            flag = false;
            //        }
            //    }
            //    p?.Dispose();//手动销毁，尝试解决“每个套接字只允许使用一次的情况”。
            //}
            //catch (Exception ex)
            //{
            //    NLogger.Warn($"创建Kafka生产者异常: {ex.Message}");
            //    flag = false;
            //}
            //finally
            //{
            //    p?.Dispose();//手动销毁，尝试解决“每个套接字只允许使用一次的情况”。
            //}
            return flag;
        }
    }
}
