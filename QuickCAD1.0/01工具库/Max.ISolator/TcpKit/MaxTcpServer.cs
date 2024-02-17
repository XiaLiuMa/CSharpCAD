﻿using Max.BaseKit;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Max.ISolator.TcpKit
{
    public class MaxTcpServer
    {
        private TcpListener listener;
        /// <summary>
        /// 标识服务端是否关闭
        /// </summary>
        private bool isClose = false;
        /// <summary>
        /// 用于控制异步接受连接
        /// </summary>
        private ManualResetEvent doConnect;
        public string Ip { get; private set; }
        public int Port { get; private set; }
        /// <summary>
        /// 已建立连接的集合
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public ConcurrentDictionary<string, MaxTcpSession> Sessions { get; private set; }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event Action<string, byte[]> OnReceived;

        public MaxTcpServer(string ip, int port)
        {
            Ip = ip;
            Port = port;
            doConnect = new ManualResetEvent(false);
            Sessions = new ConcurrentDictionary<string, MaxTcpSession>();
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        public void StartAsync()
        {
            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(Ip);
            }
            catch (Exception e)
            {
                throw e;
            }
            listener = new TcpListener(new IPEndPoint(ipAddress, Port));
            listener.Start();
            ThreadPool.QueueUserWorkItem(p =>
            {
                while (!isClose)
                {
                    doConnect.Reset();
                    listener.BeginAcceptTcpClient(AcceptCallBack, listener);
                    doConnect.WaitOne();
                }
            });
        }

        /// <summary>
        /// 异步接收连接的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptCallBack(IAsyncResult ar)
        {
            TcpListener l = ar.AsyncState as TcpListener;
            TcpClient client = l.EndAcceptTcpClient(ar);
            doConnect.Set();
            IPEndPoint iep = client.Client.RemoteEndPoint as IPEndPoint;
            string key = $"{iep.Address.ToString()}:{iep.Port}";
            if (!Sessions.ContainsKey(key))
            {
                MaxTcpSession session = new MaxTcpSession(client);
                session.OnReceived += Session_OnReceived;
                session.OnDisconnected += Session_OnDisconnected;
                Sessions.TryAdd(key, session);
                NLogger.Info($"客户端{key}已连接，开始接收数据!");

                #region 有客户端连上，开始接收该客户端数据
                ThreadPool.QueueUserWorkItem(p =>
                {
                    while (Sessions.ContainsKey(key) && !isClose)
                    {
                        session?.ReceiveAsync();
                    }
                });
                #endregion
            }
        }

        /// <summary>
        /// 接收到一条完整数据事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        private void Session_OnReceived(string key, byte[] obj)
        {
            OnReceived(key, obj);
        }

        /// <summary>
        /// 客户端掉线事件
        /// </summary>
        /// <param name="key"></param>
        private void Session_OnDisconnected(string key)
        {
            MaxTcpSession session;
            Sessions.TryRemove(key, out session);
            NLogger.Info($"远程{key}连接断开。。。");
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="data">要发送的内容</param>
        public void SendAsync(string key, byte[] data)
        {
            if (data == null || data.Length <= 0) return;
            if (!Sessions.ContainsKey(key))
            {
                throw new Exception($"{key}不在连接字典中,请先连接!");
            }
            MaxTcpSession session = Sessions[key];
            session.SentAsync(data);
        }

        /// <summary>
        /// 异步广播
        /// </summary>
        /// <param name="data"></param>
        public void BroadcastAsync(byte[] data)
        {
            if (data == null || data.Length <= 0) return;
            foreach (var session in Sessions.Values)
            {
                session.SentAsync(data);
            }
        }

        public void Close()
        {
            isClose = true;
        }
    }
}
