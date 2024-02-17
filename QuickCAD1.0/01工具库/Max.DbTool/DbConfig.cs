﻿namespace Max.DbTool
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class DbConfig
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 场地代码(用于区分数据来源)
        /// </summary>
        public string Cddm { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; set; }
        /// <summary>
        /// 最大并行连接数
        /// </summary>
        public int Concurrency { get; set; } = 1;
        /// <summary>
        /// 驱动扩展配置
        /// </summary>
        public string DriverEx { get; set; }
        /// <summary>
        /// 数据库描述
        /// </summary>
        public string DbDescribe { get; set; }
    }
}
