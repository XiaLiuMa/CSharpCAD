using System;
using System.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Max.BaseKit.Customs
{
    /// <summary>
    /// 自定义ConcurrentList(无序线程安全集合)
    /// <typeparam name="T"></typeparam>
    public class CustomConcurrentList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        public event EventHandler<ItemEventArgs> OnItemAdded;
        public event EventHandler<ItemEventArgs> OnItemRemoved;

        private List<T> _list;

        // 摘要: 
        //     初始化 System.Collections.Generic.List<T> 类的新实例，该实例为空并且具有默认初始容量。
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public CustomConcurrentList()
        {
            _list = new List<T>();
        }

        public CustomConcurrentList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }
        //
        // 摘要: 
        //     初始化 System.Collections.Generic.List<T> 类的新实例，该实例为空并且具有指定的初始容量。
        //
        // 参数: 
        //   capacity:
        //     新列表最初可以存储的元素数。
        //
        // 异常: 
        //   System.ArgumentOutOfRangeException:
        //     capacity 小于 0。
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public CustomConcurrentList(int capacity)
        {
            _list = new List<T>(capacity);
        }


        public void CopyTo(T[] array, int index)
        {
            lock (this)
            {
                _list.CopyTo(array, index);
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public void Clear()
        {
            lock (this)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (this)
            {
                return _list.Contains(item);
            }
        }

        public int IndexOf(T item)
        {
            lock (this)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (this)
            {
                _list.Insert(index, item);
                OnItemAdded?.Invoke(this, new ItemEventArgs(item));
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (this)
            {
                return _list.GetEnumerator();
            }
        }

        public void ForEach(Action<T> action)
        {
            if (action == null) return;
            lock (this)
            {
                _list.ForEach(p => action?.Invoke(p));
            }
        }

        public void Add(T item)
        {
            lock (this)
            {
                _list.Add(item);
                OnItemAdded?.Invoke(this, new ItemEventArgs(item));
            }
        }

        public bool Remove(T item)
        {
            lock (this)
            {
                bool flag = _list.Remove(item);
                OnItemRemoved?.Invoke(this, new ItemEventArgs(item));
                return flag;
            }
        }

        public void RemoveAt(int index)
        {
            lock (this)
            {
                if (index >= 0 && _list.Count >= index)
                {
                    var item = _list[index];
                    _list.RemoveAt(index);
                    OnItemRemoved?.Invoke(this, new ItemEventArgs(item));
                }
            }
        }

        public T Find(Func<T, bool> func)
        {
            lock (this)
            {
                if (func == null) return default(T);
                var match = new Predicate<T>(func.Invoke);
                return _list.Find(match);
            }
        }

        public List<T> FindAll(Func<T, bool> func)
        {
            lock (this)
            {
                if (func == null) return default;
                var match = new Predicate<T>(func.Invoke);
                return _list.FindAll(match);
            }
        }

        public List<T> Where(Func<T, bool> func)
        {
            lock (this)
            {
                if (func == null) return default;
                return _list.Where(func)?.ToList();
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                lock (this)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (this)
                {
                    _list[index] = value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                return _list.GetEnumerator();
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }

}
