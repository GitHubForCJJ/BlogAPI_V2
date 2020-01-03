using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CJJ.Blog.Apiv2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Json时间格式处理
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new IsoDateTimeConverter
            {
                DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
            });

            // Web API 跨域
            var allowOrigins = "*";//最好来自配置文件夹
            var allowHeaders = "*";//最好来自配置文件夹
            var allowMethods = "*";//最好来自配置文件夹
            var globalCors = new System.Web.Http.Cors.EnableCorsAttribute(allowOrigins, allowHeaders, allowMethods)
            {
                SupportsCredentials = true
            };
            config.EnableCors(globalCors);
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
