namespace Max.ISolator.FtpKit
{
    /// <summary>
    /// Ftp客户端配置
    /// </summary>
    public class FtpClientMod
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// FTP服务地址
        /// </summary>
        public string ServerUrl { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 最大并行数
        /// </summary>
        public int Concurrency { get; set; } = 1;
    }
}
