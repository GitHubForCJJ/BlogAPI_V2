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
                    c.IncludeXmlComments(GetXmlCommentsPath());//��swagger����xml�ĵ�������
                    c.DocumentFilter<HiddenApiFilter>();//�����������filter
                    c.CustomProvider((defaultProvider) => new SwaggerControllerDescProvider(defaultProvider, GetXmlCommentsPath()));//��ȡ��������ע�ͷ�����
                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("CJJ.Blog.Apiv2");
                    c.InjectJavaScript(thisAssembly, "CJJ.Blog.Apiv2.Models.SwaggerCustom.js");//����js
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
