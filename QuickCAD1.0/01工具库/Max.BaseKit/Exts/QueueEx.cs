using System.Collections;
using System.Collections.Generic;

namespace Max.BaseKit.Exts
{
    public class QueueEx<T> : Queue<T>
    {
        private int capacity { get; set; }
        public QueueEx(int capacity) : base(capacity)
        {
            this.capacity = capacity;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            if (this.Count > capacity)
            {
                base.Dequeue();
            }
        }
    }
}
