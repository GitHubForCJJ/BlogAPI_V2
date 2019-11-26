using CJJ.Blog.Apiv2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.Models
{
    public static class UtilConst
    {
        public static string Version = ConfigHelper.GetConfToString("Version");
        public static string IsDebug = ConfigHelper.GetConfToString("IsDebug");
        public static string TestCode = ConfigHelper.GetConfToString("TestCode");


        public static string Agent = HttpContext.Current.Request.UserAgent;
        public static string Dns = "123123";

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetIP()
        {
            try
            {
                string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }

                if (string.IsNullOrEmpty(result))
                {
                    return "0.0.0.0";
                }

                return result;
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }
    }
}