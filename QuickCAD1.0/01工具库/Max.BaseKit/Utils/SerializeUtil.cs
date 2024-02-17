using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Max.BaseKit.Utils
{
    /// <summary>
    /// 序列化常用操作
    /// </summary>
    public class SerializeUtil
    {
        #region C#可以使用，Java无法解析(与Java对接需要换其它方式)
        /// <summary>
        /// 反序列化字节数组
        /// </summary>
        /// <param name="bBuffer"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] bBuffer)
        {
            using (var ms = new MemoryStream(bBuffer, 0, bBuffer.Length))
            {
                ms.Position = 0;
                ms.Seek(0, SeekOrigin.Begin);
                var bf = new BinaryFormatter();
                return bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// 将obj对象进行序列化，并压缩
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object obj)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        #endregion
    }
}
