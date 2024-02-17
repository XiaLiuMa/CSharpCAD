﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Max.BaseKit.Utils
{
    /// <summary>
    /// 随机值工具
    /// </summary>
    public static class RandomUtil
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// 获取一个随机数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Next(int max)
        {
            lock (_random)
            {
                return _random.Next(max);
            }
        }

        /// <summary>
        /// 从区间数中随机获取1个
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Next(int min, int max)
        {
            lock (_random)
            {
                return _random.Next(min, max);
            }
        }

        /// <summary>
        /// 按概率获取
        /// </summary>
        /// <param name="trueProp"></param>
        /// <returns></returns>
        public static bool PickBoolByProp(double trueProp = 1)
        {
            if (trueProp > 1)
            {
                trueProp = 1;
            }
            if (trueProp < 0)
            {
                trueProp = 0;
            }
            Dictionary<bool, double> wt = new Dictionary<bool, double>
            {
               { true , trueProp },
               { false , 1 - trueProp }
            };
            return wt.PickOneByProb();
        }

        /// <summary>
        /// 按指定概率获取随机结果
        /// </summary>
        /// <param name="sourceDic">a 0.8 b 0.1 c 0.1</param>
        /// <returns>随机结果 [a,b,c]</returns>
        public static T PickOneByProb<T>(this Dictionary<T, double> sourceDic)
        {
            if (sourceDic == null || !sourceDic.Any())
            {
                return default(T);
            }

            int seed = (int)(10 / (sourceDic.Values.Where(c => c > 0).Min()));
            int maxValue = sourceDic.Values.Aggregate(0, (current, d) => current + (int)(seed * d));

            int rNum = Next(maxValue);
            int tem = 0;
            foreach (KeyValuePair<T, double> item in sourceDic)
            {
                tem += (int)(item.Value * seed);
                if (tem > rNum)
                {
                    return item.Key;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 随机从List中获取一项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T PickOne<T>(this List<T> source)
        {
            if (source == null || !source.Any())
            {
                return default(T);
            }
            return source[Next(source.Count)];
        }

        /// <summary>
        /// 随机从List中获取多项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static List<T> PickAny<T>(this List<T> source, int c)
        {
            if (source == null || !source.Any())
            {
                return default(List<T>);
            }
            if (source.Count <= c)
            {
                return source;
            }
            List<T> ls = new List<T>();
            for (int i = 0; i < c; i++)
            {
                var t = source.PickOne();
                if (!ls.Contains(t))
                {
                    ls.Add(t);
                }
            }
            return ls;
        }

        /// <summary>
        /// 从区间数中随机获取1个
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static DateTime RandomTime(DateTime min, DateTime max)
        {
            lock (_random)
            {
                TimeSpan timeSpan = max - min;
                TimeSpan randomTimeSpan = new TimeSpan(0, 0, _random.Next(0, (int)timeSpan.TotalSeconds));
                DateTime randomDate = min + randomTimeSpan;
                return randomDate;
            }
        }
    }
}
