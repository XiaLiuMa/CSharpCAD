﻿namespace Max.ISolator.KafkaKit
{
    /// <summary>
    /// Kafka消费者配置
    /// </summary>
    public class KafkaConsumerMod
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        public string BootstrapServers { get; set; }
        /// <summary>
        /// 消费用GroupId
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 消费用Topic
        /// </summary>
        public string Topic { get; set; }
    }
}

