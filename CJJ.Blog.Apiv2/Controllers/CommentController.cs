﻿using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.Data;
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
    /// 评论相关
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    public class CommentController : ApiController
    {
        /// <summary>
        /// 查询单个文章的评论
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetBlogComments(BaseViewModel model)
        {
            try
            {
                if (model==null||model.Num.Length != 32)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var dic = new Dictionary<string, object>()
                {
                    {nameof(Comment.IsDeleted),0 },
                    {nameof(Comment.States),0 },
                    {nameof(Comment.BlogNum), model.Num}
                };
                var alllist = BlogHelper.GetList_Comment(dic);

                return new JsonResponse { Code = 0, Data = alllist };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlog");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
    }
}
