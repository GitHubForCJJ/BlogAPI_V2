using Blog.Com.Helpers;
using CJJ.Blog.Apiv2.ViewModels;
using CJJ.Blog.NetWork.WcfHelper;
using FastDev.Common.Code;
using FastDev.Log;
using System;
using System.Linq;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 评论管理
    /// </summary>
    public class AdminCommentController : BaseController
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
                var retdata = BlogHelper.GetJsonListPage_Comment(model.Page, model.Limit, view.OrderBy, view.Where);
                return FastJson(retdata.data, model.Token, retdata.code.Toint(), retdata != null ? "请求成功" : "请求失败", retdata != null ? retdata.count : 0);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminCommentController/GetList");
                return new JsonResponse { Code = 1, Msg = "程序错误" + ex.Message };
            }
        }
    }
}

