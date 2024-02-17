using Max.BaseKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Max.ISolator.SeriaPortKit
{
    /// <summary>
    /// 简单的编码/解码器【过滤无效的数据】
    /// </summary>
    public class SerialPortCodec : AbsSerialPortCodec
    {
        /// <summary>
        /// 数据头
        /// </summary>
        public short Head { get; set; } = 0xE01;
        /// <summary>
        /// 数据尾
        /// </summary>
        public short Tail { get; set; } = 0xF01;

        /// <summary>
        /// 编码【2位头+4位数据长度+数据+2位尾】
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Coding(byte[] data)
        {
            try
            {
                if (data == null || data.Length <= 0) return null;
                List<byte> byteList = new List<byte>();
                #region 大小端问题，需要转换
                byte[] headarray = BitConverter.GetBytes(Head);
                Array.Reverse(headarray);
                byteList.AddRange(headarray);

                byte[] lengarray = BitConverter.GetBytes(data.Length);
                Array.Reverse(lengarray);
                byteList.AddRange(lengarray);

                byteList.AddRange(data);

                byte[] tailarray = BitConverter.GetBytes(Tail);
                Array.Reverse(tailarray);
                byteList.AddRange(tailarray);
                #endregion
                return byteList.ToArray();
            }
            catch (Exception ex)
            {
                NLogger.Error($"编码器编码失败:{ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 解码【2位头+4位数据长度+数据+2位尾】
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decoding(byte[] data)
        {
            string time_str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            try
            {
                if (data == null || data.Length < 8) return null;
                #region 大小端问题，需要转换
                byte[] headarray = data.Skip(0).Take(2).ToArray();
                Array.Reverse(headarray);
                short _head = BitConverter.ToInt16(headarray, 0);
                if (!_head.Equals(Head)) return null;//校验数据头

                //NLogger.Debug($"{time_str}    解码接收到数据头");

                byte[] lengarray = data.Skip(2).Take(4).ToArray();
                Array.Reverse(lengarray);
                int _length = BitConverter.ToInt32(lengarray, 0);//数据长度
                if (data.Length < _length + 8) return null;//校验数据完整性

                byte[] bytes = data.Skip(6).Take(_length).ToArray();//数据
                //NLogger.Debug($"{time_str}    解码接收到数据，长度{_length}");

                byte[] tailarray = data.Skip(6 + _length).Take(2).ToArray();
                Array.Reverse(tailarray);
                short _tail = BitConverter.ToInt16(tailarray, 0);
                if (!_tail.Equals(Tail)) return null;//校验数据尾 
                #endregion
                //NLogger.Debug($"{time_str}    解码接收到数据尾");
                return bytes;
            }
            catch (Exception ex)
            {
                NLogger.Error($"解码器解码失败:{ex.Message}");
                return null;
            }
        }
    }
}
