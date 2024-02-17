using ICSharpCode.SharpZipLib.GZip;
using System.IO;

namespace Max.BaseKit.Utils
{
    public class ZipUtil
    {
        /// <summary>
        /// GZip压缩
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipOutputStream zos = new GZipOutputStream(ms))
                {
                    zos.Write(data, 0, data.Length);
                    zos.Flush();
                    zos.Finish();
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// GZip解压
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream mos = new MemoryStream())
            {
                using (MemoryStream mis = new MemoryStream(data))
                {
                    using (GZipInputStream zis = new GZipInputStream(mis))
                    {
                        do
                        {
                            int i = zis.Read(buffer, 0, buffer.Length);
                            if (i <= 0) break;
                            mos.Write(buffer, 0, i);
                        } while (true);
                    }
                }
                return mos.ToArray();
            }
        }
    }
}
