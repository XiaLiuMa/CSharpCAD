using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Max.BaseKit.Utils
{
    public static class JsonUtil
    {
        /// <summary>
        /// 对象转json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T StrToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// json字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ObjectToStr<T>(T t)
        {
            return JsonConvert.SerializeObject(t);
        }


        /// <summary>
        /// DataTable转对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static List<T> DataTableToObjs<T>(this DataTable dataTable)
        {
            string jsonString = JsonConvert.SerializeObject(dataTable, new DataTableConverter());
            return JsonConvert.DeserializeObject<List<T>>(jsonString);
        }

        /// <summary>
        /// 对象列表转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ObjsToDataTable<T>(this List<T> list)
        {
            string jsonString = JsonConvert.SerializeObject(list);
            return JsonConvert.DeserializeObject<DataTable>(jsonString, new DataTableConverter());
        }
        
        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <param name="obj">待转化的对象</param> 
        /// <returns></returns>
        public static Dictionary<string, object> ObjToDic(object obj)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性
            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理 
                    if (m.Invoke(obj, new object[] { }) != null)
                    {
                        map.Add(p.Name, m.Invoke(obj, new object[] { })); // 向字典添加元素
                    }
                }
            }
            return map;
        }

        /// <summary>
        /// 字典类型转化为对象
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T  DicToObj<T>(Dictionary<string, object> dic) where T : new()
        {
            var md = new T();
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            foreach (var d in dic)
            {
                var filed = textInfo.ToTitleCase(d.Key);
                try
                {
                    var value = d.Value;
                    md.GetType().GetProperty(filed).SetValue(md, value);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return md;
        }

        
    }
}
