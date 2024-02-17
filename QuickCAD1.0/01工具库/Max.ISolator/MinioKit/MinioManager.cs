using Max.BaseKit;
using Max.BaseKit.Customs;
using Max.BaseKit.Utils;
using Max.ISolator.FtpKit;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Max.ISolator.MinioKit
{
    /// <summary>
    /// Minio客户端管理
    /// </summary>
    public class MinioManager
    {
        #region 单例
        private static MinioManager i;
        private readonly static object objLock = new object();
        public static MinioManager I
        {
            get
            {
                if (i == null)
                {
                    lock (objLock)
                    {
                        if (i == null)
                        {
                            i = new MinioManager();
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
        public ConcurrentDictionary<MimioClientMod, CustomObjectPool<MaxMinioClient>> Dic { get; private set; }

        public MinioManager()
        {
            Dic = new ConcurrentDictionary<MimioClientMod, CustomObjectPool<MaxMinioClient>>();
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public MaxMinioClient GetObject(MimioClientMod mod)
        {
            Dic.TryGetValue(mod, out var pool);
            if (pool == null)
            {
                pool = new CustomObjectPool<MaxMinioClient>(() => { return new MaxMinioClient(mod); }, mod.Concurrency, 3 * 60);
                Dic.TryAdd(mod, pool);
            }
            return pool?.GetObject();
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="obj"></param>
        public void ReturnObject(MimioClientMod mod, MaxMinioClient obj)
        {
            Dic.TryGetValue(mod, out var pool);
            if (pool == null) return;
            pool?.ReturnObject(obj);
        }
    }
}
