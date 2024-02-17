using System;
using System.IO;
using System.Net;

namespace Max.ISolator.FtpKit
{
    /// <summary>
    /// FTP客户端
    /// </summary>
    public class FtpClient
    {
        /// <summary>
        /// FTP客户端配置
        /// </summary>
        public FtpClientMod Mod { get; private set; }
        public FtpClient(FtpClientMod mod)
        {
            this.Mod = mod;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public void UploadFile(string filePath, string fileName)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(Mod.ServerUrl + fileName);// 创建FTP请求对象
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.Credentials = new NetworkCredential(Mod.UserName, Mod.Password);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))// 打开本地文件
            {
                using (Stream ftpStream = ftpRequest.GetRequestStream())// 获取FTP上传流
                {
                    // 将本地文件复制到FTP上传流中
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ftpStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            Console.WriteLine("文件上传成功！");
        }

        public void DownloadFile(string filePath, string fileName)
        {
            FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(Mod.ServerUrl + fileName);// 创建FTP请求对象
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            ftpRequest.Credentials = new NetworkCredential(Mod.UserName, Mod.Password);

            using (FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse())// 获取FTP响应对象
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))// 打开本地文件
                {
                    using (Stream ftpStream = ftpResponse.GetResponseStream())// 获取FTP下载流
                    {
                        // 将FTP下载流复制到本地文件中
                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;
                        while ((bytesRead = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }

            Console.WriteLine("文件下载成功！");
        }
    }
}
