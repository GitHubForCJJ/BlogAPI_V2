using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.Models
{
    /// <summary>
    /// 用户信息辅助类
    /// </summary>
    public class UserInfoUtil
    {
        /// <summary>
        /// 获取管理员用户信息
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>System.String.</returns>
        public static SysLoginUser UserInfo(string token)
        {
            var ipAddress = HttpContext.Current?.Request?.UserHostAddress;
            var agent = HttpContext.Current?.Request?.UserAgent;
            var dns = HttpContext.Current?.Request?.UserHostName;

            SysLoginUser logonUser = BlogHelper.GetUserInfoByToken(token, ipAddress, agent, dns);
            return logonUser;
        }
    }
}