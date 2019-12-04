using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.Models
{
    public static class CacheHelper
    {
        public static string Version = UtilConst.Version;

        public static void AddCacheItem(string key, string code)
        {
            var cachekey = $"{Version}_{key}";
            HttpContext.Current.Cache.Insert(cachekey, code, null, DateTime.Now.AddDays(1), System.Web.Caching.Cache.NoSlidingExpiration);
        }
        public static object GetCacheItem(string key)
        {
            var cachekey = $"{Version}_{key}";
            try
            {
                var cache = HttpContext.Current.Cache.Get(cachekey);
                HttpContext.Current.Cache.Remove(cachekey);
                return HttpContext.Current.Cache.Get(cachekey);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}