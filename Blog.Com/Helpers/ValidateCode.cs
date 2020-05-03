using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Com.Helpers
{
    /// <summary>
    /// 生成验证码的类
    /// </summary>
    public class ValidateCode
    {
        /// <summary>
        /// 验证码的最大长度
        /// </summary>
        public static readonly int MaxLength = 10;

        /// <summary>
        /// 验证码的最小长度
        /// </summary>
        public static readonly int MinLength = 1;

        /// <summary>
        /// 验证码长度
        /// </summary>
        private readonly int length;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="length">验证码长度</param>
        public ValidateCode(int length)
        {
            if (length > 10 || length < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            this.length = length;
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns>验证码</returns>
        public string CreateValidateCode()
        {
            int[] randMembers = new int[length];
            int[] validateNums = new int[length];
            string validateNumberStr = "";
            //生成起始序列值
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random seekRand = new Random(seekSeek);
            int beginSeek = seekRand.Next(0, int.MaxValue - length * 10000);
            int[] seeks = new int[length];
            for (int i = 0; i < length; i++)
            {
                beginSeek += 10000;
                seeks[i] = beginSeek;
            }
            //生成随机数字
            for (int i = 0; i < length; i++)
            {
                Random rand = new Random(seeks[i]);
                int pownum = 1 * (int)Math.Pow(10, length);
                randMembers[i] = rand.Next(pownum, int.MaxValue);
            }
            //抽取随机数字
            for (int i = 0; i < length; i++)
            {
                string numStr = randMembers[i].ToString();
                int numLength = numStr.Length;
                Random rand = new Random();
                int numPosition = rand.Next(0, numLength - 1);
                validateNums[i] = int.Parse(numStr.Substring(numPosition, 1));
            }
            //生成验证码
            for (int i = 0; i < length; i++)
            {
                validateNumberStr += validateNums[i].ToString();
            }
            return validateNumberStr;
        }

        /// <summary>
        /// 创建验证码的图片
        /// </summary>
        /// <param name="validateCode">验证码</param>
        /// <returns>数组</returns>
        public byte[] CreateValidateGraphic(string validateCode)
        {
            int width = this.ImageWidth;
            int hegiht = this.ImageHeight;
            if (width <= 0)
            {
                width = (int)Math.Ceiling(validateCode.Length * 12.0);
            }
            if (hegiht <= 0)
            {
                hegiht = 36;
            }
            Bitmap image = new Bitmap(width, hegiht);
            Graphics g = Graphics.FromImage(image);
            try
            {
                // 生成随机生成器
                Random random = new Random();

                // 清空图片背景色
                g.Clear(Color.Aqua);

                // 画图片的干扰线
                for (int i = 0; i < 25; i++)
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height),
                    Color.Blue, Color.DarkRed, 1.2F, true);
                g.DrawString(validateCode, font, brush, 8, 8);

                //画图片的前景干扰点
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);
                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //保存图片数据
                MemoryStream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Jpeg);

                //输出图片流
                return stream.ToArray();
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 设置或获取验证码图片的长度
        /// //return (int)(validateNumLength * 12.0);
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// 设置或获取验证码的高度
        ///  return 22.5;
        /// </summary>
        public int ImageHeight { get; set; }
    }
}
