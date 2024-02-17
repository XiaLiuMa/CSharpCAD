using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Max.BaseKit.Customs
{
    /// <summary>
    /// 自定义对象池
    /// </summary>
    public class CustomObjectPool<T>
    {
        /// <summary>
        /// 定时器
        /// </summary>
        private Timer timer;
        /// <summary>
        /// 对象创建函数
        /// </summary>
        private Func<T> createFunc;
        /// <summary>
        /// 对象池
        /// </summary>
        private ConcurrentQueue<T> pool;
        /// <summary>
        /// 对象池最大容量
        /// </summary>
        private int maxCapacity;
        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="createFunc">对象创建函数</param>
        /// <param name="maxCapacity">对象池最大容量</param>
        /// <param name="clearTime">定时清除空闲对象的间隔时间(单位：秒)，默认为0--不定时清清除</param>
        public CustomObjectPool(Func<T> createFunc, int maxCapacity, int clearTime = 0)
        {
            this.createFunc = createFunc;
            this.maxCapacity = maxCapacity;
            this.pool = new ConcurrentQueue<T>();
            if (clearTime > 0) //定时清除空闲对象
            {
                timer = new Timer(TimedClear, null, 0, clearTime * 1000);
            }
        }

        /// <summary>
        /// 定时清除空闲对象
        /// </summary>
        private void TimedClear(object state)
        {
            pool?.Clear();
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="timeout">超时时长(单位：100毫秒)，默认为0无超时时长</param>
        /// <returns></returns>
        public T GetObject(int timeout = 0)
        {
            if (timeout <= 0) return GetObjectRealize();
            for (int i = 0; i < timeout; i++)
            {
                var t = GetObjectRealize();
                if (t != null) return t;
                Thread.Sleep(100);
            }
            NLogger.Warn($"获取{typeof(T)}对象失败或超时。");
            return default(T);
        }

        /// <summary>
        /// 获取对象实现
        /// </summary>
        /// <returns></returns>
        private T GetObjectRealize()
        {
            if (pool.Count > 0)
            {
                pool.TryDequeue(out T obj);
                return obj;
            }
            else if (pool.Count < maxCapacity)
            {
                return createFunc();// 如果对象池未满，则创建一个新对象并返回
            }
            else //对象池已满，不允许再创建
            {
                return default(T);
            }
        }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="obj"></param>
        public void ReturnObject(T obj)
        {
            pool.Enqueue(obj);// 将对象返回到对象池中
        }

        #region lock用法，如果ConcurrentQueue无法保证控制池容量，便改用这种Queue+lock的用法
        ///// <summary>
        ///// 对象创建函数
        ///// </summary>
        //private Func<T> createFunc;
        ///// <summary>
        ///// 对象池
        ///// </summary>
        //private Queue<T> pool;
        ///// <summary>
        ///// 最大容量
        ///// </summary>
        //private int maxCapacity;
        //public CustomObjectPool(Func<T> createFunc, int maxCapacity)
        //{
        //    this.createFunc = createFunc;
        //    this.pool = new ConcurrentQueue<T>();
        //    this.maxCapacity = maxCapacity;
        //}
        //public T GetObject()
        //{
        //    lock (pool)
        //    {
        //        if (pool.Count > 0)
        //        {
        //            // 从对象池中随机抽取一个空闲对象并返回
        //            int index = new Random().Next(pool.Count);
        //            pool.TryDequeue(out T obj);
        //            for (int i = 0; i < index; i++)
        //            {
        //                T temp = pool.Dequeue();
        //                pool.Enqueue(temp);
        //            }
        //            return obj;
        //        }
        //        else if (pool.Count < maxCapacity)
        //        {
        //            // 如果对象池未满，则创建一个新对象并返回
        //            return createFunc();
        //        }
        //        else
        //        {
        //            // 如果对象池已满，则抛出异常
        //            throw new Exception("Object pool is full.");
        //        }
        //    }
        //}
        //public void ReturnObject(T obj)
        //{
        //    lock (pool)
        //    {
        //        // 将对象返回到对象池中
        //        pool.Enqueue(obj);
        //    }
        //}
        #endregion
    }
}
