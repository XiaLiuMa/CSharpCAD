using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Max.ISolator.TcpKit
{
    public class MaxTcpSession : IDisposable
    {
        /// <summary>
        /// 接收缓冲区
        /// </summary>
        private byte[] Rbuffer { get; set; }

        /// <summary>
        /// 客户端通道
        /// </summary>
        public TcpClient Client { get; private set; }
        /// <summary>
        /// 编解码器
        /// </summary>
        public MaxTcpCodec Codec { get; private set; }
        /// <summary>
        /// 用于控制异步接收数据
        /// </summary>
        private ManualResetEvent doReceive { get; set; }
        private CancellationTokenSource cts;
        private BlockingCollection<byte[]> reBlock;
        private BlockingCollection<byte[]> stBlock;
        public event Action<string> OnDisconnected = (s) => { };
        public event Action<string, byte[]> OnReceived = (s, b) => { };
        public MaxTcpSession(TcpClient client)
        {
            Client = client;
            Rbuffer = new byte[1 * 512];
            Codec = new MaxTcpCodec();
            cts = new CancellationTokenSource();
            doReceive = new ManualResetEvent(false);
            reBlock = new BlockingCollection<byte[]>(5000);
            stBlock = new BlockingCollection<byte[]>(5000);
            OrderlySent();
            OrderlyReceive();
        }

        /// <summary>
        /// 有序接收
        /// </summary>
        private void OrderlyReceive()
        {
            IPEndPoint iep = Client.Client.RemoteEndPoint as IPEndPoint;
            string key = $"{iep.Address.ToString()}:{iep.Port}";
            Task.Factory.StartNew(() =>
            {
                List<byte> reLst = new List<byte>();//积累接收到的字节
                foreach (var reBytes in reBlock.GetConsumingEnumerable())
                {
                    //NLogger.Info($"{JsonUtil.ObjectToStr(reBytes)}");
                    reLst.AddRange(reBytes);
                    #region 接收-解码-粘包
                    while (true)
                    {
                        int len1 = Codec.Head.Length;
                        if (reLst.Count < len1) break;
                        var head = reLst.Take(len1);//①包头
                        if (!head.SequenceEqual(Codec.Head))
                        {
                            reLst.RemoveAt(0);//校验失败，移除第一个字节
                            continue;
                        }
                        int len2 = len1 + 4;
                        if (reLst.Count < len2) break;
                        var dataLen = BitConverter.ToInt32(reLst.Skip(len1).Take(4).ToArray(), 0);//②数据长度
                        int len3 = len2 + dataLen;
                        if (reLst.Count < len3) break;
                        var dataBytes = reLst.Skip(len2).Take(dataLen).ToArray();//③数据
                        int len4 = len3 + Codec.Tail.Length;
                        if (reLst.Count < len4) break;
                        var tail = reLst.Skip(len3).Take(Codec.Tail.Length);//④包尾
                        if (!tail.SequenceEqual(Codec.Tail))
                        {
                            reLst.RemoveAt(0);//校验失败，移除第一个字节
                        }
                        else
                        {
                            reLst.RemoveRange(0, len4);//校验成功，移除前面所有的校验
                            OnReceived(key, dataBytes);//接收到完整的一条数据
                        }
                        continue;
                    }
                    #endregion
                }
            }, cts.Token);
        }

        /// <summary>
        /// 有序发送
        /// </summary>
        private void OrderlySent()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var stBytes in stBlock.GetConsumingEnumerable())
                {
                    byte[] bagBytes = Codec.Coding(stBytes);//编码
                    if (bagBytes == null || bagBytes.Length <= 0) continue;
                    Client.Client.Send(bagBytes);
                }
            }, cts.Token);
        }

        /// <summary>
        /// 开始异步接收数据
        /// </summary>
        public void ReceiveAsync()
        {
            doReceive.Reset();
            try
            {
                Client.Client.BeginReceive(Rbuffer, 0, Rbuffer.Length, SocketFlags.None, ReceiveCallBack, Client);
            }
            catch (Exception ex)
            {
                OfflineProcess($"开始接收数据异常:{ex.Message}");
            }
            doReceive.WaitOne();
        }

        /// <summary>
        /// 异步接收数据的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            TcpClient obj = ar.AsyncState as TcpClient;
            IPEndPoint iep = obj.Client.RemoteEndPoint as IPEndPoint;
            string key = $"{iep.Address.ToString()}:{iep.Port}";
            try
            {
                int count = obj.Client.EndReceive(ar);//接收到的有效数据长度
                if (count <= 0)
                {
                    OfflineProcess();
                }
                else
                {
                    var iBytes = Rbuffer.Take(count);//有效数据
                    reBlock.TryAdd(iBytes.ToArray());
                }
                doReceive.Set();
            }
            catch (Exception ex)
            {
                OfflineProcess($"接收数据回调异常:{ex.Message}");
            }
        }

        public void SentAsync(byte[] data)
        {
            lock (Client)
            {
                stBlock.TryAdd(data);
            }
        }

        /// <summary>
        /// 掉线处理
        /// </summary>
        private void OfflineProcess(string msg = null)
        {
            if (Client.Connected)
            {
                Client.Client.Disconnect(true);
            }

            IPEndPoint iep = Client.Client.RemoteEndPoint as IPEndPoint;
            string key = $"{iep.Address.ToString()}:{iep.Port}";
            OnDisconnected?.Invoke(key);
            Dispose();
        }

        public void Dispose()
        {
            cts?.Cancel();
            reBlock.Dispose();
            stBlock.Dispose();
            Client.Dispose();
            GC.Collect();
        }
    }
}
