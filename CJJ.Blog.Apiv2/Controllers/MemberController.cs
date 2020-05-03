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
using CJJ.Blog.Service.Models.View;
using Blog.Com.Helpers;
using System.Web;
using System.Web.Caching;
using Blog.Com.Helpers;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 会员相关
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class MemberController : ApiController
    {
        /// <summary>
        /// 会员注册
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse RegistItemMember([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView item = model.Data.ToString()?.DeserialObject<UpdateView>();
                if (item == null || item.Update == null || !item.Update.ContainsKey(nameof(Member.UserAccount)))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var qrkey = item.Update["Qrcodekey"].ToString();
                var qrcode = item.Update["Qrcode"].ToString();


                string qr = HttpRuntime.Cache.Get($"Blogimgcode_{qrkey}")?.ToString() ?? "";
                if (qr != qrcode)
                {
                    return new JsonResponse { Code = 1, Msg = "验证码错误" };
                }
                var userAccount = item.Update[nameof(Member.UserAccount)].ToString();
                var mem = BlogHelper.GetModelByWhere_Member(new Dictionary<string, object>
                {
                    {nameof(Member.IsDeleted),0 },
                    {nameof(Member.UserAccount), userAccount}
                });
                if (mem != null || mem?.KID > 0)
                {
                    return new JsonResponse { Code = 1, Msg = "账户已存在,请直接登录" };
                }
                item.Update.Add("CreateTime", DateTime.Now);
                item.Update.Add("CreateUserId", 1);
                item.Update.Add(nameof(Member.CreateUserName), "system");
                var res = BlogHelper.Add_Member(item.Update, new Service.Models.View.OpertionUser());

                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Msg = res.Message };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "MemberController/RegistItemMember");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex };
            }
        }
        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse SendQrcode([FromBody]JsonRequest model)
        {
            try
            {
                GetQrcodeView view = model.Data?.ToString()?.DeserialObject<GetQrcodeView>();
                if (string.IsNullOrEmpty(view.UserAccount) || string.IsNullOrEmpty(view.QrcodeKey))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var rand = new Random();
                string key = $"{view.UserAccount}_{view.QrcodeKey}";
                string checkcode = CacheHelper.GetCacheItem(key)?.ToString();
                if (!string.IsNullOrEmpty(checkcode))
                {
                    return new JsonResponse { Code = 0, Msg = $"验证码已存在{checkcode}" };
                }

                int qrcode = rand.Next(1000, 9999);
                CacheHelper.AddCacheItem(key, qrcode.ToString(), DateTime.Now.AddMinutes(30), Cache.NoSlidingExpiration, CacheItemPriority.High);
                return new JsonResponse { Code = 0, Msg = $"hello 验证码是{qrcode}" };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "MemberController/SendQrcode");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }

        }

        /// <summary>
        /// 会员登录
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse MemberLogin([FromBody]JsonRequest model)
        {
            try
            {
                RegistModel item = model.Data.ToString()?.DeserialObject<RegistModel>();

                if (string.IsNullOrEmpty(item.UserAccount) || string.IsNullOrEmpty(item.UserPassword))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }

                var res = BlogHelper.MemberLogin(item.UserAccount, item.UserPassword, "0", UtilConst.GetIP(), "", "");
                if (res != null && res.IsSucceed)
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
        /// <summary>
        /// 会员重置密码
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse ResetPsw([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView item = model?.Data.ToString().DeserialObject<UpdateView>();
                if (model == null || item.Update == null || !item.Update.ContainsKey(nameof(Member.UserAccount)))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var qrkey = item.Update["QrcodeKey"].ToString();
                var qrcode = item.Update["Qrcode"].ToString();
                var userAccount = item.Update[nameof(Member.UserAccount)].ToString();

                string key = $"{userAccount}_{qrkey}";
                var qr = CacheHelper.GetCacheItem(key)?.ToString() ?? "";
                if (qr != qrcode)
                {
                    return new JsonResponse { Code = 1, Msg = "验证码错误请重试" };
                }
                CacheHelper.DelCacheItem(key);

                var mem = BlogHelper.GetModelByWhere_Member(new Dictionary<string, object>
                {
                    {nameof(Member.IsDeleted),0 },
                    {nameof(Member.UserAccount), userAccount}
                });
                if (mem == null || mem?.KID <= 0)
                {
                    return new JsonResponse { Code = 1, Msg = "该账户不存在" };
                }
                item.Update.Add(nameof(Member.UpdateTime), DateTime.Now);
                item.Update.Add(nameof(Member.UpdateUserId), mem.UpdateUserId);
                item.Update.Add(nameof(Member.UpdateUserName), mem.UpdateUserName);
                var res = BlogHelper.Update_Member(item.Update, mem.KID, new OpertionUser());

                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Msg = res.Message };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "MemberController/ResetPsw");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex };
            }
        }
        /// <summary>
        /// 编辑会员 Num字段传会员useraccount
        /// </summary>
        /// <param name="model">{}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse UpdateMember([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView item = model?.Data.ToString().DeserialObject<UpdateView>();
                if (model == null || item.Update == null || string.IsNullOrEmpty(item.Num))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }

                item.Update.Add(nameof(Member.UpdateTime), DateTime.Now.Tostr());
                item.Update.Add(nameof(Member.UpdateUserId),1);
                item.Update.Add(nameof(Member.UpdateUserName), "会员修改昵称");
                var res = BlogHelper.UpdateByWhere_Member(item.Update, new Dictionary<string, object>() {
                    {nameof(Member.IsDeleted),0 },
                    { nameof(Member.UserAccount),item.Num}
                }, new OpertionUser());


                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Msg = res.Message };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "MemberController/UpdateMember");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex };
            }
        }
    }
}
