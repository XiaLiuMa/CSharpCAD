﻿using System;
using System.IO;
using System.Threading;

namespace Max.BaseKit.Exts
{
    public static class StringExt
    {
        /// <summary>
        /// 移除换行符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str1">移除换行符时要替换的字符串，默认是替换成空格</param>
        /// <returns></returns>
        public static string RemoveWarp(this string str, string str1 = " ")
        {
            if(string.IsNullOrEmpty(str)) return string.Empty;
            return str.Replace("\r", string.Empty).Replace("\n", " "); // 去除回车并将换行转为空格
        }

        /// <summary>
        /// 移除前面字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str1"></param>
        /// <param name="isRemoveSpaces">是否移除空格</param>
        /// <returns></returns>
        public static string TrimStart(this string str, string str1, bool isRemoveSpaces = true)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (isRemoveSpaces) str = str.TrimStart(' ');
            if (!string.IsNullOrEmpty(str1))
            {
                str = str.TrimStart(str1.ToCharArray());
            }
            return str;
        }

        /// <summary>
        /// 移除后面字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str1"></param>
        /// <param name="isRemoveSpaces">是否移除空格</param>
        /// <returns></returns>
        public static string TrimEnd(this string str, string str1, bool isRemoveSpaces = true)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            if (isRemoveSpaces) str = str.TrimEnd(' ');
            if (!string.IsNullOrEmpty(str1))
            {
                str = str.TrimEnd(str1.ToCharArray());
            }
            return str;
        }

        /// <summary>
        /// 字符包含判断(全包含、半包含)
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <param name="AllInclusive">false：半包含，true：全包含</param>
        /// <param name="args">参与判断的字符</param>
        /// <returns></returns>
        public static bool JudgeInclusion(this string str, bool AllInclusive, params char[] args)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (args?.Length <= 0) return true;
            if (AllInclusive) //全包含
            {
                foreach (char c in args)
                {
                    if (!str.Contains(c)) return false;
                }
                return true;
            }
            else  //半包含
            {
                foreach (char c in args)
                {
                    if (str.Contains(c)) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 字符串包含判断(全包含、半包含)
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <param name="AllInclusive">false：半包含，true：全包含</param>
        /// <param name="args">参与判断的字符</param>
        /// <returns></returns>
        public static bool JudgeInclusion(this string str, bool AllInclusive, params string[] args)
        {
            if (string.IsNullOrEmpty(str)) return false;
            if (args?.Length <= 0) return true;
            if (AllInclusive) //全包含
            {
                foreach (string s in args)
                {
                    if (!str.Contains(s)) return false;
                }
                return true;
            }
            else  //半包含
            {
                foreach (string s in args)
                {
                    if (str.Contains(s)) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// string扩展方法
        /// </summary>
        /// <param name="tgsj"></param>
        /// <returns></returns>
        public static string FormatFssj(this string tgsj)
        {
            var tgsjFormated = tgsj.Substring(0, 4) + "-" + tgsj.Substring(4, 2) + "-" + tgsj.Substring(6, 2) + " " + tgsj.Substring(8, 2) + ":" + tgsj.Substring(10, 2) + ":" + tgsj.Substring(12, 2);
            return tgsjFormated;
        }
        /// <summary>
        /// string 扩展方法，通关时间转换
        /// </summary>
        /// <param name="tgsj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DateTime FormatFssj(this string tgsj, object args)
        {
            var tgsjFormated = tgsj.Substring(0, 4) + "-" + tgsj.Substring(4, 2) + "-" + tgsj.Substring(6, 2) + " " + tgsj.Substring(8, 2) + ":" + tgsj.Substring(10, 2) + ":" + tgsj.Substring(12, 2);
            return Convert.ToDateTime(tgsjFormated);
        }

        /// <summary>
        /// string 扩展方法,解析Wybs包含的时间
        /// </summary>
        /// <param name="wybs"></param>
        /// <returns></returns>
        public static DateTime ParseWybs(this string wybs)
        {
            var sj = wybs.Substring(8, 4) + "-" + wybs.Substring(12, 2) + "-" + wybs.Substring(14, 2) + " " + wybs.Substring(16, 2) + ":" + wybs.Substring(18, 2) + ":" + wybs.Substring(20, 2);
            return Convert.ToDateTime(sj);
        }

        /// <summary>
        /// 判断文件是否被占用(返回true是被占用，返回false是未被占用)
        /// </summary>
        /// <param name="fname">文件全名</param>
        /// <param name="timeout">超时时长(单位s)</param>
        /// <returns></returns>
        public static bool IsFileInUsing(this string fname, int timeout = 0)
        {
            if (!File.Exists(fname)) return false;
            bool isUse = true;
            if (timeout <= 0)
            {
                FileStream fs = default;
                try
                {
                    fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.None);
                    isUse = false;
                }
                catch
                {

                }
                finally
                {
                    if (fs != null) fs.Close();
                }
            }
            else
            {
                for (int i = 0; i < timeout * 10; i++)
                {
                    FileStream fs = default;
                    try
                    {
                        fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.None);
                        isUse = false;
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (fs != null) fs.Close();
                    }
                    if (!isUse) return isUse; //未被占用的情况下直接返回
                    Thread.Sleep(100);
                }
            }
            return isUse;
        }
    }
}
