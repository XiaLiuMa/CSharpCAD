﻿using System;
using Max.ISolator.Core.Mods;

namespace Max.ISolator.Core
{
    /// <summary>
    /// 监听器接口
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// 消费数据
        /// </summary>
        /// <param name="idata">数据</param>
        /// <param name="callBack">回调函数</param>
        void ConsumeData(IsolatorData idata, Action<bool, string> callBack);
    }
}
