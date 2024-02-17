using System;
using Max.BaseKit.Customs;
using System.Collections.Concurrent;

namespace Max.ISolator.RabbitMqKit
{
    /// <summary>
    /// RabbitMQ工厂(当前只集成Topic模式)
    /// </summary>
    public class RmqManager
    {
        #region 单例
        private static RmqManager i;
        private readonly static object objLock = new object();
        public static RmqManager I
        {
            get
            {
                if (i == null)
                {
                    lock (objLock)
                    {
                        if (i == null)
                        {
                            i = new RmqManager();
                        }
                    }
                }
                return i;
            }
        }
        #endregion

        /// <summary>
        /// 生产者对象池字典
        /// </summary>
        public ConcurrentDictionary<RmqProducerMod, CustomObjectPool<RmqProducer>> ProducerDic { get; private set; }
        /// <summary>
        /// 消费者对象字典
        /// </summary>
        public ConcurrentDictionary<RmqConsumerMod, RmqConsumer> ConsumerDic { get; private set; }

        public RmqManager()
        {
            ProducerDic = new ConcurrentDictionary<RmqProducerMod, CustomObjectPool<RmqProducer>>();
            ConsumerDic = new ConcurrentDictionary<RmqConsumerMod, RmqConsumer>();
        }

        /// <summary>
        /// 获取生产者对象
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public RmqProducer GetProducer(RmqProducerMod mod)
        {
            ProducerDic.TryGetValue(mod, out var pool);
            if (pool == null)
            {
                pool = new CustomObjectPool<RmqProducer>(() => { return new RmqProducer(mod); }, mod.Concurrency, 3 * 60);
                ProducerDic.TryAdd(mod, pool);
            }
            return pool?.GetObject();
        }

        /// <summary>
        /// 归还生产者对象
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="obj"></param>
        public void ReturnProducer(RmqProducerMod mod, RmqProducer obj)
        {
            ProducerDic.TryGetValue(mod, out var pool);
            if (pool == null) return;
            pool?.ReturnObject(obj);
        }

        /// <summary>
        /// 生产者发布(已处理好归还对象)
        /// </summary>
        /// <param name="mod">生产者配置</param>
        /// <param name="data">生产的数据</param>
        public void Publich(RmqProducerMod mod, byte[] data)
        {
            var producer = GetProducer(mod);
            producer?.Publich(data);
            ReturnProducer(mod, producer);
        }

        /// <summary>
        /// 消费者注册
        /// </summary>
        /// <param name="mod">消费者配置</param>
        /// <param name="callBack">消费者回调</param>
        public void Subscribe(RmqConsumerMod mod, Action<RmqConsumer, byte[]> callBack)
        {
            ConsumerDic.TryGetValue(mod, out var consumer);
            if (consumer == null)
            {
                consumer = new RmqConsumer(mod);
                ConsumerDic.TryAdd(mod, consumer);
            }
            consumer?.Subscribe(callBack);
        }
    }
}
