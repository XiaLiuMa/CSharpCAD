﻿namespace WebApplication1.Models
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultPaging<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 列表数量-分页使用
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 泛型类数据
        /// </summary>
        public List<T>? Data { get; set; }
    }
}
