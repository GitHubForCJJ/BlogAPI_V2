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
            OpertionUser opt = null;
            try
            {
                opt = new OpertionUser();
                if (string.IsNullOrEmpty(token))
                {
                    return opt;
                }
                var type = token.Substring(31, 1).Toint();
                var sysuser = BlogHelper.GetSysLoginUserByToken(token);
                if (type == 1)
                {
                    opt.UserId = sysuser.Model.KID.ToString();
                    opt.UserName = sysuser.Model.UserName;
                    opt.UserClientIp = GetIP();
                }
                else if (type == 2)
                {
                    opt.UserId = sysuser.MemberModel.KID.ToString();
                    opt.UserName = sysuser.MemberModel.UserName;
                    opt.UserClientIp = GetIP();
                }

            }
            catch (Exception ex)
            {

            }
            return opt;
        }

        /// <summary>
        /// Adds the base information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="token">The token.</param>
        /// <param name="isAdd">if set to <c>true</c> [is add].</param>
        /// <param name="opt">The opt.</param>
        /// <returns></returns>
        public static Dictionary<string, object> AddBaseInfo<T>(Dictionary<string, object> dic, string token, bool isAdd, ref OpertionUser opt)
        {
            var user = UtilConst.GetLoginOpt(token);

            var propertys = typeof(T).GetProperties();
            if (propertys.Count(x => x.Name == "CreateTime") > 0 && isAdd)
            {
                dic.Add("CreateTime", DateTime.Now); ;
            }
            if (propertys.Count(x => x.Name == "CreateUserId") > 0 && isAdd)
            {
                dic.Add("CreateUserId", user.UserId);
            }
            if (propertys.Count(x => x.Name == "CreateUserName") > 0 && isAdd)
            {
                dic.Add("CreateUserName", user.UserName);
            }
            if (propertys.Count(x => x.Name == "UpdateUserId") > 0)
            {
                dic.Add("UpdateUserId", user.UserId);
            }
            if (propertys.Count(x => x.Name == "UpdateUserName") > 0)
            {
                dic.Add("UpdateUserName", user.UserName);
            }
            if (propertys.Count(x => x.Name == "UpdateTime") > 0)
            {
                dic.Add("UpdateTime", DateTime.Now);
            }
            opt.UserId = user.UserId;
            opt.UserName = user.UserName;
            opt.UserClientIp = GetIP();

            return dic;
        }
    }
}