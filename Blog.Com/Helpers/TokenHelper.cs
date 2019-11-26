using Blog.Common.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CJJ.Blog.Apiv2.Helpers
{
    public class TokenHelper
    {
        /// <summary>
        /// token 解密数据
        /// </summary>
        /// <param name="encrydata"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string TokenDecry(string encrydata, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(encrydata) || string.IsNullOrEmpty(token))
                {
                    return string.Empty;
                }

                string key = GetKeyByToken(token);
                return Des.Decrypt(encrydata, key);

            }
            catch
            {
                return string.Empty;
            }

        }
        public static string GetKeyByToken(string token)
        {
            try
            {
                StringBuilder key = new StringBuilder();
                for (int i = 0; i < token.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        key.Append(token[i]);
                    }
                    if (key.Length > 8)
                    {
                        break;
                    }
                }
                return key?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// token 加密数据
        /// </summary>
        /// <param name="encrydata"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string TokenEncry(string encrydata, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(encrydata) || string.IsNullOrEmpty(token))
                {
                    return string.Empty;
                }
                string key = GetKeyByToken(token);
                return Des.Encrypt(encrydata, key);
            }
            catch
            {
                return string.Empty;
            }

        }

    }
}