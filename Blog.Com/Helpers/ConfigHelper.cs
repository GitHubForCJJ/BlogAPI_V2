using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace CJJ.Blog.Apiv2.Helpers
{
    public class ConfigHelper
    {
        public static string GetConfToString(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key]?.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static int GetConfToInt(string key)
        {
            try
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings[key].ToString());
            }
            catch
            {
                return 0;
            }
        }
        public static bool GetConfToBool(string key)
        {
            try
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings[key].ToString());
            }
            catch
            {
                return false;
            }
        }
    }
}