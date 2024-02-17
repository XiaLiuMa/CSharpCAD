using Max.BaseKit.Customs;
using System.Collections.Concurrent;

namespace Max.ISolator.FtpKit
{
    public class FtpManager
    {
        #region 单例
        private static FtpManager i;
        private readonly static object objLock = new object();
        public static FtpManager I
        {
            get
            {
                if (i == null)
                {
                    lock (objLock)
                    {
                        if (i == null)
                        {
                            i = new FtpManager();
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
        public ConcurrentDictionary<FtpClientMod, CustomObjectPool<FtpClient>> Dic { get; private set; }

        public FtpManager()
        {
            Dic = new ConcurrentDictionary<FtpClientMod, CustomObjectPool<FtpClient>>();
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public FtpClient GetObject(FtpClientMod mod)
        {
            Dic.TryGetValue(mod, out var pool);
            if (pool == null)
            {
                pool = new CustomObjectPool<FtpClient>(() => { return new FtpClient(mod); }, mod.Concurrency, 3 * 60);
                Dic.TryAdd(mod, pool);
            }
            return pool?.GetObject();
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="obj"></param>
        public void ReturnObject(FtpClientMod mod, FtpClient obj)
        {
            Dic.TryGetValue(mod, out var pool);
            if (pool == null) return;
            pool?.ReturnObject(obj);
        }
    }
}
