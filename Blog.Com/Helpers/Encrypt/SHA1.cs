//-----------------------------------------------------------------------------------
// <copyright file="SHA1.cs" company="720U Enterprises">
// * copyright: (C) 2017 720U科技有限公司 版权所有。
// * version  : 1.0.0.0
// * author   : rongbo
// * fileName : SHA1.cs
// * history  : created by rongbo 2017/3/13 14:13:07
// </copyright>
// <summary>
//   EastWestWalk.NetFrameWork.Common.Encrypt.SHA1
// </summary>
//-----------------------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Common.Encrypt
{
    /// <summary>
    /// SHA1
    /// </summary>	
    public class SHA1
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="source">数据</param>
        /// <returns>密文</returns>
        public static string Encrypt(string source)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] str1 = Encoding.UTF8.GetBytes(source);
            byte[] str2 = sha1.ComputeHash(str1);
            sha1.Clear();
            ((IDisposable)sha1).Dispose();
            return Convert.ToBase64String(str2);
        }
    }
}