using System;
using System.Collections.Generic;
using System.Text;

namespace Max.BaseKit.Utils
{
    /// <summary>
    /// 列表帮助类
    /// </summary>
    public class ListUtil
    {
        /// <summary>
        /// 获取对象在列表的索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static int GetIndex<T>(List<T> lst, T t)
        {
            if (lst != null && lst.Count > 0)
            {
                for (int i = 0; i < lst.Count; i++)
                {
                    if (t.Equals(lst[i])) return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 修改列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="t"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<T> Alert<T>(List<T> lst, T t, int index)
        {
            if (lst == null || lst.Count <= 0) return null;
            if (index < 0 || index > lst.Count - 1) return null;
            lst[index] = t;
            return lst;
        }
    }
}
