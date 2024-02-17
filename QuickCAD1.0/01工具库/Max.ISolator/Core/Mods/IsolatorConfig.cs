namespace Max.ISolator.Core.Mods
{
    /// <summary>
    /// 隔离器公用配置
    /// </summary>
    public class IsolatorConfig
    {
        /// <summary>
        /// 隔离器模式(0:双向模式，1:生产者模式，2：消费者模式)
        /// </summary>
        public int Mode { get; set; } = 0;
        /// <summary>
        /// 生产文件缓存路径
        /// </summary>
        public string ProduceCachePath { get; set; }
        /// <summary>
        /// 是否备份生产文件
        /// </summary>
        public bool IsBackupProduce { get; set; }
        /// <summary>
        /// 生产文件备份路径
        /// </summary>
        public string ProduceBackupPath { get; set; }
        /// <summary>
        /// 消费文件缓存路径
        /// </summary>
        public string ConsumeCachePath { get; set; }
        /// <summary>
        /// 是否备份消费文件
        /// </summary>
        public bool IsBackupConsume { get; set; }
        /// <summary>
        /// 消费文件备份路径
        /// </summary>
        public string ConsumeBackupPath { get; set; }
        /// <summary>
        /// 管道大小，用于控制并行数
        /// </summary>
        public int ChannelSize { get; set; } = 1;
        /// <summary>
        /// 数量大小(单个文件可容纳多少条数据)
        /// </summary>
        public int ItemSize { get; set; } = 1000;
        /// <summary>
        /// 压缩前数据容积(单个文件可容纳多大的数据，单位M)
        /// </summary>
        public int BulkSize { get; set; } = 2;
        /// <summary>
        /// 是否压缩数据
        /// </summary>
        public bool IsZip { get; set; }
    }
}
