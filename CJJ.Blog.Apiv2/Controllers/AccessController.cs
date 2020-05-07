using Blog.Com.Helpers;
using CJJ.Blog.Apiv2.ViewModels;
using CJJ.Blog.NetWork.WcfHelper;
using FastDev.Common.Code;
using FastDev.Http;
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
    /// 访问者信息管理相关
    /// </summary>
    public class AccessController : BaseController
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="model">{""}</param>
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
                var retdata = BlogHelper.GetJsonListPage_Access(model.Page, model.Limit, view.OrderBy, view.Where);
                return FastJson(retdata.data, model.Token, retdata.code.Toint(), retdata != null ? "请求成功" : "请求失败", retdata != null ? retdata.count : 0);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AccessController/GetList");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }
    }
}
