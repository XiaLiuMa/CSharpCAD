using System.Timers;
using System.Collections.Concurrent;

namespace Max.BaseKit.Exts
{
    public delegate void MaxCacheElapsed(string key, object obj);
    public class MaxCache
    {
        private readonly ConcurrentDictionary<string, object> data_dic;//数据字典
        private readonly ConcurrentDictionary<string, Timer> timer_dic;//定时器字典
        /// <summary>
        /// 缓存自动移除回调
        /// </summary>
        public event MaxCacheElapsed CacheElapsed = (s, o) => { };
        public MaxCache()
        {
            data_dic = new ConcurrentDictionary<string, object>();
            timer_dic = new ConcurrentDictionary<string, Timer>();
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <param name="timeout"></param>
        public void SetCache(string key, object obj, double timeout)
        {
            MaxCacheTimer timer = new MaxCacheTimer(key, timeout);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
            timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件
            if (timer_dic.Keys.Contains(key))
            {
                Timer old_timer = default;
                timer_dic.TryGetValue(key, out old_timer);
                old_timer?.Dispose();//销毁老的定时器
                timer_dic.TryUpdate(key, timer, old_timer);
            }
            else
            {
                timer_dic.TryAdd(key, timer);
            }


            if (data_dic.ContainsKey(key))
            {
                object old_obj = default;
                data_dic.TryGetValue(key, out old_obj);
                data_dic.TryUpdate(key, obj, old_obj);
            }
            else
            {
                data_dic.TryAdd(key, obj);
            }
        }

        /// <summary>
        /// 定时器触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timer = sender as MaxCacheTimer;
            if (timer != null)
            {
                string key = timer.Key;

                if (data_dic.ContainsKey(key))
                {
                    object old_obj = default;
                    data_dic.TryRemove(key, out old_obj);
                    CacheElapsed(key, old_obj);
                }

                if (timer_dic.ContainsKey(key))
                {
                    Timer old_timer = default;
                    timer_dic.TryGetValue(key, out old_timer);
                    old_timer?.Dispose();//销毁老的定时器
                    timer_dic.TryRemove(key, out old_timer);
                }
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetCache(string key)
        {
            object obj = default;
            if (data_dic.ContainsKey(key))
            {
                data_dic.TryGetValue(key, out obj);
            }
            return obj;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void RemoveCache(string key)
        {
            if (data_dic.ContainsKey(key))
            {
                object old_obj = default;
                data_dic.TryRemove(key, out old_obj);
            }

            if (timer_dic.ContainsKey(key))
            {
                Timer old_timer = default;
                timer_dic.TryGetValue(key, out old_timer);
                old_timer?.Dispose();//销毁老的定时器
                timer_dic.TryRemove(key, out old_timer);
            }
        }
    }


    internal class MaxCacheTimer : Timer
    {
        /// <summary>
        /// 用缓存的key作为定时器的key
        /// </summary>
        public string Key { get; private set; }
        public MaxCacheTimer(string key, double interval)
        {
            this.Key = key;
            this.Interval = interval;
        }
    }
}
