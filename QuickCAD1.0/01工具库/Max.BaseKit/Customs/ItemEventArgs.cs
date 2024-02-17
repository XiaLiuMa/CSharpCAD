using System;

namespace Max.BaseKit.Customs
{
    public class ItemEventArgs : EventArgs
    {
        public object Item { get; }

        public ItemEventArgs(object item)
        {
            Item = item;
        }
    }
}
