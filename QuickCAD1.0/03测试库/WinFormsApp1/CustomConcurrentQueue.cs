using System;
using System.Collections.Concurrent;

namespace Max.BaseKit.Customs
{
    /// <summary>
    /// 自定义ConcurrentQueue
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class CustomConcurrentQueue<T> : ConcurrentQueue<T>
    {
        public event EventHandler<ItemAddedEventArgs> OnItemAdded;
        public event EventHandler<ItemAddedEventArgs> OnItemRemoved;
        public new void Enqueue(T item)
        {
            base.Enqueue(item);
            OnItemAdded?.Invoke(this, new ItemAddedEventArgs(item));
        }

        public new bool TryDequeue(out T result)
        {
            bool flag = base.TryDequeue(out result);
            OnItemRemoved?.Invoke(this, new ItemAddedEventArgs(result));
            return flag;
        }
    }

    public class ItemAddedEventArgs : EventArgs
    {
        public object Item { get; }

        public ItemAddedEventArgs(object item)
        {
            Item = item;
        }
    }
}
