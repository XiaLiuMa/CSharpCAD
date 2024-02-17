using System;
using System.Data;
using Max.BaseKit;
using System.Data.SqlClient;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

namespace Max.DbTool.Imp.SQL
{
    /// <summary>
    /// 提供SqlServer数据库操作实现
    /// SqlConnection、SqlCommand、SqlDataAdapter、SqlTransaction
    /// </summary>
    public class MssqlOperate : AbsSqlOperate
    {
        public override DbConfig Config { get; set; }
        public MssqlOperate(DbConfig config) { Config = config; }
        protected override IDbConnection GetConnection()
        {
            return new OracleConnection(Config.ConnStr);
        }
        protected override string GetCoverSql(string tname, List<string> keys, List<string> fields)
        {
            return default;
        }

        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
    }
}
