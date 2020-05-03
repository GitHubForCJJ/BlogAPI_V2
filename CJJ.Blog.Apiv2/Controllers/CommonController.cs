using Blog.Com.Helpers;
using CJJ.Blog.Apiv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Caching;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 公用
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class CommonController : ApiController
    {
        #region 获取验证码 
        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <param name="authcodekey">?authcodekey=asdasdas</param>
        /// <returns>HttpResponseMessage.</returns>
        [HttpGet]
        public HttpResponseMessage GetAuthCode(string authcodekey)
        {
            ValidateCode vCode = new ValidateCode(5)
            {
                ImageWidth = 70,
                ImageHeight = 35
            };
            string code;
            if (!string.IsNullOrWhiteSpace(ConfigUtil.TestCode))
            {
                code = ConfigUtil.TestCode;
            }
            else
            {
                code = vCode.CreateValidateCode();
            }

            HttpRuntime.Cache.Insert($"Blogimgcode_{authcodekey}", code, null, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration, CacheItemPriority.High, null);

            byte[] bytes = vCode.CreateValidateGraphic(code);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return resp;
        }
        #endregion
    }
}
