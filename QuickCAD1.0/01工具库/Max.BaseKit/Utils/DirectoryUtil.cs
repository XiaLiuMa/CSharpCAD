using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Max.BaseKit.Utils
{
    /// <summary>
    /// 目录工具
    /// </summary>
    public class DirectoryUtil
    {
        /// <summary>
        /// 获取当前程序的父级目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetAppParentDir(int index = 1)
        {
            string dir = AppContext.BaseDirectory;
            if (dir.EndsWith(@"\"))
            {
                dir = dir.Substring(0, dir.LastIndexOf(@"\"));
            }
            for (int i = 0; i < index; i++)
            {
                dir = dir.Substring(0, dir.LastIndexOf(@"\"));   // 或者写成这种格式
            }
            return dir;
        }

        /// <summary>
        /// 获取父级目录,默认上一级，可提取上几级目录
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetParentDirectory(string dir, int index = 1)
        {
            if (dir.EndsWith(@"\"))
            {
                dir = dir.Substring(0, dir.LastIndexOf(@"\"));
            }
            for (int i = 0; i < index; i++)
            {
                dir = dir.Substring(0, dir.LastIndexOf(@"\"));   // 或者写成这种格式
            }
            return dir;
        }


        /// <summary>
        /// 获得指定路径下所有文件名
        /// </summary>
        /// <param name="dir">文件夹</param>
        public static List<string> GetFileNames(string dir)
        {
            List<string> files = new List<string>();
            DirectoryInfo root = new DirectoryInfo(dir);
            foreach (FileInfo f in root.GetFiles())
            {
                files.Add(f.FullName);
            }
            return files;
        }

        /// <summary>
        /// 获得指定路径下所有文件(包括子目录下的文件)
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        public static void GetAllFileNames(string dir, ref List<string> list)
        {
            //绑定到指定的文件夹目录
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            //检索表示当前目录的文件和子目录
            FileSystemInfo[] fsinfos = dirInfo.GetFileSystemInfos();
            //遍历检索的文件和子目录
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                　　
                if (fsinfo is DirectoryInfo)    //判断是否为空文件夹
                {
                    GetAllFileNames(fsinfo.FullName,ref list);  //递归调用
                }
                else
                {
                    list.Add(fsinfo.FullName);
                }
            }
        }

        /// <summary>
        /// 获得指定路径下所有子目录名
        /// </summary>
        /// <param name="dir">文件夹路径</param>
        public static List<string> GetDirectorys(string dir)
        {
            List<string> dirs = new List<string>();
            DirectoryInfo root = new DirectoryInfo(dir);
            foreach (DirectoryInfo d in root.GetDirectories())
            {
                dirs.Add(d.FullName);
            }
            return dirs;
        }




        #region 测试
        //static void Main(string[] args)
        //{
        //    //获取当前程序所在的文件路径
        //    String rootPath = Directory.GetCurrentDirectory();
        //    string parentPath = Directory.GetParent(rootPath).FullName;//上级目录
        //    string topPath = Directory.GetParent(parentPath).FullName;//上上级目录
        //    StreamWriter sw = null;
        //    try
        //    {
        //        //创建输出流，将得到文件名子目录名保存到txt中
        //        sw = new StreamWriter(new FileStream("fileList.txt", FileMode.Append));
        //        sw.WriteLine("根目录：" + topPath);
        //        getDirectory(sw, topPath, 2);
        //    }
        //    catch (IOException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    finally
        //    {
        //        if (sw != null)
        //        {
        //            sw.Close();
        //            Console.WriteLine("完成");
        //        }
        //    }

        //}

        ///// <summary>
        ///// 获得指定路径下所有文件名
        ///// </summary>
        ///// <param name="sw">文件写入流</param>
        ///// <param name="path">文件写入流</param>
        ///// <param name="indent">输出时的缩进量</param>
        //public static void getFileName(StreamWriter sw, string path, int indent)
        //{
        //    DirectoryInfo root = new DirectoryInfo(path);
        //    foreach (FileInfo f in root.GetFiles())
        //    {
        //        for (int i = 0; i < indent; i++)
        //        {
        //            sw.Write("  ");
        //        }
        //        sw.WriteLine(f.Name);
        //    }
        //}

        ///// <summary>
        ///// 获得指定路径下所有子目录名
        ///// </summary>
        ///// <param name="sw">文件写入流</param>
        ///// <param name="path">文件夹路径</param>
        ///// <param name="indent">输出时的缩进量</param>
        //public static void getDirectory(StreamWriter sw, string path, int indent)
        //{
        //    getFileName(sw, path, indent);
        //    DirectoryInfo root = new DirectoryInfo(path);
        //    foreach (DirectoryInfo d in root.GetDirectories())
        //    {
        //        for (int i = 0; i < indent; i++)
        //        {
        //            sw.Write("  ");
        //        }
        //        sw.WriteLine("文件夹：" + d.Name);
        //        getDirectory(sw, d.FullName, indent + 2);
        //        sw.WriteLine();
        //    }
        //} 
        #endregion

    }
}
