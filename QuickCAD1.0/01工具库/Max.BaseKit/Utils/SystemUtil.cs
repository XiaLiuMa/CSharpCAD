using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Max.BaseKit.Utils
{
    public class SystemUtil
    {
        public static int GetProcessId()
        {
            Process processes = Process.GetCurrentProcess();
            return processes.Id;
        }

        public static int GetProcessId(string name)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName.Equals(name))
                {
                    return process.Id;
                }
            }
            return -1;
        }

        public static int GetThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// 获取系统时间【已处理windows系统和linux系统】
        /// </summary>
        public static DateTime GetSystemTime()
        {
            //var os = RuntimeInformation.OSDescription;//获取当前操作系统信息
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return DateTime.Now;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "date";
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return DateTime.Parse(output);
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"获取Linux系统时间异常：{ex.Message}");
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 设置系统时间【已处理windows系统和linux系统】
        /// </summary>
        /// <param name="dateTime"></param>
        public static void SetSystemTime(DateTime dateTime)
        {
            //var os = RuntimeInformation.OSDescription;//获取当前操作系统信息
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = $"/C date \"{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}\"";
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"设置Windows系统时间异常：{ex.Message}");
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                try
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo.FileName = "sudo";
                    process.StartInfo.Arguments = $"date -s '{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}'";
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    NLogger.Warn($"设置Linux系统时间异常：{ex.Message}");
                }
            }
        }
    }
}
