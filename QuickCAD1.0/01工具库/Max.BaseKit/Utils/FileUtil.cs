﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace Max.BaseKit.Utils
{
    public class FileUtil
    {
        /// <summary>
        /// 安全删除文件
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <param name="timeout">超时时长(单位：秒)，默认为0</param>
        /// <returns></returns>
        public static bool SafeDelete(string filePath, int timeout = 0)
        {
            if (!File.Exists(filePath)) return true;

            bool flag = false;
            int retryCount = 0;
            while (retryCount < timeout * 10)
            {
                try
                {
                    File.Delete(filePath); //尝试删除文件
                    flag = true;
                    break; //如果成功删除文件，则退出循环
                }
                catch (IOException ex) when (IsFileInUse(ex))
                {
                    // 文件被占用，等待一段时间后重试删除操作
                    Thread.Sleep(100);
                    retryCount++;
                }
                catch (Exception ex)
                {
                    flag = false;
                    NLogger.Warn(ex.ToString());
                }
            }
            return flag;
        }

        /// <summary>
        /// 检查文件是否被占用的辅助方法
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool IsFileInUse(IOException ex)
        {
            int errorCode = ex.HResult & 0xFFFF;
            return errorCode == 32 || errorCode == 33;
        }

        /// <summary>
        /// 采用流&独占方式读取文件到字节数组
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public static byte[] StreamRead(string filePath)
        {
            byte[] byteArry = default;
            if (!File.Exists(filePath)) return byteArry;
            //using (FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None)) //没必要用独占的方式
            using (FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    List<byte> bytelst = new List<byte>();//数据临时存储
                    while (true)
                    {
                        byte[] buff = new byte[1024 * 1024 * 2];
                        int r = fStream.Read(buff, 0, buff.Length);//返回实际读取到的字节
                        bytelst.AddRange(buff.Skip(0).Take(r));
                        if (r == 0) break;//当字节位0的时候 证明已经读取结束
                    }
                    byteArry = bytelst.ToArray();//数据字节数组
                }
                catch (IOException ex)
                {
                    NLogger.Warn($"StreamRead发生异常：{ex.Message}");
                }
            }
            return byteArry;
        }

        /// <summary>
        /// 采用流&独占方式读取文件到字符串
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <param name="encoding">编码(不指定编码时默认为Encoding.UTF8)</param>
        /// <returns></returns>
        public static string StreamRead(string filePath, Encoding encoding = null)
        {
            string rString = default;
            byte[] byteArry = StreamRead(filePath);
            if (byteArry == null || byteArry.Length <= 0) return rString;
            encoding = encoding ?? Encoding.UTF8;
            return encoding.GetString(byteArry);
        }

        /// <summary>
        /// 采用流&独占方式读取文件到base64字符串
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public static string StreamReadToBase64(string filePath)
        {
            string rString = default;
            byte[] byteArry = StreamRead(filePath);
            if (byteArry == null || byteArry.Length <= 0) return rString;
            return Convert.ToBase64String(byteArry);
        }

        /// <summary>
        /// 采用流方式写入数据到文件
        /// 如果文件不存在就新建文件，如果文件存在则覆盖原有文件内容
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public static bool StreamWrite(string filePath, byte[] bytes)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                NLogger.Warn($"StreamWrite发生异常：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 采用流&独占方式复制文件
        /// </summary>
        /// <param name="sourceFilePath">资源文件全路径</param>
        /// <param name="destinationFilePath">目标文件全路径</param>
        /// <returns></returns>
        public static bool StreamCopy(string sourceFilePath, string destinationFilePath)
        {
            bool flag = default;
            try
            {
                if (File.Exists(destinationFilePath)) File.Delete(destinationFilePath);// 如果目标文件已经存在，则删除它
                using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        sourceStream.CopyTo(destinationStream);
                    }
                }
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                NLogger.Warn($"StreamCopy发生异常：{ex.Message}");
            }
            return flag;
        }

        /// <summary>
        /// 采用流&独占方式移动文件
        /// </summary>
        /// <param name="sourceFilePath">资源文件全路径</param>
        /// <param name="destinationFilePath">目标文件全路径</param>
        /// <returns></returns>
        public static bool StreamMove(string sourceFilePath, string destinationFilePath)
        {
            bool flag = default;
            try
            {
                using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        sourceStream.CopyTo(destinationStream);
                    }
                }
                if (File.Exists(sourceFilePath)) File.Delete(sourceFilePath);
                flag = true;
            }
            catch (Exception ex)
            {
                flag = false;
                NLogger.Warn($"StreamMove发生异常：{ex.Message}");
            }
            return flag;
        }

        /// <summary>
        /// 清空文本
        /// </summary>
        /// <param name="txtPath">文本路径</param>
        public static void ClearTxt(string txtPath)
        {
            FileStream stream = File.Open(txtPath, FileMode.OpenOrCreate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            stream.Close();
        }

        /// <summary>
        /// 追加或覆盖文本
        /// </summary>
        /// <param name="txtPath">文本路径</param>
        /// <param name="str">要追加的文本</param>
        /// <param name="saOrAp">false为覆盖，true为追加</param>
        public static void SaveFile(string txtPath, string str, bool saOrAp)
        {
            StreamWriter sw = new StreamWriter(txtPath, saOrAp);//saOrAp表示覆盖或者是追加  
            sw.WriteLine(str);
            sw.Close();
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dirPath"></param>
        public static void ClearDirectory(string dirPath)
        {
            var filePaths = Directory.GetFiles(dirPath);
            foreach (string file in filePaths)
            {
                File.Delete(file);
            }
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// GetFileNameNoPath 获取不包括路径的文件名
        /// </summary>
        public static string GetFileNameNoPath(string filePath)
        {
            return Path.GetFileName(filePath);
        }
        /// <summary>
        /// 获取目标文件的大小
        /// </summary>
        public static int GetFileSize(string filePath)
        {
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var size = (int)fs.Length;
            fs.Close();

            return size;
        }
        /// <summary>
        /// ReadFileReturnBytes 从文件中读取二进制数据
        /// </summary>
        public static byte[] ReadFileReturnBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            var br = new BinaryReader(fs);

            var buff = br.ReadBytes((int)fs.Length);

            br.Close();
            fs.Close();

            return buff;
        }
        /// <summary>
        /// 使用UTF-8格式读取文本文件的内容。如果文件不存在，则返回空
        /// </summary>
        public static string ReadText(string file_path)
        {
            if (!File.Exists(file_path))
            {
                return null;
            }
            return File.ReadAllText(file_path, Encoding.UTF8);
        }
        /// <summary>
        /// WriteBuffToFile 将二进制数据写入文件中
        /// </summary>
        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            var bw = new BinaryWriter(fs);

            bw.Write(buff);
            bw.Flush();

            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// WriteBuffToFile 将二进制数据写入文件中
        /// </summary>
        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            var bw = new BinaryWriter(fs);

            bw.Write(buff, offset, len);
            bw.Flush();

            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// 将字符串写入指定文件.如果指定文件或目录不存在，则创建
        /// </summary>
        public static void WriteText(string filePath, string text)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// 查找特定路径下特定后缀名的含有指定字段的文件,并进行排序
        /// </summary>
        /// <param name="dirInfo">要查找的目录路径</param>
        /// <param name="subName">要包含的指定字段，若为""，则表示全部文件(夹)</param>
        /// <param name="pattern">指定文件后缀，例如"*.txt"，"*.json","*.xml"等</param>
        /// <returns>符合条件的文件(夹)名称列表</returns>
        public static List<FileInfo> FindFiles(DirectoryInfo dirInfo, string subName, string pattern)
        {
            List<FileInfo> files = new List<FileInfo>();
            if (!dirInfo.Exists) return files;
            foreach (var fInfo in dirInfo.EnumerateFiles(pattern))
            {
                if (string.IsNullOrEmpty(subName))
                {
                    files.Add(fInfo);
                }
                else
                {
                    if (fInfo.Name.Contains(subName.Trim()))
                    {
                        files.Add(fInfo);
                    }
                }
            }
            files.OrderBy(p => p.CreationTime);
            return files;
        }
    }
}
