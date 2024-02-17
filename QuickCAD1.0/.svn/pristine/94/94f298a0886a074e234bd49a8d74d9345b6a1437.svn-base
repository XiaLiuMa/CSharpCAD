using System;
using Max.BaseKit.Customs;
using System.Collections.Concurrent;

namespace Max.ISolator.KafkaKit
{
    public class KafkaManager
    {
        #region 单例
        private static KafkaManager i;
        private readonly static object objLock = new object();
        public static KafkaManager I
        {
            get
            {
                if (i == null)
                {
                    lock (objLock)
                    {
                        if (i == null)
                        {
                            i = new KafkaManager();
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
        public ConcurrentDictionary<KafkaProducerMod, CustomObjectPool<KafkaProducer>> ProducerDic { get; private set; }
        /// <summary>
        /// 消费者对象字典
        /// </summary>
        public ConcurrentDictionary<KafkaConsumerMod, KafkaConsumer> ConsumerDic { get; private set; }

        public KafkaManager()
        {
            ProducerDic = new ConcurrentDictionary<KafkaProducerMod, CustomObjectPool<KafkaProducer>>();
            ConsumerDic = new ConcurrentDictionary<KafkaConsumerMod, KafkaConsumer>();
        }

        /// <summary>
        /// 获取生产者对象
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public KafkaProducer GetProducer(KafkaProducerMod mod)
        {
            ProducerDic.TryGetValue(mod, out var pool);
            if (pool == null)
            {
                pool = new CustomObjectPool<KafkaProducer>(() => { return new KafkaProducer(mod); }, mod.Concurrency, 3 * 60);
                ProducerDic.TryAdd(mod, pool);
            }
            return pool?.GetObject();
        }

        /// <summary>
        /// 归还生产者对象
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="obj"></param>
        public void ReturnProducer(KafkaProducerMod mod, KafkaProducer obj)
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
        public void Publich(KafkaProducerMod mod, string data)
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
        public void Subscribe(KafkaConsumerMod mod, Action<KafkaConsumer, string> callBack)
        {
            ConsumerDic.TryGetValue(mod, out var consumer);
            if (consumer == null)
            {
                consumer = new KafkaConsumer(mod);
                ConsumerDic.TryAdd(mod, consumer);
            }
            consumer?.Subscribe(callBack);
        }
    }
}
