using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.View;
using CJJ.Blog.Service.Models.Data;
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
    public class BlogController : ApiController
    {
        /// <summary>
        /// h5不加密获取list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetListBlog([FromBody]BaseViewModel model)
        {
            try
            {
                var dicwhere = new Dictionary<string, object>() {
                        {nameof(Bloginfo.IsDeleted),0 },
                                    {nameof(Bloginfo.States),0 },
                    };
                if (model.KID > 0)
                {
                    dicwhere.Add(nameof(Bloginfo.Type), model.KID);
                }
                var retlist = BlogHelper.GetJsonListPage_Bloginfo(model.Page, model.Limit, "CreateTime desc", dicwhere);
                return new JsonResponse { Code = retlist.code.Toint(), Data = retlist.code.Toint() == 0 ? retlist.data : null };

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlog");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }

        /// <summary>
        /// 查询单个文章
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetItemBlog([FromBody]BaseViewModel model)
        {
            try
            {
                if (model.KID < 0)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var retlist = BlogHelper.GetModelByKID_Bloginfo(model.KID);

                return new JsonResponse { Code = retlist != null ? 0 : 1, Data = retlist != null ? retlist : null };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlog");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
    }
}
