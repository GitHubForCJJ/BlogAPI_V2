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
            HttpContext.Current.Cache.Insert(cachekey, code);
        }
        public static object GetCacheItem(string key)
        {
            var cachekey = $"{Version}_{key}";
            try
            {
                return HttpContext.Current.Cache.Get(cachekey);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}