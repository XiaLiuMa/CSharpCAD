﻿using System;
using Max.BaseKit;
using RJCP.IO.Ports;
using System.Threading;

namespace Max.ISolator.SeriaPortKit
{
    /// <summary>
    /// 串口客户端(在linux下使用的时候必须把01Rely下的几个文件放在发布文件夹下使用)
    /// </summary>
    public class SerialPortCilent : SerialPortStream
    {
        /// <summary>
        /// 编码解码器
        /// </summary>
        public AbsSerialPortCodec Codec { get; private set; }
        /// <summary>
        /// 接收到数据事件
        /// </summary>
        public event Action<byte[]> OnReceived = (p) => { };

        public SerialPortCilent(string port, int baud, int data, int stopbits, int parity) : base(port, baud, data, (Parity)parity, (StopBits)stopbits)
        {
            Codec = new SerialPortCodec();

            PortName = port;
            BaudRate = baud;
            DataBits = data;
            Parity = (Parity)parity;
            StopBits = (StopBits)stopbits;

            Open();

            ErrorReceived += (sender, errorArgs) =>
            {
                Console.WriteLine($"串口收发数据发生错误：{errorArgs.EventType}");
            };

            DataReceived += (sender, dataReceiveArgs) =>
            {
                switch (dataReceiveArgs.EventType)
                {
                    case SerialData.NoData:
                        break;

                    case SerialData.Chars:
                        byte[] buffer = new byte[BytesToRead];
                        base.Read(buffer, 0, buffer.Length);
                        byte[] bytes = Codec.Decoding(buffer);//解码
                        if (bytes != null)
                        {
                            OnReceived?.Invoke(bytes);
                        }
                        break;
                    case SerialData.Eof:
                        break;
                    #region 有时会接收SerialData.Chars|SerialData.Eof数据
                    case SerialData.Chars | SerialData.Eof:
                        byte[] t_buffer = new byte[BytesToRead];
                        base.Read(t_buffer, 0, t_buffer.Length);
                        byte[] t_bytes = Codec.Decoding(t_buffer);//解码
                        if (t_bytes != null)
                        {
                            OnReceived?.Invoke(t_bytes);
                        }
                        break;
                    #endregion
                    default:
                        break;
                }
            };
        }

        private static object lckObj = new object();

        /// <summary>
        /// 发送字节数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool SendBytes(byte[] bytes)
        {
            lock (lckObj)
            {
                try
                {
                    byte[] data = Codec.Coding(bytes);//编码
                    if (data == null) return false;
                    base.Write(data, 0, data.Length);
                    Thread.Sleep(100);//休眠100毫秒，避免连续发送
                    return true;
                }
                catch (Exception ex)
                {
                    NLogger.Error($"向串口写入数据失败:{ex.Message}");
                    return false;
                }
            }
        }
    }
}
