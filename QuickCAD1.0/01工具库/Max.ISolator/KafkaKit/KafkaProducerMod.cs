namespace Max.ISolator.KafkaKit
{
    /// <summary>
    /// Kafka生产者配置
    /// </summary>
    public class KafkaProducerMod
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
        /// 生产用Topic
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 消息大小(M)
        /// </summary>
        public int MaxSize { get; set; } = 2;
        /// <summary>
        /// 最大并行数
        /// </summary>
        public int Concurrency { get; set; } = 1;
    }
}

