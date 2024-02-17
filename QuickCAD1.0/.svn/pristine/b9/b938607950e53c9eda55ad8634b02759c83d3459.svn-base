using System;
using System.Net;
using Max.BaseKit;
using System.Net.Sockets;
using System.Threading;

namespace Max.ISolator.TcpKit
{
    public class MaxTcpClient
    {
        private MaxTcpSession session;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; private set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool IsConnected { get; private set; }
        /// <summary>
        /// 是否自动重连
        /// </summary>
        public bool AutoConnect { get; private set; }
        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event Action<byte[]> OnReceived = (p) => { };

        public MaxTcpClient(string ip, int port,bool autoConnect = true)
        {
            Ip = ip;
            Port = port;
            AutoConnect = autoConnect;
        }

        /// <summary>
        /// 接收到一条完整数据事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        private void Session_OnReceived(string key, byte[] obj)
        {
            OnReceived(obj);
        }
        /// <summary>
        /// 掉线事件(掉线处理)
        /// </summary>
        /// <param name="key"></param>
        private void Session_OnDisconnected(string key)
        {
            IsConnected = false;
            string msg = AutoConnect ? "5秒后重连" : "不再重连";
            NLogger.Info($"远程{key}连接断开，{msg}。。。");
            if (AutoConnect)
            {
                Thread.Sleep(5 * 1000);
                ConnectAsync();
            }
        }

        /// <summary>
        /// 连接服务端
        /// </summary>
        public void ConnectAsync()
        {
            if (IsConnected) return;
            try
            {
                IPAddress ipAddress = IPAddress.Parse(Ip);
                TcpClient client = new TcpClient();
                client.BeginConnect(ipAddress, Port, ConnectCallBack, client);
            }
            catch (Exception ex)
            {
                NLogger.Warn($"TcpClient开始连接异常:{ex.Message}");
                Session_OnDisconnected(string.Empty);
            }
        }

        /// <summary>
        /// 异步连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallBack(IAsyncResult ar)
        {
            TcpClient client = ar.AsyncState as TcpClient;
            try
            {
                client.EndConnect(ar);

                session = new MaxTcpSession(client);
                session.OnReceived += Session_OnReceived;
                session.OnDisconnected += Session_OnDisconnected;

                IsConnected = true;
                NLogger.Info("TcpClient建立连接，开始接收数据!");
                ThreadPool.QueueUserWorkItem(x =>
                {
                    while (IsConnected)
                    {
                        session.ReceiveAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                NLogger.Warn($"TcpClient连接回调异常:{ex.Message}");
                Session_OnDisconnected(string.Empty);
            }
        }

        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="data"></param>
        public void SendAsync(byte[] data)
        {
            session?.SentAsync(data);
        }
    }
}
