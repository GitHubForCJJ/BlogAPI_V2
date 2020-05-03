using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace CJJ.Blog.Apiv2.Models
{
    /// <summary>
    /// 缓存管理加version  方便缓存数据库切换
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// The version
        /// </summary>
        public static string Version = UtilConst.Version;

       

        /// <summary>
        /// 添加缓存  如果key存在会覆盖之前的值
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="code">The code.</param>
        /// <param name="dateTime">绝对时间</param>
        /// <param name="timeSpan">滑动时间 Cache.NoSlidingExpiration</param>
        /// <param name="cacheItemPriority">策略 CacheItemPriority.High</param>
        public static void AddCacheItem(string key, string code,DateTime dateTime,TimeSpan timeSpan, CacheItemPriority cacheItemPriority)
        {
            var cachekey = $"{Version}_{key}";
            //HttpContext.Current.Cache.Insert(cachekey, code, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
            HttpRuntime.Cache.Insert(cachekey, code, null, dateTime, timeSpan, cacheItemPriority, null);
        }
        /// <summary>
        /// 获取缓存并删除
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetCacheItemAndDel(string key)
        {
            var cachekey = $"{Version}_{key}";
            try
            {
                var cache = HttpRuntime.Cache.Get(cachekey);
                HttpRuntime.Cache.Remove(cachekey);
                return cache;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 获取缓存并删除
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object GetCacheItem(string key)
        {
            var cachekey = $"{Version}_{key}";
            try
            {
                var cache = HttpRuntime.Cache.Get(cachekey);
                return cache;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static object DelCacheItem(string key)
        {
            var cachekey = $"{Version}_{key}";
            try
            {
                var cache = HttpRuntime.Cache.Remove(cachekey);
                return cache;
            }
            catch 
            {
                return null;
            }
        }
    }
}