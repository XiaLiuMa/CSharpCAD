using System.Data;

namespace Max.BaseKit.Utils
{
    /// <summary>
    /// 计算工具
    /// </summary>
    public static class CalcUtil
    {
        /// <summary>
        /// 由DataTable计算公式
        /// </summary>
        /// <param name="expression">表达式【"1+2*(4-3)+15/50"】</param>
        /// <param name="filter">过滤条件，默认无</param>
        /// <returns></returns>
        public static object CalcByDataTable(string expression, string filter ="")
        {
            object result = new DataTable().Compute(expression, filter);
            return result;
        }
    }
}
