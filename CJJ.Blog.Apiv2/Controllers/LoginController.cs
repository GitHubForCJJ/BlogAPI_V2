using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.View;
using FastDev;
using FastDev.Log;
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
    /// 管理端登录相关
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class LoginController : ApiController
    {
        /// <summary>
        /// 管理端密码登录
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse Login([FromBody] LoginModel model)
        {
            SysLoginUser user = new SysLoginUser { IsSucceed = false };
            try
            {
                if (string.IsNullOrEmpty(model.Account) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.CodeKey))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不完整" };
                }

                var code = CacheHelper.GetCacheItemAndDel(model.CodeKey)?.ToString();
                if (string.IsNullOrEmpty(code) || code != model.Code)
                {
                    return new JsonResponse { Code = 1, Msg = "验证码错误" };
                }

                user = BlogHelper.EmployeePasswordLogin(model.Account, model.Password, UtilConst.GetIP(), UtilConst.Agent, UtilConst.Dns);
                if (!CJJ.Blog.Apiv2.Models.ConfigUtil.Isdebug)
                {
                    user.DataIsEncrypt = true;
                }
                return new JsonResponse { Code = user.IsSucceed ? 0 : 1, Data = user };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "LoginController/Login错误");
                return new JsonResponse { Code = 1, Msg = "系统错误" + ex.Message };
            }

        }

        /// <summary>
        /// 获取验证码图片
        /// </summary>
        /// <param name="authCodeKey">The authentication code key.</param>
        /// <returns>
        /// HttpResponseMessage.
        /// </returns>
        [HttpGet]
        public HttpResponseMessage GetAuthCode(string authCodeKey)
        {
            ValidateCode vCode = new ValidateCode(5)
            {
                ImageWidth = 70,
                ImageHeight = 35
            };
            string code = string.Empty;
            if (!string.IsNullOrWhiteSpace(UtilConst.TestCode))
            {
                code = UtilConst.TestCode;
            }
            else
            {
                code = vCode.CreateValidateCode();
            }
            CacheHelper.AddCacheItem(authCodeKey, code,DateTime.Now.AddMinutes(15),Cache.NoSlidingExpiration,CacheItemPriority.High);
            byte[] bytes = vCode.CreateValidateGraphic(code);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return resp;
        }



    }
}
