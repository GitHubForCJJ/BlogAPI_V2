
using FastDev.Configer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.Models
{
    /// <summary>
    /// 所有config 配置
    /// </summary>
    public class ConfigUtil
    {
        /// <summary>
        /// 是否调试模式
        /// </summary>
        public static bool Isdebug = ConfigHelper.GetConfigToBool("IsDebug");
    }
}