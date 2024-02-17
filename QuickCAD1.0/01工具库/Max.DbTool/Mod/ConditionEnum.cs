﻿namespace Max.DbTool.Mod
{
    /// <summary>
    /// SQL条件枚举
    /// </summary>
    public enum ConditionEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal,
        /// <summary>
        /// 大于等于
        /// </summary>
        Greater,
        /// <summary>
        /// 小于等于
        /// </summary>
        Less,
        /// <summary>
        /// 区间(全封闭)
        /// </summary>
        Between,
        /// <summary>
        /// 包含
        /// </summary>
        In,
        /// <summary>
        /// 全匹配
        /// </summary>
        Like,
        /// <summary>
        /// 左匹配
        /// </summary>
        LikeLeft,
        /// <summary>
        /// 右匹配
        /// </summary>
        LikeRight,
        /// <summary>
        /// 中匹配
        /// </summary>
        LikeMiddle
    }
}
