using System.Collections.Generic;

namespace Max.DbTool.Mod
{
    /// <summary>
    /// 条件模型
    /// </summary>
    public class ConditionModel
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 条件类型
        /// </summary>
        public ConditionEnum ConditionType { get; set; }
        /// <summary>
        /// 条件值
        /// </summary>
        public List<object> ConditionValues { get; set; }
    }
}
