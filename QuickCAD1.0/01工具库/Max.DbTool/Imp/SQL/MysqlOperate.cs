using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供Mysql数据库操作实现
    /// MySqlConnection、MySqlCommand、MySqlDataAdapter、MySqlTransaction
    /// </summary>
    public class MysqlOperate : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public MysqlOperate(DbConfig config) { Config = config; }
        protected override IDbConnection GetConnection()
        {
            return new MySqlConnection(Config.ConnStr);
        }

        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)
        /// 【样例： "replace into t (col1,col2,col3) values (@col1,@col2,@col3)";】
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">可为空</param>
        /// <param name="fields">字段集</param>
        protected override string GetCoverSql(string tname, List<string> keys, List<string> fields)
        {
            if (fields == null || fields.Count <= 0) return default;

            string replace_sql = string.Empty;//replace语句
            string replace_s1 = string.Empty;//replace中第1个参数语句"col1,col2,col3"
            string replace_s2 = string.Empty;//replace中第2个参数语句"@col1,@col2,@col3"

            fields?.ForEach(item =>
            {
                replace_s1 += $"{item},";
                replace_s2 += $"@{item},";
            });

            replace_sql = $"replace into {tname} ({replace_s1.TrimEnd(',')}) values ({replace_s2.TrimEnd(',')})";

            return replace_sql;
        }
        
        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
    }
}
