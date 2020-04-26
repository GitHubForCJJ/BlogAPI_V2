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
    /// 后台管理 菜单相关接口
    /// </summary>
    /// <seealso cref="Go.Campuses.HttpApi.Controllers.BaseApiController" />
    /// <seealso cref="Go.Campuses.HttpApi.Controllers.BaseNoAuthApiController" />
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class MenuController : BaseController
    {
        #region 菜单管理相关
        /// <summary>
        ///菜单管理  获取整个菜单
        /// </summary>
        /// <param name="model">"空"</param>
        /// <returns>JsonResponse.</returns>
        [HttpPost]
        public JsonResponse GetAllMenuDataList([FromBody]JsonRequest model)
        {
            try
            {
                var user = UserInfoUtil.UserInfo(model.Token);

                List<Sysmenu> data = BlogHelper.GetListByuserid_Sysmenu(user.Model.KID);
                return FastJson(data, model.Token);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "/Menu/GetAllMenuDataList");
                return new JsonResponse() { Code = 1, Msg = $"请求失败：{ex.Message}" };
            }
        }

        /// <summary>
        /// 查询一个菜单信息
        /// </summary>
        /// <param name="model">{"kid":123}</param>
        /// <returns>JsonResponse.</returns>
        [HttpPost]
        public JsonResponse GetItemMenuByKid([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView ret = model.Data.ToString().DeserializeObject<UpdateView>();
                if (ret.KID <= 0)
                {
                    return FastJson("", model.Token, 1, "kid不能为空");
                }
                Sysmenu data = BlogHelper.GetModelByKID_Sysmenu(ret.KID);
                return FastJson(data, model.Token);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "/Menu/GetItemMenuByKid");
                return new JsonResponse() { Code = 1, Msg = $"请求失败：{ex.Message}" };
            }
        }


        /// <summary>
        ///删除一个菜单
        /// </summary>
        /// <param name="model">{"kid":123}</param>
        /// <returns>JsonResponse.</returns>
        [HttpPost]
        public JsonResponse DelItemMenuByKid([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView ret = model.Data.ToString().DeserializeObject<UpdateView>();
                if (ret.KID <= 0)
                {
                    return FastJson("", model.Token, 1, "kid不能为空");
                }
                SysLoginUser user = UserInfoUtil.UserInfo(model.Token);
                Result data = BlogHelper.Update_Sysmenu(new Dictionary<string, object>() { { nameof(Sysmenu.IsDeleted), 1 } }, ret.KID, new OpertionUser()
                {
                    UserId = user?.Model?.KID.ToString(),
                    UserName = user?.Model?.UserName,
                    UserClientIp = GetIP(),
                });
                return FastJson(data, model.Token, data.IsSucceed ? 0 : 1, data.Message);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "/Menu/DelItemMenuByKid");
                return new JsonResponse() { Code = 1, Msg = $"请求失败：{ex.Message}" };
            }
        }

        /// <summary>
        /// 新增一个菜单信息
        /// </summary>
        /// <param name="model">{"KID":0,"FatherID":0,"BusinessType":1,"MenuName":"首页","MenuIco":"ico","MenuUrl":"url","MenuMsg":"描述","MenuSort":0}</param>
        /// <returns>JsonResponse.</returns>
        [HttpPost]
        public JsonResponse AddItemMenu([FromBody]JsonRequest model)
        {
            try
            {
                SysLoginUser user = UserInfoUtil.UserInfo(model.Token);
                Sysmenu item = model.Data.ToString().DeserializeObject<Sysmenu>();
                item.CreateTime = DateTime.Now;
                item.UpdateTime = DateTime.Now;
                item.CreateUserId = user?.Model?.KID.ToString();
                item.CreateUserName = user?.Model?.UserName;
                item.UpdateUserId = item.CreateUserId;
                item.UpdateUserName = item.CreateUserName;
                var data = BlogHelper.AddByEntity_Sysmenu(item, new OpertionUser()
                {
                    UserId = user?.Model?.KID.ToString(),
                    UserName = user?.Model?.UserName,
                    UserClientIp = GetIP(),
                });
                return FastJson(data, model.Token, data.IsSucceed ? 0 : 1, data.Message);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "/Menu/DelItemMenuByKid");
                return new JsonResponse() { Code = 1, Msg = $"请求失败：{ex.Message}" };
            }
        }

        /// <summary>
        /// 修改一个菜单信息
        /// </summary>
        /// <param name="model">{"update":{"MenuName":"首页1","UpdateTime":"2019-04-26 15:01:16"},"kid":1}</param>
        /// <returns>JsonResponse.</returns>
        [HttpPost]
        public JsonResponse UpdateItemMenu([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView ret = model.Data.ToString().DeserializeObject<UpdateView>();
                if (ret.KID <= 0)
                {
                    return FastJson("", model.Token, 1, "kid不能为空");
                }
                SysLoginUser user = UserInfoUtil.UserInfo(model.Token);
                var data = BlogHelper.Update_Sysmenu(ret.Update, ret.KID, new OpertionUser()
                {
                    UserId = user?.Model?.KID.ToString(),
                    UserName = user?.Model?.UserName,
                    UserClientIp = GetIP(),
                });
                return FastJson(data, model.Token, data.IsSucceed ? 0 : 1, data.Message);

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "/Menu/DelItemMenuByKid");
                return new JsonResponse() { Code = 1, Msg = $"请求失败：{ex.Message}" };
            }
        }


        #endregion

    }
}
