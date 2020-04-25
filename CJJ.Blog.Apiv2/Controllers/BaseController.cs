using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using CJJ.Blog.Apiv2.App_Filters;
using System.Web.Http;
using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.Apiv2.Helpers;
using Newtonsoft.Json;
using System.Web;
using CJJ.Blog.Service.Models.View;
using Newtonsoft.Json.Converters;
using FastDev.Common.Extension;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 菜单相关
    /// </summary>
    [WithOutPermisstion]
    public class BaseController : ApiController
    {
        /// <summary>
        /// LayerDemo自定义Json格式.
        /// </summary>
        /// <param name="data">数据实体</param>
        /// <param name="token">The token.</param>
        /// <param name="retMsg">错误信息</param>
        /// <param name="reqHash">The req hash.</param>
        /// <param name="retCode">回应编码 0正常 成功,其他数据为失败</param>
        /// <param name="retCnt">当为数据分页时,需要给条数,以便分页</param>
        /// <returns>JsonResult.</returns>
        [HiddenApi]
        public JsonResponse FastJson(object data, string token = "", int retCode = 0, string retMsg = "请求成功", int retCnt = 0, string reqHash = "")
        {
            if (ConfigUtil.Isdebug)
            {
                data = data.SerializeObject();
            }
            else
            {
                data = Des.Encrypt(data.SerializeObject(), TokenHelper.GetKeyByToken(token, 8));
            }

            return new JsonResponse() { Code = retCode, Count = retCnt, Msg = retMsg, Data = data };
        }
        /// <summary>
        /// 加密的返回，前端根据code来判断是否处理数据
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <param name="token">登录token</param>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HiddenApi]
        public JsonResponse FastResponse(object data, string token, int code = 0, int count = 0, string msg = "")
        {
            var setting = new JsonSerializerSettings { };
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            var bdata = JsonConvert.SerializeObject(data, timeFormat);
            if (ConfigHelper.GetConfToBool("IsDebug"))
            {
                return new JsonResponse
                {
                    Code = code,
                    Data = bdata,
                    Count= count,
                    Msg = msg
                };
            }
            else
            {
                var encrydata = TokenHelper.TokenEncry(bdata, token);
                return new JsonResponse
                {
                    Code = code,
                    Data = encrydata,
                    Msg = msg,
                    Count = count,
                };
            }

        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>System.String.</returns>
        [HiddenApi]
        public static string GetIP()
        {
            try
            {
                string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }

                if (string.IsNullOrEmpty(result))
                {
                    return "0.0.0.0";
                }

                return result;
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }

        /// <summary>
        /// Adds the base information.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="token">The token.</param>
        /// <param name="isAdd">if set to <c>true</c> [is add].</param>
        /// <param name="opt">The opt.</param>
        /// <returns></returns>
        [HiddenApi]
        public static Dictionary<string, object> AddBaseInfo<T>(Dictionary<string, object> dic, string token, bool isAdd, ref OpertionUser opt)
        {
            var user = UtilConst.GetLoginOpt(token);

            var propertys = typeof(T).GetProperties();
            if (propertys.Count(x => x.Name == "CreateTime") > 0 && isAdd)
            {
                dic.Add("CreateTime", DateTime.Now); ;
            }
            if (propertys.Count(x => x.Name == "CreateUserId") > 0 && isAdd)
            {
                dic.Add("CreateUserId", user.UserId);
            }
            if (propertys.Count(x => x.Name == "CreateUserName") > 0 && isAdd)
            {
                dic.Add("CreateUserName", user.UserName);
            }
            if (propertys.Count(x => x.Name == "UpdateUserId") > 0)
            {
                dic.Add("UpdateUserId", user.UserId);
            }
            if (propertys.Count(x => x.Name == "UpdateUserName") > 0)
            {
                dic.Add("UpdateUserName", user.UserName);
            }
            if (propertys.Count(x => x.Name == "UpdateTime") > 0)
            {
                dic.Add("UpdateTime", DateTime.Now);
            }
            opt.UserId = user.UserId;
            opt.UserName = user.UserName;
            opt.UserClientIp = GetIP();

            return dic;
        }
    }
}
