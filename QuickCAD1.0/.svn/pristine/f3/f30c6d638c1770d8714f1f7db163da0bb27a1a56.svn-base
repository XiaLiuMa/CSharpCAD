using System;
using Max.BaseKit.Exts;
using System.Runtime.Caching;

namespace Max.BaseKit.Customs
{
    public class CacheHelper
    {
        private static readonly MemoryCache cache = MemoryCache.Default;

        /// <summary>
        /// 检查数据与缓存中的数据相比较是否发生变化
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckDataChanged(string key, object data)
        {
            var tobj = cache.Contains(key) ? cache.Get(key) : default;
            string json1 = data == null ? string.Empty : data.ToJson();
            string json2 = tobj == null ? string.Empty : tobj.ToJson();
            return !json1.Equals(json2);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetCacheData(string key)
        {
            return cache.Contains(key) ? cache.Get(key) : default;
        }

        /// <summary>
        /// 更新缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="removedCallback">移除回调函数</param>
        /// <param name="timeout">超时时长(单位：秒)，小于等于0表示用不过期</param>
        public static void UpdateCacheData(string key, object data, Action<string, object> removedCallback, double timeout = 0)
        {
            cache.Remove(key);

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = timeout <= 0 ? DateTimeOffset.MaxValue : DateTimeOffset.Now.AddSeconds(timeout),
                RemovedCallback = p => { removedCallback?.Invoke(key, p.CacheItem.Value); },
            };
            cache.Set(key, data, policy);
        }
    }
}
