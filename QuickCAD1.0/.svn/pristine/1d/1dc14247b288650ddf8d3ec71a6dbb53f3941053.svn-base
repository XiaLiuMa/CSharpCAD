using System;
using System.Data;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Max.BaseKit.Exts;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供Oracle数据库操作实现
    /// OracleConnection、OracleCommand、OracleDataAdapter、OracleTransaction
    /// </summary>
    public class OracleOperate : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public OracleOperate(DbConfig config) { Config = config; }
        protected override IDbConnection GetConnection()
        {
            return new OracleConnection(Config.ConnStr);
        }
        
        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)(Oracle区分字段的大小写，注意双引号来区分)
        /// 【样例1："MERGE INTO tname t USING (SELECT :ID ID,:TNAME TNAME FROM dual) p ON (t.ID = p.ID) WHEN MATCHED THEN UPDATE SET t.TNAME = p.TNAME WHEN NOT MATCHED THEN INSERT (ID, TNAME) VALUES (p.ID, p.TNAME)"】
        ///  【样例2："【"MERGE INTO tname t USING (SELECT :ID{0} ID,:TNAME{0} TNAME FROM dual UNION ALL SELECT :ID{1} ID,:TNAME{1} TNAME FROM dual) p ON (t.ID = p.ID) WHEN MATCHED THEN UPDATE SET t.TNAME = p.TNAME WHEN NOT MATCHED THEN INSERT (ID, TNAME) VALUES (p.ID, p.TNAME)"】;"】   
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">主键集</param>
        /// <param name="fields">字段集</param>
        protected override string GetCoverSql(string tname, List<string> keys, List<string> fields)
        {
            if (keys == null || keys.Count <= 0 || fields == null || fields.Count <= 0) return default;

            string merge_sql = string.Empty;//MERGE语句
            string using_sql = string.Empty;//USING (SELECT :Id AS Id, :Name AS Name, :Age AS Age FROM dual) src
            string update_sql = string.Empty;//UPDATE SET p.Name = src.Name, p.Age = src.Age
            string insert_s1 = string.Empty;//insert中第1个参数语句"col1,col2,col3"
            string insert_s2 = string.Empty;//insert中第2个参数语句"p.col1,p.col2,p.col3"
            string insert_sql = string.Empty;//INSERT (Id, Name, Age) VALUES (src.Id, src.Name, src.Age)
            string on_sql = string.Empty;//ON (p.Id = src.Id)

            fields?.ForEach(item =>
            {
                using_sql += $":{item} AS {item},";
                if (!keys.Contains(item.ToUpper()))//非主键
                {
                    update_sql += $"t.{item}=p.{item},";
                }
                insert_s1 += $"{item},";
                insert_s2 += $"p.{item},";
            });

            keys?.ForEach(item => { on_sql += $" AND t.{item}=p.{item} "; });

            using_sql = $"USING (SELECT {using_sql.TrimEnd(',')} FROM dual) p ";
            update_sql = $"UPDATE SET {update_sql.TrimEnd(',')} ";
            insert_sql = $"INSERT ({insert_s1.TrimEnd(',')}) VALUES ({insert_s2.TrimEnd(',')})";
            merge_sql = $"MERGE INTO {tname} t {using_sql} ON ({on_sql.TrimStart("AND")}) WHEN MATCHED THEN {update_sql} WHEN NOT MATCHED THEN {insert_sql}";

            return merge_sql;
        }

        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
    }
}
