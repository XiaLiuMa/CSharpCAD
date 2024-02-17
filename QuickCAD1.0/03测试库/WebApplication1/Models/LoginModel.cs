﻿namespace WebApplication1.Models
{
    /// <summary>
    /// 用户登录Model
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录密码：MD5加密
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; }
    }
}
