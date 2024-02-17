﻿using System;
using System.Collections.Generic;

namespace Max.ISolator.TcpKit
{
    public delegate void ReceivedCallBack(byte[] bytes);
    public class MaxTcpCodec
    {
        /// <summary>
        /// 是否是大端模式
        /// </summary>
        public bool IsBigEnd { get; private set; }
        /// <summary>
        /// 数据头
        /// </summary>
        public byte[] Head { get; private set; }
        /// <summary>
        /// 数据头
        /// </summary>
        public byte[] Tail { get; private set; }

        public MaxTcpCodec(bool isbigend = false)
        {
            IsBigEnd = isbigend;
            Head = new byte[] { 0xF1, 0xF2, 0xF3, 0xF4 };
            Tail = new byte[] { 0xF4, 0xF3, 0xF2, 0xF1 };
        }

        /// <summary>
        /// 编码【4位包头+4位数据长度=数据+4位包尾】
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Coding(byte[] data)
        {
            if (data == null || data.Length <= 0) return null;
            try
            {
                List<byte> tbytes = new List<byte>();
                tbytes.AddRange(Head);//包头
                tbytes.AddRange(BitConverter.GetBytes(data.Length));//数据长度
                tbytes.AddRange(data);//数据
                tbytes.AddRange(Tail);//包尾
                byte[] bytes = tbytes.ToArray();
                if (IsBigEnd) Array.Reverse(bytes);
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"编码失败:{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 解码【2位总包数+2位第几个包+2位数据长度+数据】
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decoding(byte[] data)
        {
            //解码，在MaxTcpSession中粘包的时候已处理
            return data;
        }
    }
}
