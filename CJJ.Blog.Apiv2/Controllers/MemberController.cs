using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.Apiv2.ViewModels;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.Data;
using FastDev.Common.Code;
using FastDev.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    public class MemberController : ApiController
    {
        /// <summary>
        /// 会员注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse RegistItemMember([FromBody]UpdateView model)
        {
            try
            {
                if (model == null || model.Update == null || !model.Update.ContainsKey(nameof(Member.UserAccount)))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var qrkey = model.Update["QrcodeKey"].ToString();
                var qrcode = model.Update["Qrcode"].ToString();


                var qr = CacheHelper.GetCacheItem(qrkey)?.ToString() ?? "";
                if (qr != qrcode)
                {
                    return new JsonResponse { Code = 1, Msg = "验证码错误" + qr + "{" + qrkey };
                }
                var userAccount = model.Update[nameof(Member.UserAccount)].ToString();
                var mem = BlogHelper.GetModelByWhere_Member(new Dictionary<string, object>
                {
                    {nameof(Member.IsDeleted),0 },
                    {nameof(Member.UserAccount), userAccount}
                });
                if (mem != null || mem?.KID > 0)
                {
                    return new JsonResponse { Code = 1, Msg = "账户已存在,请直接登录" };
                }
                model.Update.Add("CreateTime", DateTime.Now);
                model.Update.Add("CreateUserId", 1);
                model.Update.Add(nameof(Member.CreateUserName), "system");
                var res = BlogHelper.Add_Member(model.Update, new Service.Models.View.OpertionUser());

                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Msg = res.Message };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "MemberController/RegistItemMember");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex };
            }
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse SendQrcode([FromBody]GetQrcodeView model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserAccount) || string.IsNullOrEmpty(model.QrcodeKey))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var rand = new Random();
                var qrcode = rand.Next(1000, 9999);
                CacheHelper.AddCacheItem(model.QrcodeKey, qrcode.ToString());
                return new JsonResponse { Code = 0, Msg = $"key是{model.QrcodeKey}二维码是{qrcode}" };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlog");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }

        }

        /// <summary>
        /// 会员登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse MemberLogin([FromBody]RegistModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserAccount) || string.IsNullOrEmpty(model.UserPassword))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }

                var res = BlogHelper.MemberLogin(model.UserAccount, model.UserPassword, "0", UtilConst.GetIP(), "", "");
                if (res != null)
                {
                    res.MemberInfo.UserPassword = "";
                }

                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Msg = res.Message, Data = res };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "MemberController/RegistItemMember");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex };
            }
        }
    }
}
