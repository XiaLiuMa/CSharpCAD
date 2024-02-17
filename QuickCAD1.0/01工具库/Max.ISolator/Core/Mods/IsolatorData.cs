﻿using System.Collections.Generic;

namespace Max.ISolator.Core.Mods
{
    /// <summary>
    /// 隔离器数据模型
    /// </summary>
    public class IsolatorData
    {
        /// <summary>
        /// 数据唯一标识
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 场地代码(用于区分数据来源于哪个库)
        /// </summary>
        public string Cddm { get; set; }
        /// <summary>
        /// 命令标识
        /// </summary>
        public string Cmd { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Rwmc { get; set; }
        /// <summary>
        /// 触发标识(0:"ZD"--自动/1:"SD"--手动)
        /// </summary>
        public int Cfbs { get; set; }
        /// <summary>
        /// 操作类型("C":清空；"R":覆盖；"D":删除；"DR":先删除再覆盖)
        /// </summary>
        public string Czlx { get; set; } = "R";
        /// <summary>
        /// 要操作的数据
        /// </summary>
        public List<Dictionary<string, object>> Lst { get; set; }
        #region 附属属性
        /// <summary>
        /// 执行时间(需要精确到毫秒用于保证文件唯一性：yyyyMMddHHmmssfffffff)
        /// </summary>
        public string Rtime { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string Stime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string Etime { get; set; }
        /// <summary>
        /// 任务优先级(1-10级，数组越小级别越高)
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 隔离器Id
        /// </summary>
        public string IsolatorId { get; set; }
        /// <summary>
        /// 切片序号
        /// </summary>
        public int CutNum { get; set; }
        /// <summary>
        /// 切片数量
        /// </summary>
        public int CutCount { get; set; }
        #endregion
    }
}
