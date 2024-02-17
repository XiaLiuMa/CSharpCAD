using System;
using System.Runtime.InteropServices;

namespace Max.BaseKit.Utils
{
    public static class StructUtil
    {
        /// <summary>
        /// 字节数组转换成结构体
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object BytesToStruct(byte[] bytes, Type type)
        {
            var size = Marshal.SizeOf(type);

            if (size > bytes.Length)
            {
                return null;
            }
            var structPtr = Marshal.AllocHGlobal(size);

            object obj;

            try
            {
                Marshal.Copy(bytes, 0, structPtr, size);

                obj = Marshal.PtrToStructure(structPtr, type);
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }

            return obj;
        }
        /// <summary>
        /// 结构体转换成字节数组
        /// </summary>
        /// <param name="structObj">结构体</param>
        /// <returns>返回相应的字节数组</returns>
        public static byte[] StructToBytes(object structObj)
        {
            var size = Marshal.SizeOf(structObj);

            var bytes = new byte[size];

            var structPtr = Marshal.AllocHGlobal(size);

            try
            {
                Marshal.StructureToPtr(structObj, structPtr, false);

                Marshal.Copy(structPtr, bytes, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }

            return bytes;
        }
    }
}
