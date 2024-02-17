using Dm;
using System;
using Max.BaseKit.Exts;
using System.Data;
using System.Collections.Generic;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供达梦数据库操作实现
    /// DmConnection、DmCommand、DmDataReader、DmTransaction
    /// </summary>
    public class DamengOperate : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public DamengOperate(DbConfig config) { Config = config; }
        protected override IDbConnection GetConnection()
        {
            return new DmConnection(Config.ConnStr);
        }

        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)
        /// 【样例1："DECLARE v_count NUMBER;BEGIN SELECT COUNT(*) INTO v_count FROM TEST01 WHERE COL1  = :COL1;IF v_count > 0 THEN UPDATE TEST01 SET IMG = :IMG WHERE COL1 = :COL1;ELSE INSERT INTO TEST01 (COL1, IMG) VALUES (:COL1, :IMG);END IF;END;"】 
        /// 备注：也可以像Oracle那种形式来使用，但是用那种形式无法处理大图片。
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">主键集</param>
        /// <param name="fields">字段集</param>
        protected override string GetCoverSql(string tname, List<string> keys, List<string> fields)
        {
            if (keys == null || keys.Count <= 0 || fields == null || fields.Count <= 0) return default;

            string process_sql = string.Empty;//存储过程语句
            string update_sql = string.Empty;//UPDATE TEST01 SET Name = :Name, Age = :Age
            string insert_s1 = string.Empty;//insert中第1个参数语句"col1,col2,col3"
            string insert_s2 = string.Empty;//insert中第2个参数语句":col1,:col2,:col3"
            string insert_sql = string.Empty;//INSERT INTO TEST01 (COL1, IMG) VALUES (:COL1, :IMG)
            string on_sql = string.Empty;//ON (p.Id = src.Id)

            fields?.ForEach(item =>
            {
                if (!keys.Contains(item.ToUpper()))//非主键
                {
                    update_sql += $"{item}=:{item},";
                }
                insert_s1 += $"{item},";
                insert_s2 += $":{item},";
            });

            keys?.ForEach(item => { on_sql += $" AND {item}=:{item} "; });

            update_sql = $"UPDATE {tname} SET {update_sql.TrimEnd(',')} WHERE {on_sql.TrimStart("AND")} ";
            insert_sql = $"INSERT INTO {tname} ({insert_s1.TrimEnd(',')}) VALUES ({insert_s2.TrimEnd(',')}) ";
            process_sql = $"DECLARE v_count NUMBER;BEGIN SELECT COUNT(*) INTO v_count FROM {tname} WHERE {on_sql.TrimStart("AND")};IF v_count > 0 THEN {update_sql};ELSE {insert_sql};END IF;END;";

            return process_sql;
        }

        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
    }
}
