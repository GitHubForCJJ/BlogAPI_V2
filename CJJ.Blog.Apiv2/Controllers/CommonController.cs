using Blog.Com.Helpers;
using CJJ.Blog.Apiv2.Models;
using FastDev.Log;
using Qiniu.Storage;
using Qiniu.Util;
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

            CacheHelper.AddCacheItem($"Blogimgcode_{authcodekey}", code, DateTime.Now.AddMinutes(15), Cache.NoSlidingExpiration, CacheItemPriority.High);

            byte[] bytes = vCode.CreateValidateGraphic(code);
            var resp = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return resp;
        }
        #endregion

        #region 七牛
        /// <summary>
        /// 获取七牛上传鉴权ToKen
        /// </summary>
        /// <returns>FastDev.Http.JsonResponse.</returns>
        [HttpPost]
        public JsonResponse GetQiNiuUploadToken()
        {
            try
            {
                string qiniutoken = "QiNiuUploadToken";
                string token = CacheHelper.GetCacheItem(qiniutoken)?.ToString();
                if (string.IsNullOrWhiteSpace(token))
                {
                    Mac mac = new Mac(ConfigUtil.QiNiuAccessKey, ConfigUtil.QiNiuSecretKey);
                    PutPolicy putPolicy = new PutPolicy();
                    // 设置要上传的目标空间
                    putPolicy.Scope = ConfigUtil.QiNiuBucket;
                    // 上传策略的过期时间(单位:秒)
                    putPolicy.SetExpires(2 * 60 * 60);
                    // 生成上传token
                    token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                    CacheHelper.AddCacheItem(qiniutoken, token, DateTime.Now.AddSeconds(2 * 60 * 50), Cache.NoSlidingExpiration, CacheItemPriority.High);
                }
                return new JsonResponse() { Code = 0, Msg = $"请求成功", Data = token };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "/Comm/GetQiNiuUploadToken");
                return new JsonResponse() { Code = 1, Msg = $"请求失败：{ex.Message}" };
            }
        }
        #endregion
    }
}
