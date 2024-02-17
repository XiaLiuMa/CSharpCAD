using Max.BaseKit;
using Max.BaseKit.Exts;
using Max.DbTool.Imp.NoSQL;
using Max.DbTool.Imp.SQL;
using Max.BaseKit.Customs;
using System.Collections.Concurrent;

namespace Max.DbTool
{
    /// <summary>
    /// 数据库操作管理
    /// </summary>
    public class DbOperateManger
    {
        #region 单例
        private static DbOperateManger i;
        private readonly static object objLock = new object();
        public static DbOperateManger I
        {
            get
            {
                if (i == null)
                {
                    lock (objLock)
                    {
                        if (i == null)
                        {
                            i = new DbOperateManger();
                        }
                    }
                }
                return i;
            }
        }
        #endregion

        /// <summary>
        /// 对象池字典
        /// </summary>
        public ConcurrentDictionary<DbConfig, CustomObjectPool<IDbOperate>> Dic { get; private set; }

        public DbOperateManger()
        {
            Dic = new ConcurrentDictionary<DbConfig, CustomObjectPool<IDbOperate>>();
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public IDbOperate GetObject(DbConfig mod)
        {
            Dic.TryGetValue(mod, out var pool);
            if (pool == null)
            {
                pool = new CustomObjectPool<IDbOperate>(() => { return Creat(mod); }, mod.Concurrency, 3 * 60);
                Dic.TryAdd(mod, pool);
            }
            return pool?.GetObject(10 * 60 * 5);//超时时长5分钟
        }

        /// <summary>
        /// 创建一个数据库操作对象
        /// </summary>
        /// <param name="mod">数据库配置</param>
        /// <returns></returns>
        private IDbOperate Creat(DbConfig mod)
        {
            IDbOperate operate = default;
            switch (mod.DbType)
            {
                case "SQLITE": operate = new SqliteOperate(mod); break; //sqlite数据库
                case "ORACLE": operate = new OracleOperate(mod); break; //orcal数据库
                case "MYSQL": operate = new MysqlOperate(mod); break; //mysql数据库
                case "MSSQL": operate = new MysqlOperate(mod); break; //sqlserver数据库
                case "MONGO": operate = new MongoOperate(mod); break;  //mongo数据库
                case "GBASE": operate = new GbaseOperate(mod); break;  //GBase数据库
                case "DAMENG": operate = new DamengOperate(mod); break;  //Dameng数据库
                default: break;
            }
            if (operate == null) NLogger.Warn($"创建{mod?.ToJson()}数据库操作对象失败。");
            return operate;
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="obj"></param>
        public void ReturnObject(DbConfig mod, IDbOperate obj)
        {
            Dic.TryGetValue(mod, out var pool);
            if (pool == null) return;
            pool?.ReturnObject(obj);
        }
    }
}
