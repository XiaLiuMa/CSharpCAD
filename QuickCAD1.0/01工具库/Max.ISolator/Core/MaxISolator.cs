using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Max.ISolator.Core
{
    public class MaxIsolator
    {
        #region 单例
        private static MaxIsolator i;
        private readonly static object objLock = new object();
        public static MaxIsolator I
        {
            get
            {
                if (i == null)
                {
                    lock (objLock)
                    {
                        if (i == null)
                        {
                            i = new MaxIsolator();
                        }
                    }
                }
                return i;
            }
        }
        #endregion

        /// <summary>
        /// 监听器集合
        /// </summary>
        public List<IListener> IListener_Lst { get; private set; }
        /// <summary>
        /// 隔离器字典
        /// </summary>
        public ConcurrentDictionary<string, IIsolator> IIsolator_Dic { get; private set; }
        /// <summary>
        /// 隔离器回调函数
        /// </summary>
        public Action<int> OnCallback { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="programName">程序名称，用于反射是指定dll</param>
        /// <param name="callback">隔离器回调函数</param>
        public void Init(string programName, Action<int> callback)
        {
            this.OnCallback = callback;
            IListener_Lst = new List<IListener>();
            IIsolator_Dic = new ConcurrentDictionary<string, IIsolator>();

            #region 反射获取所有服务实现类
            Assembly assembly = Assembly.LoadFrom($"{programName}.dll");
            var classes = assembly.GetTypes().Where(t => typeof(IListener).IsAssignableFrom(t));
            foreach (var cl in classes)
            {
                var handler = Activator.CreateInstance(cl) as IListener;
                IListener_Lst.Add(handler);
            }
            #endregion
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void BootUp()
        {
            foreach (var item in IIsolator_Dic)
            {
                item.Value?.BootUp(); //启动隔离器
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void ShutDown()
        {
            foreach (var item in IIsolator_Dic)
            {
                item.Value?.ShutDown(); //启动隔离器
            }
        }
    }
}
