//-----------------------------------------------------------------------------------
// <copyright file="Md5.cs" company="720U Enterprises">
// * copyright: (C) 2017 720U科技有限公司 版权所有。
// * version  : 1.0.0.0
// * author   : rongbo
// * fileName : Md5.cs
// * history  : created by rongbo 2017/3/13 14:12:33
// </copyright>
// <summary>
//   EastWestWalk.NetFrameWork.Common.Encrypt.Md5
// </summary>
//-----------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Common.Encrypt
{
    /// <summary>
    /// Md5
    /// </summary>	
    public class Md5
    {
        /// <summary>
        /// 加密32
        /// </summary>
        /// <param name="source">数据</param>
        /// <returns>密文</returns>
        public static string Encrypt32(string source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder sb = new StringBuilder(32);
            foreach (byte item in t)
            {
                sb.Append(item.ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 加密16
        /// </summary>
        /// <param name="source">数据</param>
        /// <returns>密文</returns>
        public static string Encrypt16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(source)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2.ToUpper();
        }


        /// <summary>  
        /// MD5 加密字符串  
        /// </summary>  
        /// <param name="rawPass">源字符串</param>  
        /// <returns>加密后字符串</returns>  
        public static string MD5Encoding(string rawPass)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider  
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化  
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>  
        /// MD5盐值加密  
        /// </summary>  
        /// <param name="rawPass">源字符串</param>  
        /// <param name="salt">盐值</param>  
        /// <returns>加密后字符串</returns>  
        public static string MD5Encoding(string rawPass, object salt)
        {
            if (salt == null) return rawPass;
            return MD5Encoding(rawPass + "{" + salt.ToString() + "}");
        }
    }
}