using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Com.Helpers
{
    public static class JsonHelper
    {
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
