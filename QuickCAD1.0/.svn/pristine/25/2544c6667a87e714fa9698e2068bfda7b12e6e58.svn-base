using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Max.BaseKit.Utils
{
    /// <summary>
    /// ftp帮助类
    /// </summary>
    public class FtpUtil
    {
        public delegate void ProgressBar(string fileName, double progress);
        /// <summary>
        /// 错误日志输出
        /// </summary>
        public event Action<Exception> ErrorLog = (s) => { };
        /// <summary>
        /// ftp地址
        /// </summary>
        public string FtpUrl { get; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string UserName { get; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string PassWord { get; }
        private string baseTemp = "~HJ8kOPVH69K109G_o_OplK";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ftpUrl">ftp地址</param>
        /// <param name="userName">登录名</param>
        /// <param name="passWord">登录密码</param>
        public FtpUtil(string ftpUrl, string userName, string passWord)
        {
            this.FtpUrl = ftpUrl.TrimEnd('\\');
            this.UserName = userName;
            this.PassWord = passWord;
        }
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="filePath">要上传的文件地址</param>
        /// <param name="ftpDirName">ftp文件目录。例：ff/aa/</param>
        /// <param name="name">上传到ftp的文件名称</param>
        /// <param name="option">ftp配置</param>
        /// <returns></returns>

        public bool Upload(string filePath, string ftpDirName, string name, FtpOption option, ProgressBar progress = null)
        {
            var path = FtpUrl + ftpDirName.Replace("\\", "/").TrimStart('/');
            if (option.Equals(FtpOption.create))
            {
                MakeDir(ftpDirName);
            }
            FileInfo info = new FileInfo(filePath);
            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(path.TrimEnd('\\').TrimEnd('/') + "/" + name));
            request.UseBinary = true;
            request.Credentials = new NetworkCredential(UserName, PassWord);
            request.KeepAlive = false;
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.ContentLength = info.Length;
            int buffLenght = 2048;
            byte[] bs = new byte[buffLenght];
            FileStream fs = info.OpenRead();
            int allbye = (int)info.Length;
            int contentLen = 0;
            int startbye = 0;
            double percent = 0;
            try
            {
                Stream stream = request.GetRequestStream();
                contentLen = fs.Read(bs, 0, buffLenght);
                startbye = contentLen;
                while (contentLen != 0)
                {
                    if (progress != null)
                    {
                        progress.Invoke(filePath, percent);
                    }
                    stream.Write(bs, 0, contentLen);
                    contentLen = fs.Read(bs, 0, buffLenght);
                    startbye = contentLen + startbye;
                    if (allbye == 0) continue;
                    percent = (double)startbye / (double)allbye * 100;
                }
                stream.Close();
                stream.Dispose();
                fs.Close();
                fs.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog(ex);
                return false;
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="ftpFilePath">ftp文件路径。例如：text/a.mp4</param>
        /// <param name="filePath">要保存在的文件路径</param>
        /// <returns></returns>
        public bool Download(string ftpFilePath, string filePath, ProgressBar progress = null)
        {
            string tempPath = Path.GetDirectoryName(filePath) + @"\" + baseTemp + Guid.NewGuid().ToString().Replace("-", "") + @".temp";
            try
            {
                FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpUrl + ftpFilePath));
                request.UseBinary = true;
                request.Credentials = new NetworkCredential(UserName, PassWord);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                long cl = GetFileSize(ftpFilePath);
                int buffSize = 1024 * 1024 * 5;
                int readCount = 0;
                byte[] bs = new byte[buffSize];
                readCount = stream.Read(bs, 0, buffSize);
                FileStream tempStream = new FileStream(tempPath, FileMode.Create);
                double percent = 0;
                while (readCount > 0)
                {
                    tempStream.Write(bs, 0, readCount);
                    readCount = stream.Read(bs, 0, buffSize);
                    if (cl == 0) continue;
                    percent = (double)tempStream.Length / (double)cl * 100;
                    if (percent <= 100)
                    {
                        if (progress != null)
                        {
                            progress.Invoke(filePath, percent);
                        }
                    }
                }
                stream.Close();
                stream.Dispose();
                tempStream.Close();
                tempStream.Dispose();
                response.Close();
                response.Dispose();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Move(tempPath, filePath);
                return true;
            }
            catch (Exception ex)
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
                ErrorLog(ex);
                return false;
            }

        }
        /// <summary>
        /// 删除ftp文件
        /// </summary>
        /// <param name="fileName">文件名称。例1.mp4</param>
        /// <returns></returns>
        public bool DeleteFile(string fileName)
        {
            try
            {
                string url = FtpUrl + fileName.TrimStart('/');
                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
                reqFtp.UseBinary = true;
                reqFtp.KeepAlive = false;
                reqFtp.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFtp.Credentials = new NetworkCredential(UserName, PassWord);
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog(ex);
                return false;
            }

        }
        /// <summary>
        /// 获得文件大小
        /// </summary>
        /// <param name="url">FTP文件的完全路径</param>
        /// <returns></returns>
        public long GetFileSize(string url)
        {

            long fileSize = 0;
            try
            {
                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpUrl + url.TrimStart('/')));
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(UserName, PassWord);
                reqFtp.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                fileSize = response.ContentLength;

                response.Close();
            }
            catch (Exception ex)
            {
                ErrorLog(ex);
            }
            return fileSize;
        }
        /// <summary>
        /// 创建ftp目录
        /// </summary>
        /// <param name="dirName">ftp文件目录.例如“/test/fff/”</param>
        /// <returns></returns>
        public bool MakeDir(string dirName)
        {
            try
            {
                string fullDir = dirName.Substring(0, dirName.LastIndexOf("/"));
                string[] dirs = fullDir.Split('/');
                string curDir = "/";
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dir = dirs[i];
                    if (dir != null && dir.Length > 0)  //如果是以/开始的路径,第一个为空
                    {
                        curDir += dir + "/";
                        if (FtpDirExists(curDir)) continue;
                        string lll = FtpUrl + curDir.TrimStart('/');
                        FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpUrl + curDir.TrimStart('/')));
                        ftpWebRequest.UseBinary = true;
                        ftpWebRequest.Credentials = new NetworkCredential(UserName, PassWord);
                        ftpWebRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                        FtpWebResponse response = (FtpWebResponse)ftpWebRequest.GetResponse();
                        response.Close();
                        response.Dispose();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorLog(ex);
            }
            return false;
        }
        /// <summary>
        /// 判断ftp上文件路径是否存在
        /// </summary>
        /// <param name="ftpPath">ftp目录是否存在.例如“/test/fff/”</param>
        /// <returns></returns>
        public bool FtpDirExists(string ftpPath)
        {
            FtpWebRequest ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(FtpUrl + ftpPath.TrimStart('/')));
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.Credentials = new NetworkCredential(UserName, PassWord);
            ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectory;
            FtpWebResponse ftpWeb = null;
            try
            {
                ftpWeb = (FtpWebResponse)ftpWebRequest.GetResponse();
                FtpStatusCode code = ftpWeb.StatusCode;//OpeningData
                ftpWeb.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (ftpWeb != null)
                {
                    ftpWeb.Close();
                    ftpWeb.Dispose();
                }
                ErrorLog(ex);
                return false;
            }
        }


    }
    /// <summary>
    /// ftp配置
    /// </summary>
    public enum FtpOption
    {
        /// <summary>
        /// 如果ftp非有
        /// </summary>
        create,
        none
    }
}
