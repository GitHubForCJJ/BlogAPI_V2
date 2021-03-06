﻿using Blog.Com.Helpers;
using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.Apiv2.ViewModels;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.View;
using CJJ.Blog.Service.Models.Data;
using CJJ.Blog.Service.Models.View;
using FastDev.Common.Code;
using FastDev.Common.Extension;
using FastDev.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 分类相关
    /// </summary>
    /// <seealso cref="CJJ.Blog.Apiv2.Controllers.BaseController" />
    public class CategoryController : BaseController
    {
        /// <summary>
        /// 管理端 分页获取所有的分类{"where":{}}
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetList([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView view = model.Data.ToString().DeserializeObject<UpdateView>();
                if (view == null || view.Where == null)
                {
                    view = new UpdateView();
                    view.Where = new Dictionary<string, object>();
                }
                view.Where.Add(nameof(Category.IsDeleted), 0);
                if (string.IsNullOrEmpty(view.OrderBy))
                {
                    view.OrderBy = "CreateTime desc";
                }
                var list = BlogHelper.GetJsonListPage_Category(model.Page, model.Limit, view.OrderBy, view.Where);
                return FastResponse(list.data, model.Token, list.code.Toint(), list.code.Toint() == 0 ? list.count : 0);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CategoryController/GetListCartgory");
                return new JsonResponse { Code = 1, Msg = "程序视乎开小差了" };
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

                Category retdata = BlogHelper.GetModelByKID_Category(view.KID);
                return FastJson(retdata, model.Token);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CategoryController/GetItem");
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
                view.Update = AddBaseInfo<Category>(view.Update, model.Token, false, ref opt);

                Result res = BlogHelper.Update_Category(view.Update, view.KID, opt);
                return FastJson(res, model.Token, res.IsSucceed ? 0 : 1, res.IsSucceed ? "操作成功" : "操作失败");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CategoryController/UpdateItem");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }

        /// <summary>
        /// 添加单个
        /// </summary>
        /// <param name="model">{"update":{"Name":""}}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse AddItem([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView view = model.Data.DeserialObject<UpdateView>();
                if (view == null || view.Update == null)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                SysLoginUser user = UserInfoUtil.UserInfo(model.Token);
                if (!user.IsAdmin)
                {
                    return new JsonResponse { Code = 1, Msg = "暂无操作权限" };
                }
                OpertionUser opt = new OpertionUser();
                view.Update = AddBaseInfo<Category>(view.Update, model.Token, true, ref opt);

                Result res = BlogHelper.Add_Category(view.Update, opt);
                return FastJson(res, model.Token, res.IsSucceed ? 0 : 1, res.IsSucceed ? "操作成功" : "操作失败");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CategoryController/UpdateItem");
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
                SysLoginUser user = UserInfoUtil.UserInfo(model.Token);
                if (!user.IsAdmin)
                {
                    return new JsonResponse { Code = 1, Msg = "暂无操作权限" };
                }
                view.Update = new Dictionary<string, object>();
                view.Update.Add(nameof(Category.IsDeleted), 1);
                OpertionUser opt = new OpertionUser();
                view.Update = AddBaseInfo<Category>(view.Update, model.Token, false,user.Model, ref opt);
                Result res = BlogHelper.Update_Category(view.Update, view.KID, opt);
                return FastJson(res, model.Token, res.IsSucceed ? 0 : 1, res.IsSucceed ? "操作成功" : "操作失败");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CategoryController/DeleteItem");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }
    }
}
