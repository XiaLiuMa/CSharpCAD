﻿namespace WebApplication1.Models
{
    /// <summary>
    /// api接口返回状态类型
    /// </summary>
    public enum StatusType
    {
        General = -1,
        ParameterNull = -2,
        ParameterInvalid = -3,
        NotFound = -4,
        SysException = -5,//系统异常
        Success = 0
    }

    /// <summary>
    /// 接口返回数据格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultMsg<T>
    {
        /// <summary>
        /// 返回状态码
        /// </summary>
        public StatusType Code { get; set; }
        /// <summary>
        /// 成功或是失败消息
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 泛型类数据
        /// </summary>
        public T? Data { get; set; }
    }
}
