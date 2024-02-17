﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace WebApplication1.Models
{ 
    public class ValidateCode
    {
        /// <summary>
        /// 验证码的最大长度
        /// </summary>
        public int MaxLength { get; private set; } = 10;
        /// <summary>
        /// 验证码的最小长度
        /// </summary>
        public int MinLength { get; private set; } = 1;

        public string[] CreateValidateCode(int length)
        {
            List<string> nums = new List<string>();
            var rand = new Random();
            int count = 0;
            while (count++ < length)
            {
                var tmp = rand.Next(0, 10);
                nums.Add(tmp.ToString());
            }

            return nums.ToArray();
        }
        public byte[] CreateValidateGraphic(string[] checkCode)
        {
            if (checkCode == null || checkCode.Length <= 0)return null;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 32.5)), 60);
            System.Drawing.Graphics g = Graphics.FromImage(image);

            try
            {
                Random random = new Random();//生成随机生成器
                g.Clear(Color.White);//清空图片背景色
                //定义颜色
                Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                //画图片的背景噪音线
                for (int i = 0; i < 25; i++)
                {
                    int cindex = random.Next(7);
                    int findex = random.Next(5);
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    // g.DrawLine(new Pen(c[cindex]), x1, y1, x2, y2);
                }
                //定义字体
                string[] f = { "Arial" };

                for (int k = 0; k <= checkCode.Length - 1; k++)
                {
                    int cindex = random.Next(7);
                    int findex = random.Next(5);

                    Font drawFont = new Font(f[0], 32, (System.Drawing.FontStyle.Bold));
                    SolidBrush drawBrush = new SolidBrush(c[cindex]);

                    float x = 5.0F;
                    float y = 0.0F;
                    float width = 42.0F;
                    float height = 48.0F;
                    int sjx = random.Next(10);
                    int sjy = random.Next(image.Height - (int)height);

                    RectangleF drawRect = new RectangleF(x + sjx + (k * 25), y + sjy, width, height);

                    StringFormat drawFormat = new StringFormat();
                    drawFormat.Alignment = StringAlignment.Center;

                    g.DrawString(checkCode[k], drawFont, drawBrush, drawRect, drawFormat);
                }

                //画图片的前景噪音点
                for (int i = 0; i < 500; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }
                int cindex1 = random.Next(7);
                //画图片的边框线
                g.DrawRectangle(new Pen(c[cindex1]), 0, 0, image.Width - 1, image.Height - 1);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                return ms.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
        /// <summary>
        /// 得到验证码图片的长度
        /// </summary>
        /// <param name="validateNumLength">验证码的长度</param>
        /// <returns></returns>
        public static int GetImageWidth(int validateNumLength)
        {
            return (int)(validateNumLength * 12.0);
        }
        /// <summary>
        /// 得到验证码的高度
        /// </summary>
        /// <returns></returns>
        public static double GetImageHeight()
        {
            return 22.5;
        }
    }

    #region
    //public class ValidateCode
    //{
    //    public ValidateCode()
    //    {
    //    }
    //    /// <summary>
    //    /// 验证码的最大长度
    //    /// </summary>
    //    public int MaxLength
    //    {
    //        get { return 10; }
    //    }
    //    /// <summary>
    //    /// 验证码的最小长度
    //    /// </summary>
    //    public int MinLength
    //    {
    //        get { return 1; }
    //    }
    //    /// <summary>
    //    /// 生成验证码
    //    /// </summary>
    //    /// <param name="length">指定验证码的长度</param>
    //    /// <returns></returns>
    //    public string CreateValidateCode(int length)
    //    {
    //        int[] randMembers = new int[length];
    //        int[] validateNums = new int[length];
    //        string validateNumberStr = "";
    //        //生成起始序列值
    //        int seekSeek = unchecked((int)DateTime.Now.Ticks);
    //        Random seekRand = new Random(seekSeek);
    //        int beginSeek = (int)seekRand.Next(0, Int32.MaxValue - length * 10000);
    //        int[] seeks = new int[length];
    //        for (int i = 0; i < length; i++)
    //        {
    //            beginSeek += 10000;
    //            seeks[i] = beginSeek;
    //        }
    //        //生成随机数字
    //        for (int i = 0; i < length; i++)
    //        {
    //            Random rand = new Random(seeks[i]);
    //            int pownum = 1 * (int)Math.Pow(10, length);
    //            randMembers[i] = rand.Next(pownum, Int32.MaxValue);
    //        }
    //        //抽取随机数字
    //        for (int i = 0; i < length; i++)
    //        {
    //            string numStr = randMembers[i].ToString();
    //            int numLength = numStr.Length;
    //            Random rand = new Random();
    //            int numPosition = rand.Next(0, numLength - 1);
    //            validateNums[i] = Int32.Parse(numStr.Substring(numPosition, 1));
    //        }
    //        //生成验证码
    //        for (int i = 0; i < length; i++)
    //        {
    //            validateNumberStr += validateNums[i].ToString();
    //        }
    //        return validateNumberStr;
    //    }

    //    //C# MVC 升级版
    //    /// <summary>
    //    /// 创建验证码的图片
    //    /// </summary>
    //    /// <param name="containsPage">要输出到的page对象</param>
    //    /// <param name="validateNum">验证码</param>
    //    public byte[] CreateValidateGraphic(string validateCode)
    //    {
    //        Bitmap image = new Bitmap((int)Math.Ceiling(validateCode.Length * 12.0), 22);
    //        Graphics g = Graphics.FromImage(image);
    //        try
    //        {
    //            //生成随机生成器
    //            Random random = new Random();
    //            //清空图片背景色
    //            g.Clear(Color.White);
    //            //画图片的背景噪音线
    //            //for (int i = 0; i < 3; i++)
    //            //{
    //            //    int x1 = random.Next(image.Width);
    //            //    int x2 = random.Next(image.Width);
    //            //    int y1 = random.Next(image.Height);
    //            //    int y2 = random.Next(image.Height);
    //            //    g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
    //            //}
    //            //把产生的随机数以字体的形式写入画面
    //            Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold));
    //            //g.DrawString(validateCode, font, new SolidBrush(Color.Red), 2, 2);
    //                //Color[] c = { Color.Red, Color.Blue, Color.Orange, Color.Green, Color.Black };
    //                //Random rand = new Random();
    //                //int cindex = rand.Next(5);
    //                //Brush b = new System.Drawing.SolidBrush(c[cindex]);
    //            g.DrawString(validateCode, font,new SolidBrush(Color.FromArgb(0x42, 0x86, 0xCD)), 2, 2);
    //            //画图片的前景噪音点
    //            for (int i = 0; i < 10; i++)
    //            {
    //                int x = random.Next(image.Width);
    //                int y = random.Next(image.Height);
    //                image.SetPixel(x, y, Color.FromArgb(random.Next()));
    //            }
    //            //画图片的边框线
    //            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
    //            MemoryStream stream = new MemoryStream();
    //            image.Save(stream, ImageFormat.Jpeg);
    //            //输出图片流
    //            return stream.ToArray();
    //        }
    //        finally
    //        {
    //            g.Dispose();
    //            image.Dispose();
    //        }
    //    }

    //    /// <summary>
    //    /// 得到验证码图片的长度
    //    /// </summary>
    //    /// <param name="validateNumLength">验证码的长度</param>
    //    /// <returns></returns>
    //    public static int GetImageWidth(int validateNumLength)
    //    {
    //        return (int)(validateNumLength * 12.0);
    //    }
    //    /// <summary>
    //    /// 得到验证码的高度
    //    /// </summary>
    //    /// <returns></returns>
    //    public static double GetImageHeight()
    //    {
    //        return 22.5;
    //    }

    //}
    #endregion
}
