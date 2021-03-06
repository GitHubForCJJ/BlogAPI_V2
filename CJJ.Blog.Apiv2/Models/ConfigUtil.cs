﻿
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

        /// <summary>
        /// The test code
        /// </summary>
        public static string TestCode= ConfigHelper.GetConfigToString("TestCode");

        /// <summary>
        /// 缓存所有的list  然后在内存中赛选
        /// </summary>
        public static string BlogListCacheKey = "BlogListCacheKey";
        /// <summary>
        /// 所有类型的
        /// </summary>
        public static string BlogTypeListCacheKey = "BlogTypeListCacheKey";
        /// <summary>
        /// 单个 前缀部分
        /// </summary>
        public static string BlogItemCacheKeyPrefix = "BlogItemCacheKeyPrefix";
        /// <summary>
        /// QiNiuAccessKey
        /// </summary>
        public static string QiNiuAccessKey= ConfigHelper.GetConfigToString("QiNiuAccessKey");
        /// <summary>
        /// QiNiuAccessKey
        /// </summary>
        public static string QiNiuSecretKey = ConfigHelper.GetConfigToString("QiNiuSecretKey");
        /// <summary>
        /// 上传目标空间
        /// </summary>
        public static string QiNiuBucket = ConfigHelper.GetConfigToString("QiNiuBucket");
    }
}