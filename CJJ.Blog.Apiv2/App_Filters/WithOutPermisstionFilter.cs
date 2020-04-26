
using Blog.Common.Helpers;
using FastDev.Common.Encrypt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CJJ.Blog.Apiv2.App_Filters
{
    /// <summary>
    /// 权限过滤
    /// </summary>
    public class WithOutPermisstionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 在调用操作方法之前发生。
        /// </summary>
        /// <param name="actionContext">操作上下文。</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (ConfigHelper.GetConfToBool("IsDebug"))
            {
                return;
            }
            var model = (JsonRequest)actionContext.ActionArguments["model"];

            var datamd5 = Md5.MD5Encoding(model.Data + model.Timestamp.ToString(), model.Token);
            if (model.Md5 != datamd5)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, "请求数据MD5验证不合法" + model.Data + ";" +model.Token + ";" + model.Timestamp.ToString() + ";" + datamd5);
                return;
            }
            //des解码
            var dedata = TokenHelper.TokenDecry(model.Data.ToString(), model.Token);
            model.Data = dedata;
            return;
        }
    }
}