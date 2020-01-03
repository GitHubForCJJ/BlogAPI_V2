using System.Web.Http;
using WebActivatorEx;
using CJJ.Blog.Apiv2;
using Swashbuckle.Application;
using CJJ.Blog.Apiv2.App_Filters;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace CJJ.Blog.Apiv2
{
    /// <summary>
    /// SwaggerConfig
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Registers this instance.
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "CJJ.Blog.Apiv2");
                    c.IncludeXmlComments(GetXmlCommentsPath());//让swagger根据xml文档来解析
                    c.DocumentFilter<HiddenApiFilter>();//添加隐藏属性filter
                    c.CustomProvider((defaultProvider) => new SwaggerControllerDescProvider(defaultProvider, GetXmlCommentsPath()));//获取控制器的注释方法类
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("CJJ.Blog.Apiv2");
                    c.InjectJavaScript(thisAssembly, "CJJ.Blog.Apiv2.Models.SwaggerCustom.js");//汉化js
                   // c.DisableValidator();
                });
        }
        /// <summary>
        /// Gets the XML comments path.
        /// </summary>
        /// <returns>System.String.</returns>
        private static string GetXmlCommentsPath()
        {
            return $"{System.AppDomain.CurrentDomain.BaseDirectory}/bin/CJJ.Blog.Apiv2.XML";
        }
    }
}
