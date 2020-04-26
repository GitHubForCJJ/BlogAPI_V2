using Blog.Common.Encrypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Blog.Common.Helpers
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

                string key = GetKeyByToken(token, 8);
                return Des.Decrypt(encrydata, key);

            }
            catch
            {
                return string.Empty;
            }

        }
        /// <summary>
        /// Gets the key by token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static string GetKeyByToken(string token, int keylength)
        {
            try
            {
                int idx = 1;
                StringBuilder key = new StringBuilder();
                for (int i = 0; i < token.Length; i++)
                {
                    if (i % 2 == 1 && idx <= keylength)
                    {
                        idx++;
                        key.Append(token[i]);
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
                string key = GetKeyByToken(token, 8);
                return Des.Encrypt(encrydata, key);
            }
            catch
            {
                return string.Empty;
            }

        }

    }
}