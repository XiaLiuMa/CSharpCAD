using System;
using System.Data;
using System.Collections.Generic;
using Max.BaseKit.Exts;
using Max.BaseKit;
using System.Linq;
using GBS.Data.GBasedbt;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供Gbase数据库操作实现【GBS.Data.GBasedbt驱动方式，无需服务器安装驱动】
    /// GbsConnection、GbsCommand、GbsDataAdapter、GbsTransaction
    /// </summary>
    public class GbaseOperate : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public GbaseOperate(DbConfig config) { Config = config; }
        /// <summary>
        /// 连接字符串示例: "host=192.168.73.229;service=25492;server=ol_gbasedbt1210_1;database=gqks;uid=gbasedbt;pwd=admin@123;Protocol=onsoctcp;Db_locale=zh_CN.utf8;Client_locale=zh_CN.utf8;Server=ol_gbasedbt1210_1;"
        /// </summary>
        /// <returns></returns>
        protected override IDbConnection GetConnection()
        {
            return new GbsConnection(Config.ConnStr);
        }

        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)
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
            bool found = false; //查找是否存在图片处理
            foreach (var dic in datas)
            {
                foreach (var item in dic)
                {
                    object obj = (item.Value == null) ? DBNull.Value : item.Value;
                    if (typeof(byte[]).Equals(obj.GetType()))
                    {
                        found = true;
                        break; // 跳出内层循环
                    }
                    if (found)
                    {
                        break; // 跳出外层循环
                    }
                }
            }

            if (found)
            {
                return BatchCover1(tname, keys, datas);
            }
            else
            {
                return BatchCover2(tname, keys, datas);
            }
        }

        /// <summary>
        /// 批量覆盖语句(先查询有没有，有就修改，没有就新增)【先查询看有没有，再进行新增或修改操作】
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">主键字段集</param>
        /// <param name="dlst">数据集</param>
        private bool BatchCover1(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            bool flag = default;
            try
            {
                using (GbsConnection connection = new GbsConnection(Config.ConnStr))
                {
                    connection.Open();
                    using (GbsTransaction trans = connection.BeginTransaction())
                    {
                        foreach (var dic in datas)
                        {
                            int count = 0; //用于判断数据是否存在
                            try
                            {
                                #region 1、查询数据是否存在
                                using (var cmd = connection.CreateCommand())
                                {
                                    string where_p = string.Empty;//where参数语句"col1='111' and col2='222'"
                                    var where_parameters = new List<GbsParameter>();
                                    foreach (var item in dic)
                                    {
                                        object obj = item.Value == null ? DBNull.Value : item.Value;
                                        if (keys.Contains(item.Key))
                                        {
                                            where_p += $"{item.Key}=? AND";
                                            if (typeof(byte[]).Equals(obj.GetType()))
                                            {
                                                var param = new GbsParameter("?", GbsType.Blob);
                                                param.SourceColumn = item.Key;
                                                param.Value = obj;
                                                where_parameters.Add(param);
                                            }
                                            else
                                            {
                                                var param = new GbsParameter("?", obj);
                                                param.SourceColumn = item.Key;
                                                where_parameters.Add(param);
                                            }
                                        }
                                    }
                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddRange(where_parameters.ToArray());
                                    cmd.CommandText = $"SELECT COUNT(*) FROM {tname} WHERE {where_p.TrimEnd('D', 'N', 'A')}";
                                    decimal r_count = (decimal)cmd.ExecuteScalar();
                                    count = (int)r_count;
                                }
                                #endregion

                                if (count > 0) //数据存在
                                {
                                    #region 2、修改数据
                                    using (var cmd = connection.CreateCommand())
                                    {
                                        string update_p = string.Empty;//update参数语句"col1='111',col2='222'"
                                        var update_parameters = new List<GbsParameter>();//修改参数，因为有顺序，所以需要单独建按顺序插入
                                        string where_p = string.Empty;//where参数语句"col1='111' and col2='222'"
                                        var where_parameters = new List<GbsParameter>();//条件参数，因为有顺序，所以需要单独建按顺序插入
                                        foreach (var item in dic)
                                        {
                                            object obj = item.Value == null ? DBNull.Value : item.Value;
                                            if (keys.Contains(item.Key))
                                            {
                                                where_p += $"{item.Key}=? AND";
                                                if (typeof(byte[]).Equals(obj.GetType()))
                                                {
                                                    var param = new GbsParameter("?", GbsType.Blob);
                                                    param.SourceColumn = item.Key;
                                                    param.Value = obj;
                                                    where_parameters.Add(param);
                                                }
                                                else
                                                {
                                                    var param = new GbsParameter("?", obj);
                                                    param.SourceColumn = item.Key;
                                                    where_parameters.Add(param);
                                                }
                                            }
                                            else
                                            {
                                                update_p += $"{item.Key}=? ,";
                                                if (typeof(byte[]).Equals(obj.GetType()))
                                                {
                                                    var param = new GbsParameter("?", GbsType.Blob);
                                                    param.SourceColumn = item.Key;
                                                    param.Value = obj;
                                                    update_parameters.Add(param);
                                                }
                                                else
                                                {
                                                    var param = new GbsParameter("?", obj);
                                                    param.SourceColumn = item.Key;
                                                    update_parameters.Add(param);
                                                }
                                            }
                                        }
                                        cmd.Transaction = trans;
                                        cmd.Parameters.AddRange(update_parameters.ToArray());
                                        cmd.Parameters.AddRange(where_parameters.ToArray());
                                        cmd.CommandText = $" UPDATE {tname} SET {update_p.TrimEnd(',')} WHERE {where_p.TrimEnd('D', 'N', 'A')}";//修改语句
                                        int val = cmd.ExecuteNonQuery();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 3、插入数据
                                    using (var cmd = connection.CreateCommand())
                                    {
                                        string insert_p1 = string.Empty;//insert中第1个参数语句"col1,col2,col3"
                                        string insert_p2 = string.Empty;//insert中第2个参数语句"?,?,?"
                                        var parameters = new List<GbsParameter>();
                                        foreach (var item in dic)
                                        {
                                            insert_p1 += $"{item.Key},";
                                            insert_p2 += $"?,";
                                            object obj = item.Value == null ? DBNull.Value : item.Value;
                                            if (typeof(byte[]).Equals(obj.GetType()))
                                            {
                                                var param = new GbsParameter("?", GbsType.Blob);
                                                param.SourceColumn = item.Key;
                                                param.Value = obj;
                                                parameters.Add(param);
                                            }
                                            else
                                            {
                                                parameters.Add(new GbsParameter("?", obj));
                                            }
                                        }
                                        cmd.Transaction = trans;
                                        cmd.Parameters.AddRange(parameters.ToArray());
                                        cmd.CommandText = $"INSERT INTO {tname} ({insert_p1.TrimEnd(',')}) VALUES ({insert_p2.TrimEnd(',')})";//插入语句
                                        int val = cmd.ExecuteNonQuery();
                                    }
                                    #endregion
                                }
                            }
                            catch (Exception ex)
                            {
                                NLogger.Warn($"GBase执行{Config.Id}的BatchCover异常:{ex.Message}");
                                trans.Rollback();
                                flag = false;
                            }
                        }
                        trans.Commit();
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn($"GBase执行{Config.Id}的BatchCover时连接异常:{ex.Message}");
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 批量覆盖语句(有就修改，没有就新增)【GBase厂家提供的驱动暂时还不支持该语句图片处理，所以只能处理非图片数据】
        /// 【样例1："MERGE INTO tname t USING (SELECT :ID ID,:TNAME TNAME FROM dual) p ON (t.ID = p.ID) WHEN MATCHED THEN UPDATE SET t.TNAME = p.TNAME WHEN NOT MATCHED THEN INSERT (ID, TNAME) VALUES (p.ID, p.TNAME)"】
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">主键字段集</param>
        /// <param name="dlst">数据集</param>
        private bool BatchCover2(string tname, List<string> keys, List<IDictionary<string, object>> datas)
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

                using (GbsConnection connection = new GbsConnection(Config.ConnStr))
                {
                    connection.Open();
                    using (GbsTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (var dic in datas)
                            {
                                using (var cmd = connection.CreateCommand())
                                {
                                    #region ③组装using语句
                                    string using_sql = "SELECT ";//using语句
                                    var parameters = new List<GbsParameter>();
                                    foreach (var item in dic)
                                    {
                                        object obj = (item.Value == null) ? DBNull.Value : item.Value;
                                        using_sql += item.Value == null ? $"null {item.Key}, " : typeof(string).Equals(item.Value.GetType()) ? $"'{item.Value}' {item.Key}, " : $"{item.Value} {item.Key}, ";
                                        #region GBase厂家提供的驱动暂时还不支持该语句图片处理
                                        //using_sql += item.Value == null ? $"null {item.Key}, " : typeof(string).Equals(item.Value.GetType()) ? $"'{item.Value}' {item.Key}, " : typeof(byte[]).Equals(item.Value.GetType()) ? $"? {item.Key}, " : $"{item.Value} {item.Key}, ";
                                        //if (typeof(byte[]).Equals(obj.GetType()))
                                        //{
                                        //    var param = new GbsParameter("?", GbsType.Blob);
                                        //    param.SourceColumn = item.Key;
                                        //    param.Value = obj;
                                        //    cmd.Parameters.Add(param);
                                        //} 
                                        #endregion
                                    }
                                    using_sql = $"{using_sql.TrimEnd(' ', ',')} FROM dual";//" SELECT :ID{0} ID,:TNAME{0} TNAME FROM dual
                                    #endregion

                                    #region ④组装MERGE INTO完成sql语句组装
                                    string sqlstr = $"MERGE INTO {tname} t USING ({using_sql}) p ON ({where_sql}) {update_sql} {insert_sql}"; //SQL语句
                                    #endregion

                                    cmd.CommandText = sqlstr;
                                    cmd.Transaction = trans;
                                    //cmd.Parameters.AddRange(parameters.ToArray());
                                    int val = cmd.ExecuteNonQuery();
                                }
                            }
                            trans.Commit();
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            NLogger.Warn($"GBase执行{Config.Id}的BatchCover异常:{ex.Message}");
                            trans.Rollback();
                            flag = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn($"GBase执行{Config.Id}的BatchCover时连接异常:{ex.Message}");
                flag = false;
            }
            return flag;
        }

        public override bool BatchDelete(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            bool flag = default;
            try
            {
                using (GbsConnection conn = new GbsConnection(Config.ConnStr))
                {
                    conn.Open();
                    using (GbsTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var dic in datas)
                            {
                                using (var cmd = conn.CreateCommand())
                                {
                                    string where_str = string.Empty;//条件语句"col1=? AND col2=?"
                                    var parameters = new List<GbsParameter>();
                                    foreach (var item in dic)
                                    {
                                        object obj = (item.Value == null) ? DBNull.Value : item.Value;
                                        if (keys.Contains(item.Key))
                                        {
                                            where_str += $" AND {item.Key}=?";
                                            parameters.Add(new GbsParameter("?", obj));
                                        }
                                    }

                                    cmd.Transaction = trans;
                                    cmd.Parameters.AddRange(parameters.ToArray());
                                    cmd.CommandText = $"DELETE FROM {tname} WHERE 1=1 {where_str} ";//delete语句
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
