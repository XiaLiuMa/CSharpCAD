﻿using System.Collections.Generic;

namespace Max.DbTool.Mod
{
    /// <summary>
    /// 分页返回
    /// </summary>
    public class OutPage<T>
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 列表数据
        /// </summary>
        public List<T> Datas { get; set; }
    }
}
