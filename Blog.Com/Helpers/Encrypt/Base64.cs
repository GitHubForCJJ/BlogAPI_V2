using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Common.Encrypt
{
    /// <summary>
    /// 
    /// </summary>
    public class Base64
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="code_type">The code_type.如 utf-8</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static string EncodeBase64(string code, string code_type = "utf-8")
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="code_type">The code_type.如 utf-8</param>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static string DecodeBase64(string code, string code_type = "utf-8")
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
    }
}
