using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;


namespace Max.BaseKit.Utils
{
    
    /// <summary>
    /// 提供对一般Object的常用处理
    /// </summary>
    public static class ObjectUtil
    {
        /// <summary>
        /// 从二进制字节数组中反序列化对象
        /// </summary>
        public static object BinaryDeserialize(byte[] data)
        {
            object obj;
            using (var ms = new MemoryStream(data))
            {
                var f = new BinaryFormatter();
                obj = f.Deserialize(ms);
            }
            return obj;
        }
        /// <summary>
        /// 将一个对象序列化为二进制字节数组
        /// </summary>
        public static byte[] BinarySerialize(object obj)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                var f = new BinaryFormatter();
                f.Serialize(ms, obj);
                data = ms.ToArray();
            }
            return data;
        }
        /// <summary>
        /// 创建某个interface/class的实例. 要求子类必须有空参数构造器.
        /// </summary>
        public static object CreateObject(string typeShortName, Type parentType)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.Name.Equals(typeShortName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (parentType.IsAssignableFrom(t))
                        {
                            return Activator.CreateInstance(t, true);
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 在所有程序集中查找指定类型的所有子类或接口实现类
        /// </summary>
        public static List<Type> FindSubClasses(Type parentType)
        {
            var list = new List<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if (parentType.IsAssignableFrom(t))
                        {
                            list.Add(t);
                        }
                    }
                }
                catch (Exception e)
                {
                    NLogger.Error(e.StackTrace);
                }
            }

            return list;
        }
        /// <summary>
        /// 在当前AppDomain中查找类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type FindType(string name)
        {
            var type = Type.GetType(name);
            if (type != null)
            {
                return type;
            }

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(name, false);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取指定类型的所有字段，包括继承的字段，但不包括object一级的字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetTypeFields(Type type)
        {
            var t0 = typeof(object);
            var list = new List<FieldInfo>();
            while (type != t0)
            {
                list.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                type = type.BaseType;
            }
            return list;
        }
        /// <summary>
        /// 判断是否为数值类型
        /// </summary>
        /// <param name="destDataType"></param>
        /// <returns></returns>
        public static bool IsNumbericType(Type destDataType)
        {
            if ((destDataType == typeof(int)) || (destDataType == typeof(uint)) || (destDataType == typeof(double))
                || (destDataType == typeof(short)) || (destDataType == typeof(ushort)) || (destDataType == typeof(decimal))
                || (destDataType == typeof(long)) || (destDataType == typeof(ulong)) || (destDataType == typeof(float))
                || (destDataType == typeof(byte)) || (destDataType == typeof(sbyte)))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 判断一个类型是否为基本类型。这里的基本类型的含义是Primritive及String, Enum, Guid, DateTime, Decimal
        /// </summary>
        public static bool IsPrimritive(Type type)
        {
            if (type.IsPrimitive || type.IsEnum)
            {
                return true;
            }

            switch (type.ToString())
            {
                case "System.String":
                case "System.Decimal":
                case "System.DateTime":
                case "System.Guid":
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 判断给定的类型是否为字符串类型
        /// </summary>
        /// <returns></returns>
        public static bool IsString(Type type)
        {
            switch (type.ToString())
            {
                case "System.String":
                case "System.Char":
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 从字符串中解析基本类型的数据。支持的类型包括Primritive, Enum, String, Guid, DateTime, Decimal
        /// </summary>
        public static bool ParsePrimritive(Type type, string text, out object value)
        {
            value = null;

            if (type.IsEnum)
            {
                value = Enum.Parse(type, text, true);
                return true;
            }

            switch (type.ToString())
            {
                case "System.String":
                    value = text;
                    return true;

                case "System.Int32":
                    {
                        int i;
                        if (int.TryParse(text, out i))
                        {
                            value = i;
                            return true;
                        }
                    }
                    break;

                case "System.Char":
                    {
                        char c;
                        if (char.TryParse(text, out c))
                        {
                            value = c;
                            return true;
                        }
                    }
                    break;

                case "System.Boolean":
                    {
                        bool b;
                        if (bool.TryParse(text, out b))
                        {
                            value = b;
                            return true;
                        }
                    }
                    break;

                case "System.Byte":
                    {
                        byte b;
                        if (byte.TryParse(text, out b))
                        {
                            value = b;
                            return true;
                        }
                    }
                    break;

                case "System.SByte":
                    {
                        sbyte b;
                        if (sbyte.TryParse(text, out b))
                        {
                            value = b;
                            return true;
                        }
                    }
                    break;

                case "System.Int16":
                    {
                        short s;
                        if (short.TryParse(text, out s))
                        {
                            value = s;
                            return true;
                        }
                    }
                    break;

                case "System.UInt16":
                    {
                        ushort u;
                        if (ushort.TryParse(text, out u))
                        {
                            value = u;
                            return true;
                        }
                    }
                    break;

                case "System.UInt32":
                    {
                        uint u;
                        if (uint.TryParse(text, out u))
                        {
                            value = u;
                            return true;
                        }
                    }
                    break;

                case "System.Int64":
                    {
                        long l;
                        if (long.TryParse(text, out l))
                        {
                            value = l;
                            return true;
                        }
                    }
                    break;

                case "System.UInt64":
                    {
                        ulong u;
                        if (ulong.TryParse(text, out u))
                        {
                            value = u;
                            return true;
                        }
                    }
                    break;

                case "System.Single":
                    {
                        float f;
                        if (float.TryParse(text, out f))
                        {
                            value = f;
                            return true;
                        }
                    }
                    break;

                case "System.Double":
                    {
                        double d;
                        if (double.TryParse(text, out d))
                        {
                            value = d;
                            return true;
                        }
                    }
                    break;

                case "System.Decimal":
                    {
                        decimal d;
                        if (decimal.TryParse(text, out d))
                        {
                            value = d;
                            return true;
                        }
                    }
                    break;

                case "System.DateTime":
                    {
                        DateTime t;
                        if (DateTime.TryParse(text, out t))
                        {
                            value = t;
                            return true;
                        }
                    }
                    break;

                case "System.Guid":
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            return false;
                        }

                        const string p0 = @"^[0-9A-Za-z]{8,8}-[0-9A-Za-z]{4,4}-[0-9A-Za-z]{4,4}-[0-9A-Za-z]{4,4}-[0-9A-Za-z]{12,12}$";
                        const string p1 = @"^\{[0-9A-Za-z]{8,8}-[0-9A-Za-z]{4,4}-[0-9A-Za-z]{4,4}-[0-9A-Za-z]{4,4}-[0-9A-Za-z]{12,12}\}$";

                        if (Regex.IsMatch(text, p0) || Regex.IsMatch(text, p1))
                        {
                            value = new Guid(text);
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }
        /// <summary>
        /// 过滤 Sql 语句字符串中的注入脚本
        /// </summary>
        /// <param name="source">传入的字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string SqlFilter(string source)
        {
            source = source.Replace("'", "''");


            source = source.Replace(";", "；");


            source = source.Replace("(", "（");
            source = source.Replace(")", "）");




            source = source.Replace("Exec", string.Empty);
            source = source.Replace("Execute", string.Empty);


            source = source.Replace("xp_", "x p_");
            source = source.Replace("sp_", "s p_");


            source = source.Replace("0x", "0 x");

            return source;
        }
    }
}
