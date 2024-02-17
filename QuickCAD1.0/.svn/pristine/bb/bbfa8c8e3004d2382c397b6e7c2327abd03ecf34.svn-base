namespace Max.ISolator.RabbitMqKit
{
    /// <summary>
    /// RabbitMQ消费者配置
    /// </summary>
    public class RmqConsumerMod
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Id { get; set; }
        #region 基础配置
        /// <summary>
        /// 虚拟地址
        /// </summary>
        public string Vhost { get; set; }
        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Exchange名称
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// Exchange类型
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 路由密钥
        /// </summary>
        public string RoutingKey { get; set; }
        #endregion
        /// <summary>
        /// 是否需要接收确认
        /// </summary>
        public bool IsAck { get; set; }
        /// <summary>
        /// 优先级(0-9),默认为-1，也就是不启用优先级
        /// </summary>
        public int Priority { get; set; } = -1;
        /// <summary>
        /// 过期时间(单位：秒),默认为0也就是不设置过期时间
        /// </summary>
        public int ExpiryTime { get; set; } = 0;
        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }
    }
}
