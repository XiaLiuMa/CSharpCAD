using System;
using Dapper;
using System.Linq;
using Max.BaseKit;
using System.Text;
using System.Data;
using Max.BaseKit.Exts;
using System.Collections.Generic;
using Max.DbTool.Mod;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 基于Dapper的抽象SQL数据库操作
    /// 1、实体T必须基于Dapper配置。
    /// 2、Dapper不支持的数据库请单独实现。
    /// </summary>
    public abstract class AbsSqlOperate : AbsDbOperate
    {
        /// <summary>
        /// 获取数据库连接
        /// </summary>
        /// <returns></returns>
        protected abstract IDbConnection GetConnection();
        /// <summary>
        /// 获取覆盖语句
        /// </summary>
        /// <param name="tname">表名</param>
        /// <param name="keys">主键集</param>
        /// <param name="fields">字段集</param>
        /// <returns></returns>
        protected abstract string GetCoverSql(string tname, List<string> keys, List<string> fields);

        /// <summary>
        /// 尝试连接数据库
        /// </summary>
        /// <returns></returns>
        public override bool TryConnect()
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"Mysql执行{Config?.ToJson()}的TryConnect异常:{ex.Message}");
                    return false;
                }
            }
        }
        public override List<string> QueryTableNames()
        {
            //TODO:
            throw new NotImplementedException();
        }
        /// <summary>
        /// 執行SQL【只适用于支持sql的数据库】
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public override bool ExecuteSql(string sqlstr)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            sqlstr = sqlstr.Replace("\r\n", " "); //将换行符替换为空格
                            conn.Execute(sqlstr, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>ExecuteSql异常:{ex.Message}>>SQL:{sqlstr}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>ExecuteSql连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        /// <summary>
        /// 查询SQL【只适用于支持sql的数据库】
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        public override List<Dictionary<string, object>> QuerySql(string sqlstr)
        {
            var dicts = new List<Dictionary<string, object>>();
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    try
                    {
                        sqlstr = sqlstr.Replace("\r\n", " "); //将换行符替换为空格
                        var lst = conn.Query(sqlstr)?.ToList();
                        dicts = DynLstToDictLst(lst);
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"{Config?.ToJson()}>>QuerySql异常:{ex.Message}>>SQL:{sqlstr}");
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>QuerySql连接异常:{ex.Message}。");
                }
            }
            return dicts;
        }
        public override bool Insert<T>(T model)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"insert into {GetTableName<T>()} {GetInsertStr(GetFields<T>())}";
                            conn.Execute(sql, model, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Insert<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Insert<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Insert(string tname, IDictionary<string, object> data)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"insert into {tname} {GetInsertStr(data?.Keys.AsList())}";
                            conn.Execute(sql, DictionaryToExpandoObject(data), transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Insert异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Insert连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchInsert<T>(List<T> models)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"insert into {GetTableName<T>()} {GetInsertStr(GetFields<T>())}";
                            conn.Execute(sql, models, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchInsert<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchInsert<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchInsert(string tname, List<IDictionary<string, object>> datas)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"insert into {tname} {GetInsertStr(datas?.FirstOrDefault().Keys.AsList())}";
                            conn.Execute(sql, DictionaryToExpandoObject(datas), transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchInsert异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchInsert连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Delete<T>(T model)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"delete from {GetTableName<T>()} where {GetWhereStr(GetPrimaryKeyNames<T>())}";
                            conn.Execute(sql, model, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Delete<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Delete<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Delete(string tname, List<string> keys, IDictionary<string, object> data)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"delete from {tname} where {GetWhereStr(keys)}";
                            conn.Execute(sql, DictionaryToExpandoObject(data), transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Delete异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Delete连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchDelete<T>(List<T> models)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"delete from {GetTableName<T>()} where {GetWhereStr(GetPrimaryKeyNames<T>())}";
                            conn.Execute(sql, models, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchDelete<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchDelete<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchDelete(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"delete from {tname} where {GetWhereStr(keys)}";
                            conn.Execute(sql, DictionaryToExpandoObject(datas), transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchDelete异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchDelete连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool DeleteByCondition<T>(Func<T, bool> filter)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            var query = $"select * from {GetTableName<T>()}";
                            var models = conn.Query<T>(query).Where(filter).ToList();//要删除的数据
                            sql = $"delete from {GetTableName<T>()} where {GetWhereStr(GetPrimaryKeyNames<T>())}";
                            conn.Execute(sql, models, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>DeleteByCondition<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>DeleteByCondition<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool DeleteByCondition(string tname, params ConditionModel[] conditions)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            string where = ConditionConversion(conditions);
                            where = string.IsNullOrEmpty(where) ? "" : $"and {where}";
                            sql = $"delete from {tname} where 1=1 {where}";
                            conn.Execute(sql, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>DeleteByCondition异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>DeleteByCondition连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Clear<T>()
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"truncate table {GetTableName<T>()}";
                            conn.Execute(sql, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Clear<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Clear<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Clear(string tname)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = $"truncate table {tname}";
                            conn.Execute(sql, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Clear异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Clear连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Update<T>(T model)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            var keys = GetPrimaryKeyNames<T>();
                            string updateStr = GetUpdateStr(keys, GetFields<T>());
                            string whereStr = GetWhereStr(keys);
                            sql = $"update {GetTableName<T>()} set {updateStr} where {whereStr}";
                            conn.Execute(sql, model, transaction);//执行批量操作
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Update<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Update<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Update(string tname, List<string> keys, IDictionary<string, object> data)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            string updateStr = GetUpdateStr(keys, data?.Keys.ToList());
                            string whereStr = GetWhereStr(keys);
                            sql = $"update {tname} set {updateStr} where {whereStr}";
                            conn.Execute(sql, DictionaryToExpandoObject(data), transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Update异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Update连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchUpdate<T>(List<T> models)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            var keys = GetPrimaryKeyNames<T>();
                            string updateStr = GetUpdateStr(keys, GetFields<T>());
                            string whereStr = GetWhereStr(keys);
                            sql = $"update {GetTableName<T>()} set {updateStr} where {whereStr}";
                            conn.Execute(sql, models, transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchUpdate<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchUpdate<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchUpdate(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            string updateStr = GetUpdateStr(keys, datas?.FirstOrDefault().Keys.ToList());
                            string whereStr = GetWhereStr(keys);
                            sql = $"update {tname} set {updateStr} where {whereStr}";
                            conn.Execute(sql, DictionaryToExpandoObject(datas), transaction);
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchUpdate异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchUpdate连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Cover<T>(T model)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = GetCoverSql(GetTableName<T>(), GetPrimaryKeyNames<T>(), GetFields<T>());
                            conn.Execute(sql, model, transaction);//执行批量操作
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Cover<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Cover<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool Cover(string tname, List<string> keys, IDictionary<string, object> data)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = GetCoverSql(tname, keys, data?.Keys.ToList());
                            conn.Execute(sql, DictionaryToExpandoObject(data), transaction);//执行批量操作
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>Cover异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>Cover连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchCover<T>(List<T> models)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = GetCoverSql(GetTableName<T>(), GetPrimaryKeyNames<T>(), GetFields<T>());
                            conn.Execute(sql, models, transaction);//执行批量操作
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchCover<T>异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchCover<T>连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override bool BatchCover(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        string sql = string.Empty;
                        try
                        {
                            sql = GetCoverSql(tname, keys, datas.FirstOrDefault()?.Keys.ToList());
                            conn.Execute(sql, DictionaryToExpandoObject(datas), transaction);//执行批量操作
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            NLogger.Warn($"{Config?.ToJson()}>>BatchCover异常:{ex.Message}>>SQL:{sql}");
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>BatchCover连接异常:{ex.Message}。");
                    return false;
                }
            }
        }
        public override T QueryFirst<T>(params object[] values)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = string.Empty;
                    try
                    {
                        var keys = GetPrimaryKeyNames<T>();
                        string where = " where 1=1";
                        for (int i = 0; i < keys.Count; i++)
                        {
                            bool isString = (bool)values[i]?.GetType().Equals(typeof(string));
                            where += isString ? $" and {keys[i]}='{values[i]}'" : $" and {keys[i]}={values[i]}";
                        }
                        sql = $"select * from {GetTableName<T>()} {where}";
                        return conn.Query<T>(sql).ToList().FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"{Config?.ToJson()}>>QueryFirst<T>异常:{ex.Message}>>SQL:{sql}");
                        return default;
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>QueryFirst连接异常:{ex.Message}。");
                    return default;
                }
            }
        }
        public override List<T> QueryByCondition<T>(Func<T, bool> filter)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = string.Empty;
                    try
                    {
                        sql = $"select * from {GetTableName<T>()}";
                        if (filter == null)
                        {
                            return conn.Query<T>(sql).ToList();
                        }
                        else
                        {
                            return conn.Query<T>(sql).Where(filter).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"{Config?.ToJson()}>>QueryByCondition<T>异常:{ex.Message}>>SQL:{sql}");
                        return default;
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>QueryByCondition<T>连接异常:{ex.Message}。");
                    return default;
                }
            }
        }
        public override List<Dictionary<string, object>> QueryByCondition(string table, params ConditionModel[] conditions)
        {
            using (IDbConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string sql = string.Empty;
                    try
                    {
                        string where = ConditionConversion(conditions);
                        where = string.IsNullOrEmpty(where) ? "" : $"and {where}";
                        sql = $"select * from {table} where 1=1 {where}";
                        var lst = conn.Query(sql)?.ToList();
                        return DynLstToDictLst(lst);
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"{Config?.ToJson()}>>QueryByCondition异常:{ex.Message}>>SQL:{sql}");
                        return default;
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"{Config?.ToJson()}>>QueryByCondition连接异常:{ex.Message}。");
                    return default;
                }
            }
        }

        /// <summary>
        /// 获取参数符号
        /// </summary>
        /// <returns></returns>
        private string GetParameterSymbols()
        {
            string symbols = string.Empty;
            switch (Config.DbType)
            {
                case "SQLITE": symbols = "@"; break; //sqlite数据库
                case "ORACLE": symbols = ":"; break; //orcal数据库
                case "MYSQL": symbols = "@"; break; //mysql数据库
                case "MSSQL": symbols = "@"; break; //sqlserver数据库("@"或":"都可以)
                case "GBASE": symbols = "?"; break;  //GBase数据库
                case "DAMENG": symbols = ":"; break;  //Dameng数据库
                default: break;
            }
            return symbols;
        }
        /// <summary>
        /// 获取条件字符串
        /// 【样例："Name = @Name AND Age = @Age";】
        /// </summary>
        /// <param name="keys">主键集</param>
        /// <returns></returns>
        private string GetWhereStr(List<string> keys)
        {
            if (keys == null || keys.Count <= 0) return "1=1";
            string symbols = GetParameterSymbols();//参数符号
            string whereStr = string.Empty;
            keys?.ForEach(p =>
            {
                if ("GBASE".Equals(Config.DbType)) //Gbase特殊些，只用"?"来占位的
                {
                    whereStr += $"{p}={symbols} and ";
                }
                else
                {
                    whereStr += $"{p}={symbols}{p} and ";
                }
            });
            return whereStr.TrimEnd("and");
        }
        /// <summary>
        /// 获取插入字符串
        /// 【样例："(ID, TNAME) VALUES (@ID, @TNAME)";】
        /// 备注：还可以用Tuple<string, string>进行拆分，本方案直接拼接好语句了
        /// </summary>
        /// <param name="fields">字段集</param>
        /// <returns></returns>
        private string GetInsertStr(List<string> fields)
        {
            string symbols = GetParameterSymbols();//参数符号
            string str1 = string.Empty;
            string str2 = string.Empty;
            fields?.ForEach(p =>
            {
                str1 += $"{p},";
                if ("GBASE".Equals(Config.DbType)) //Gbase特殊些，只用"?"来占位的
                {
                    str2 += $"{symbols},";
                }
                else
                {
                    str2 += $"{symbols}{p},";
                }
            });
            return $"({str1.TrimEnd(",")}) VALUES ({str2.TrimEnd(",")})";
        }
        /// <summary>
        /// 获取修改字符串
        /// 【样例："Name = @Name,Age = @Age";】
        /// </summary>
        /// <param name="keys">主键集</param>
        /// <param name="fields">字段集</param>
        /// <returns></returns>
        private string GetUpdateStr(List<string> keys, List<string> fields)
        {
            if (keys == null || keys.Count <= 0) return string.Empty;
            if (fields == null || fields.Count <= 0) return string.Empty;
            string symbols = GetParameterSymbols();//参数符号
            string updateStr = string.Empty;//修改字符串
            fields?.ForEach(p =>
            {
                if (!keys.Contains(p))
                {
                    if ("GBASE".Equals(Config.DbType)) //Gbase特殊些，只用"?"来占位的
                    {
                        updateStr += $"{p}={symbols},";
                    }
                    else
                    {
                        updateStr += $"{p}={symbols}{p},";
                    }
                }
            });
            return updateStr.TrimEnd(",");
        }
        /// <summary>
        /// SQL条件转换
        /// </summary>
        /// <param name="conditions">条件字典</param>
        /// <returns></returns>
        private string ConditionConversion(params ConditionModel[] conditions)
        {
            string where = string.Empty;
            try
            {
                foreach (var item in conditions)
                {
                    string _filed = item.FieldName;//字段名
                    switch (item.ConditionType)
                    {
                        case ConditionEnum.Equal: //相等
                            {
                                var _value = item.ConditionValues?.FirstOrDefault();//第一项
                                bool isString = _value.GetType().Equals(typeof(string));
                                where += isString ? $" {_filed}='{_value}' and " : $" {_filed}={_value} and ";
                            }
                            break;
                        case ConditionEnum.Greater: //大于等于
                            {
                                var _value = item.ConditionValues?.FirstOrDefault();//第一项
                                bool isString = _value.GetType().Equals(typeof(string));
                                where += isString ? $" {_filed}>='{_value}' and " : $" {_filed}>={_value} and ";
                            }
                            break;
                        case ConditionEnum.Less: //小于等于
                            {
                                var _value = item.ConditionValues?.FirstOrDefault();//第一项
                                bool isString = _value.GetType().Equals(typeof(string));
                                where += isString ? $" {_filed}<='{_value}' and " : $" {_filed}<={_value} and ";
                            }
                            break;
                        case ConditionEnum.Between: //区间(全封闭)
                            {
                                var _value1 = item.ConditionValues?.Skip(0).Take(1);//第一项
                                bool isString1 = _value1.GetType().Equals(typeof(string));
                                where += isString1 ? $" {_filed}>='{_value1}' and " : $" {_filed}>={_value1} and ";
                                var _value2 = item.ConditionValues?.Skip(1).Take(1);//第二项
                                bool isString2 = _value2.GetType().Equals(typeof(string));
                                where += isString2 ? $" {_filed}<='{_value2}' and " : $" {_filed}<={_value2} and ";
                            }
                            break;
                        case ConditionEnum.In: //包含
                            {
                                string inSql = string.Empty;
                                var inLsts = Limit(item.ConditionValues, 999); //分页，避免出现in中大于1000的情况
                                foreach (var inLst in inLsts)
                                {
                                    string inStr = string.Empty;
                                    foreach (var initem in inLst)
                                    {
                                        bool isString = initem.GetType().Equals(typeof(string));
                                        inStr += isString ? $"'{initem}'," : $"{initem},"; ;
                                    }
                                    inSql += $"{_filed} in ({inStr.TrimEnd(",")}) or ";
                                }
                                where += $"({inSql.TrimEnd("or")}) and ";
                            }
                            break;
                        case ConditionEnum.Like: //全匹配(用于字符串)
                            {
                                where += $" {_filed} like '{item.ConditionValues.FirstOrDefault()}' and ";
                            }
                            break;
                        case ConditionEnum.LikeLeft:  //左匹配(用于字符串)
                            {
                                where += $" {_filed} like '%{item.ConditionValues.FirstOrDefault()}' and ";
                            }
                            break;
                        case ConditionEnum.LikeRight:  //右匹配(用于字符串)
                            {
                                where += $" {_filed} like '{item.ConditionValues.FirstOrDefault()}%' and ";
                            }
                            break;
                        case ConditionEnum.LikeMiddle:  //中间匹配(用于字符串)
                            {
                                where += $" {_filed} like '%{item.ConditionValues.FirstOrDefault()}%' and ";
                            }
                            break;
                    }
                }
                where = $" {where.TrimEnd("and")} ";
            }
            catch (Exception ex)
            {
                NLogger.Warn($"AbsSqlOperate>>ConditionConversion异常:{ex.Message}");
            }
            return where;
        }

        /// <summary>
        /// 定制化：List<dynamic>转List<Dictionary<string,object>>
        /// 支持中文转码和图片处理
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        /// <returns></returns>
        protected List<Dictionary<string, object>> DynLstToDictLst(List<dynamic> datas)
        {
            var rLst = new List<Dictionary<string, object>>();
            try
            {
                if (datas.Count <= 0) return rLst;
                foreach (var data in datas)
                {
                    var idic = data as IDictionary<string, object>;
                    var dic = new Dictionary<string, object>();
                    foreach (var item in idic)
                    {
                        if (typeof(byte[]).Equals(item.Value?.GetType()))//字节数组类型
                        {
                            if (item.Key.StartsWith("TP_"))//图片：图片不能转
                            {
                                dic.Add(item.Key, Convert.ToBase64String(item.Value as byte[]));
                            }
                            else //除了图片之外，字节数组大部分情况下都是中文转码
                            {
                                Encoding gbk = Encoding.GetEncoding("GB2312");
                                string temstr = gbk.GetString(item.Value as byte[]);
                                dic.Add(item.Key, temstr);
                            }
                        }
                        else if (typeof(string).Equals(item.Value?.GetType()))//字符串需要去除空格
                        {
                            dic.Add(item.Key, ((string)item.Value).Trim());
                        }
                        else if (!typeof(DBNull).Equals(item.Value?.GetType()))//非空类型,到了该逻辑多半是dynamic类型
                        {
                            #region 处理dynamic时将int类型也转成double的问题
                            if (item.Value == null)
                            {
                                dic.Add(item.Key, item.Value);//原始方案
                            }
                            else if (!$"{item.Value}".Contains('.'))
                            {
                                dic.Add(item.Key, Convert.ToInt32(item.Value));//转成int类型，避免被转成double
                            }
                            else
                            {
                                dic.Add(item.Key, item.Value);//原始方案
                            }
                            #endregion
                        }
                        else
                        {
                            dic.Add(item.Key, item.Value);//原始方案
                        }
                    }
                    rLst.Add(dic);
                }
            }
            catch (Exception ex)
            {
                NLogger.Warn($"DynLstToDictLst异常:{ex.Message}");
            }
            return rLst;
        }

        /// <summary>
        /// 根据指定条分页
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="count">每页条数，默认900</param>
        /// <returns></returns>
        private List<List<object>> Limit(List<object> lst, int count = 999)
        {
            var rlst = new List<List<object>>();
            int num = lst.Count % count == 0 ? lst.Count / count : lst.Count / count + 1;//页数
            for (int i = 0; i < num; i++)
            {
                try
                {
                    int tnum = (lst.Count - ((i + 1) * count)) >= 0 ? count : lst.Count - (i * count);//条数
                    List<object> tlst = lst.Skip(i * count).Take(tnum).ToList();
                    rlst.Add(tlst);
                }
                catch (Exception ex)
                {
                    NLogger.Error($"数据分页异常>>{ex.Message}");
                }
            }
            return rlst;
        }
    }
}
