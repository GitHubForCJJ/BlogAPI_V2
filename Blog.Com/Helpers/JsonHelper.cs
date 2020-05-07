using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Com.Helpers
{
    /// <summary>
    /// 拓展辅助类  写的有点乱
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 时间转为 yyyy-MM-dd hh:mm:ss格式
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        public static string Tostr(this DateTime time)
        {
            try
            {
                if (time == null)
                {
                    return "1990-01-01 08:00:00";
                }
                else
                {
                    return time.ToString("yyyy-MM-dd hh:mm:ss");
                }
            }
            catch
            {
                return "1990-01-01 08:00:00";
            }
        }
        public static List<T> DeserialObjectToList<T>(this object data)
        {
            try
            {
                string sdata = data.ToString();
                return JsonConvert.DeserializeObject<List<T>>(sdata);
            }
            catch(Exception e)
            {
                return null;
            }
        }
        public static T DeserialObject<T>(this object data)
        {
            try
            {
                string sdata = data.ToString();
                return JsonConvert.DeserializeObject<T>(sdata);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
        public static T DeserialObject<T>(this string data)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                return default(T);
            }
        }
        public static string SerializObject(this object data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string SerializObject<T>(this T data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
