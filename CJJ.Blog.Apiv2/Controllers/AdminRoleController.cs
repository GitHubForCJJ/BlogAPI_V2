using Blog.Com.Helpers;
using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.Apiv2.ViewModels;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.View;
using CJJ.Blog.Service.Models.Data;
using CJJ.Blog.Service.Models.View;
using FastDev.Common.Code;
using FastDev.Log;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 角色相关
    /// </summary>
    public class AdminRoleController : BaseController
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="model">{"where":{"RoleName|l":"员工"}}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetList([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView view = model.Data.DeserialObject<UpdateView>();
                if (view == null || view.Where == null)
                {
                    return new JsonResponse { Code = 1, Msg = "参数错误" };
                }

                if (string.IsNullOrEmpty(view.OrderBy))
                {
                    view.OrderBy = "CreateTime desc";
                }
                var retdata = BlogHelper.GetJsonListPage_Sysrole(model.Page, model.Limit, view.OrderBy, view.Where);
                return FastJson(retdata.data, model.Token, retdata.code.Toint(), retdata != null ? "请求成功" : "请求失败", retdata != null ? retdata.count : 0);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminRoleController/GetList");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }
        /// <summary>
        /// 获取单个
        /// </summary>
        /// <param name="model">{"kid":1}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetItem([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView view = model.Data.DeserialObject<UpdateView>();
                if (view == null || view.KID <= 0)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }

                Sysrole retdata = BlogHelper.GetModelByKID_Sysrole(view.KID);
                return FastJson(retdata, model.Token);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminRoleController/GetItem");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }
        /// <summary>
        /// 编辑单个
        /// </summary>
        /// <param name="model">{"kid":1}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse UpdateItem([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView view = model.Data.DeserialObject<UpdateView>();
                if (view == null || view.KID <= 0 || view.Update == null)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                SysLoginUser user = UserInfoUtil.UserInfo(model.Token);
                if (!user.IsAdmin)
                {
                    return new JsonResponse { Code = 1, Msg = "暂无操作权限" };
                }
                OpertionUser opt = new OpertionUser();
                view.Update = AddBaseInfo<Sysrole>(view.Update, model.Token, false, ref opt);

                Result res = BlogHelper.Update_Sysrole(view.Update, view.KID, opt);
                return FastJson(res, model.Token, res.IsSucceed ? 0 : 1, res.IsSucceed ? "操作成功" : "操作失败");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminRoleController/UpdateItem");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }

        /// <summary>
        /// 删除单个
        /// </summary>
        /// <param name="model">{"kid":1}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse DeleteItem([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView view = model.Data.DeserialObject<UpdateView>();
                if (view == null || view.KID <= 0)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                view.Update = new Dictionary<string, object>();
                view.Update.Add(nameof(Sysrole.IsDeleted), 0);
                OpertionUser opt = new OpertionUser();
                view.Update = AddBaseInfo<Sysrole>(view.Update, model.Token, false, ref opt);
                Result res = BlogHelper.Update_Sysrole(view.Update,view.KID,opt);
                return FastJson(res, model.Token, res.IsSucceed ? 0 : 1, res.IsSucceed ? "操作成功" : "操作失败");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminRoleController/DeleteItem");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }

        
    }
}
