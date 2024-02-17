using System;
using System.Data;
using Max.BaseKit.Exts;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供Sqlite数据库操作实现【Microsoft.Data.Sqlite驱动】，此驱动支持ARM系统
    /// SqliteConnection、SqliteCommand、SqliteDataReader、SqliteTransaction
    /// 备注：【System.Data.SQLite驱动】也可以实现，只是不支持ARM系统
    /// </summary>
    public class SqliteOperate : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public SqliteOperate(DbConfig config) { Config = config; }
        protected override IDbConnection GetConnection()
        {
            return new SqliteConnection(Config.ConnStr);
        }

        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)
        /// 【样例："INSERT INTO Person (Id, Name, Age) VALUES(@Id, @Name, @Age) ON CONFLICT(Id) DO UPDATE SET Name = excluded.Name,Age = excluded.Age";】
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">可为空</param>
        /// <param name="fields">字段集</param>
        protected override string GetCoverSql(string tname, List<string> keys, List<string> fields)
        {
            if (fields == null || fields.Count <= 0) return default;

            string insert_s1 = string.Empty;//replace中第1个参数语句"col1,col2,col3"
            string insert_s2 = string.Empty;//replace中第2个参数语句"@col1,@col2,@col3"
            string insert_sql = string.Empty;//INSERT INTO Person (Id, Name, Age) VALUES (@Id, @Name, @Age)
            string update_sql = string.Empty;//UPDATE SET p.Name = src.Name, p.Age = src.Age
            string on_sql = string.Empty;//ON (Id,Age)

            fields?.ForEach(item =>
            {
                insert_s2 += $"{item},";
                insert_s2 += $"@{item},";
                if (!keys.Contains(item.ToUpper()))//非主键
                {
                    update_sql += $"{item}=excluded.{item},";
                }
            });
            keys?.ForEach(item => { on_sql += $"{item},"; });

            insert_sql = $"INSERT INTO Person ({insert_s1.TrimEnd(",")}) VALUES ({insert_s2.TrimEnd(",")}) ";
            insert_sql = $"{insert_sql} ON CONFLICT({on_sql.TrimEnd(",")}) DO UPDATE SET {update_sql.TrimEnd(",")}";

            return insert_sql;
        }
        
        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
    }
}