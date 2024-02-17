using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Max.BaseKit.Exts
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// 字符包含判断(全包含、半包含)
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <param name="AllInclusive">false：半包含，true：全包含</param>
        /// <param name="args">参与判断的字符</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 对象转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Convert<T>(this object obj)
        {
            T t = default(T);
            try
            {
                t = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
            }
            catch (Exception ex)
            {
                NLogger.Warn(ex.Message);
            }
            return t ;
        }

        /// <summary>
        /// Dictionary<string, object>转换IDictionary<string, object>
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IDictionary<string, object> ConvertToIDictionary(this Dictionary<string, object> dic)
        {
            IDictionary<string, object> rdic = dic;
            return rdic;
        }

        /// <summary>
        /// List<Dictionary<string, object>>转换List<IDictionary<string, object>>
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<IDictionary<string, object>> ConvertToIDictionary(this List<Dictionary<string, object>> lst)
        {
            return lst?.Cast<IDictionary<string, object>>().ToList();
        }

        /// <summary>
        /// IDictionary<string, object>转换Dictionary<string, object>
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ConvertToDictionary(this IDictionary<string, object> dic)
        {
            Dictionary<string, object> rdic = dic.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return rdic;
        }

        /// <summary>
        /// List<IDictionary<string, object>>转换List<Dictionary<string, object>>
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ConvertToDictionary(this List<IDictionary<string, object>> lst)
        {
            return lst?.Select(d => (Dictionary<string, object>)d).ToList();
        }
    }
}
