using System;
using System.Collections.Concurrent;

namespace Max.BaseKit.Customs
{
    /// <summary>
    /// 自定义ConcurrentQueue(线程安全集合，先入先出)
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class CustomConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public event EventHandler<ItemEventArgs> OnItemAdded;
        public event EventHandler<ItemEventArgs> OnItemRemoved;
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnItemAdded?.Invoke(this, new ItemEventArgs(item));
        }

        public new bool TryDequeue(out T result)
        {
            bool flag = base.TryDequeue(out result);
            OnItemRemoved?.Invoke(this, new ItemEventArgs(result));
            return flag;
        }
    }
}
