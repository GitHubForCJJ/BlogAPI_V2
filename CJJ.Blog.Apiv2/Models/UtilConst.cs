using CJJ.Blog.Apiv2.Helpers;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Models.View;
using FastDev.Common.Code;
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

        public static OpertionUser GetLoginOpt(string token)
        {
            var opt = new OpertionUser();
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return new OpertionUser();
                }
                var type = token.Substring(31, 1).Toint();
                if (type == 1)
                {
                    var sysuser = BlogHelper.GetSysLoginUserByToken(token);
                    opt.UserId = sysuser.Model.KID.ToString();
                    opt.UserName = sysuser.Model.UserName;
                    opt.UserClientIp = GetIP();

                }

            }
            catch (Exception ex)
            {

            }
            return opt;
        }
    }
}