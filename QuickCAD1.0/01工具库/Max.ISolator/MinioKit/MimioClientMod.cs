namespace Max.ISolator.MinioKit
{
    /// <summary>
    /// 文件服务器配置
    /// </summary>
    public class MimioClientMod
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// endPoint是一个URL，支持[s3.amazonaws.com,s3.amazonaws.com,localhost]
        /// </summary>
        public string Endpoint { get; set; }
        /// <summary>
        ///  accessKey类似于用户ID，用于唯一标识你的账户。可选，为空代表匿名访问
        /// </summary>
        public string AccessKey { get; set; }
        /// <summary>
        ///  secretKey是你账户的密码。可选，为空代表匿名访问
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        ///  桶名称
        /// </summary>
        public string BucketName { get; set; }
        /// <summary>
        /// 最大并行数
        /// </summary>
        public int Concurrency { get; set; } = 1;
    }
}
