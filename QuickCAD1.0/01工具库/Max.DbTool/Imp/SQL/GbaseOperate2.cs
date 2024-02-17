﻿using System;
using System.Data;
using System.Data.Odbc;
using System.Collections.Generic;
using Max.BaseKit.Exts;
using Max.BaseKit;
using System.Linq;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供Gbase数据库操作实现【ODBC驱动方式，要求所在服务器必须先安装ODBC驱动--目前已不使用，仅做备份】
    /// OdbcConnection、OdbcCommand、OdbcDataAdapter、OdbcTransaction
    /// </summary>
    public class GbaseOperate2 : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public GbaseOperate2(DbConfig config) { Config = config; }
        /// <summary>
        /// 连接字符串示例1（Windows）: "host=192.168.73.229;service=25492;server=ol_gbasedbt1210_1;database=gqks;uid=gbasedbt;pwd=admin@123;Protocol=onsoctcp;Db_locale=zh_CN.utf8;Client_locale=zh_CN.utf8;GBase ODBC DRIVER (64-Bit);Server=ol_gbasedbt1210_1;"
        ///  连接字符串示例2（Linux）: "host=192.168.73.229;service=25492;server=ol_gbasedbt1210_1;database=gqks;uid=gbasedbt;pwd=admin@123;Protocol=onsoctcp;Db_locale=zh_CN.utf8;Client_locale=zh_CN.utf8;GBase ODBC DRIVER (64-Bit);Server=ol_gbasedbt1210_1;"
        /// </summary>
        /// <returns></returns>
        protected override IDbConnection GetConnection()
        {
            return new OdbcConnection(Config.ConnStr);
        }

        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)(Oracle区分字段的大小写，注意双引号来区分)
        /// 【样例1："MERGE INTO tname t USING (SELECT ? ID,? TNAME FROM dual) p ON (t.ID = p.ID) WHEN MATCHED THEN UPDATE SET t.TNAME = p.TNAME WHEN NOT MATCHED THEN INSERT (ID, TNAME) VALUES (p.ID, p.TNAME)"】
        /// <param name="tname">表名</param>
        /// <param name="keys">主键集</param>
        /// <param name="fields">字段集</param>
        protected override string GetCoverSql(string tname, List<string> keys, List<string> fields)
        {
            if (keys == null || keys.Count <= 0 || fields == null || fields.Count <= 0) return default;

            string merge_sql = string.Empty;//MERGE语句
            string using_sql = string.Empty;//USING (SELECT ? AS Id, ? AS Name, ? AS Age FROM dual) src
            string update_sql = string.Empty;//UPDATE SET p.Name = src.Name, p.Age = src.Age
            string insert_s1 = string.Empty;//insert中第1个参数语句"col1,col2,col3"
            string insert_s2 = string.Empty;//insert中第2个参数语句"p.col1,p.col2,p.col3"
            string insert_sql = string.Empty;//INSERT (Id, Name, Age) VALUES (src.Id, src.Name, src.Age)
            string on_sql = string.Empty;//ON (p.Id = src.Id)

            fields?.ForEach(item =>
            {
                using_sql += $"? AS {item},";
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

        public override bool BatchCover(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            bool flag = default;
            try
            {
                #region ①组装修改sql语句和插入sql语句
                var fieldNames = datas?.FirstOrDefault()?.Keys?.ToList(); //字段名称集[col1,col2,col3]
                if (fieldNames == null || fieldNames.Count <= 0) return false;
                string insert_p1 = string.Empty;//insert中第1个参数语句"col1,col2,col3"
                string insert_p2 = string.Empty;//insert中第2个参数语句"p.col1,p.col2,p.col3"
                string update_p1 = string.Empty;//update中第1个参数语句"t.col1=p.col1,t.col2=p.col2"
                foreach (var item in fieldNames)
                {
                    insert_p1 += $"{item},";
                    insert_p2 += $"p.{item},";
                    if (keys?.Count > 0 && !keys.Contains(item))
                    {
                        update_p1 += $"t.{item}=p.{item},";
                    }
                }
                string insert_sql = $"WHEN NOT MATCHED THEN INSERT ({insert_p1.TrimEnd(',')}) VALUES ({insert_p2.TrimEnd(',')}) ";//插入语句
                string update_sql = $"WHEN MATCHED THEN UPDATE SET {update_p1.TrimEnd(',')} ";//修改语句
                #endregion

                #region ②组装条件语句
                string where_sql = " 1=1 ";//条件语句【1=1 AND t.ID = p.ID】
                if (keys?.Count > 0)
                {
                    foreach (var item in keys)
                    {
                        where_sql += $" AND t.{item}=p.{item} ";
                    }
                }
                #endregion

                using (OdbcConnection connection = new OdbcConnection(Config.ConnStr))
                {
                    connection.Open();
                    using (OdbcTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var dic in datas)
                            {
                                #region ③组装using语句
                                string using_sql = "SELECT ";//using语句
                                foreach (var item in dic)
                                {
                                    object obj = (item.Value == null) ? DBNull.Value : item.Value;
                                    using_sql += typeof(string).Equals(obj.GetType()) ? $"'{obj}' {item.Key}, " : $"{obj} {item.Key}, ";
                                }
                                using_sql = $"{using_sql.TrimEnd(' ', ',')} FROM dual";//" SELECT :ID{0} ID,:TNAME{0} TNAME FROM dual
                                #endregion

                                #region ④组装MERGE INTO完成sql语句组装
                                string sqlstr = $"MERGE INTO {tname} t USING ({using_sql}) p ON ({where_sql}) {update_sql} {insert_sql}"; //SQL语句
                                #endregion
                                using (var cmd = new OdbcCommand(sqlstr, connection))
                                {
                                    cmd.Transaction = trans;
                                    int val = cmd.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            NLogger.Warn($"{Config?.ToJson()}>>BatchCover异常:{ex.Message}");
                            trans.Rollback();
                            flag = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn($"{Config?.ToJson()}>>BatchCover时连接异常:{ex.Message}");
                flag = false;
            }
            return flag;
        }

        public override bool BatchDelete(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            bool flag = default;
            try
            {
                using (OdbcConnection conn = new OdbcConnection(Config.ConnStr))
                {
                    conn.Open();
                    using (OdbcTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var dic in datas)
                            {
                                string where_str = string.Empty;//条件语句
                                foreach (var item in dic)
                                {
                                    if (keys.Contains(item.Key))
                                    {
                                        object obj = item.Value == null ? DBNull.Value : item.Value;
                                        where_str += typeof(string).Equals(obj.GetType()) ? $"AND {item.Key}='{obj}' " : $"AND {item.Key}={obj} ";
                                    }
                                }
                                string delete_sql = $"DELETE FROM {tname} WHERE 1=1 {where_str} ";//delete语句 
                                using (var cmd = new OdbcCommand(delete_sql, conn))
                                {
                                    cmd.Transaction = trans;
                                    int val = cmd.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            NLogger.Warn($"{Config?.ToJson()}>>BatchDelete异常:{ex.Message}");
                            trans.Rollback();
                            flag = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn($"{Config?.ToJson()}>>BatchDelete时连接异常:{ex.Message}");
                flag = false;
            }
            return flag;
        }

        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
    }
}
