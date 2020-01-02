using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.Apiv2.ViewModels;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.Data;
using CJJ.Blog.Service.Model.View;
using CJJ.Blog.Service.Models.Data;
using CJJ.Blog.Service.Models.View;
using FastDev.Common.Code;
using FastDev.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
                return new JsonResponse { Code = retlist.code.Toint(), Count = retlist.code.Toint() == 0 ? retlist.data.Count() : 0, Data = retlist.code.Toint() == 0 ? retlist.data : null };

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlog");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }

        /// <summary>
        /// 查询单个文章  传博客编号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetItemBlog([FromBody]BaseViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Num))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var retlist = BlogHelper.GetModelByNum(model.Num);
                if (retlist != null && retlist.Start == 0)
                {
                    retlist.Start = 1;
                }

                #region 异步添加访问次数

                Task.Run(() =>
                {
                    var dic = new Dictionary<string, object>()
                    {
                        {nameof(Bloginfo.BlogNum),model.Num },
                        {nameof(Bloginfo.IsDeleted),0 }
                    };
                    var updic = new Dictionary<string, object>()
                    {
                        {nameof(Bloginfo.Views),retlist.Views+1 },
                    };
                    BlogHelper.UpdateByWhere_Bloginfo(updic, dic, new Service.Models.View.OpertionUser());
                });

                #endregion

                return new JsonResponse { Code = retlist != null ? 0 : 1, Data = retlist != null ? retlist : null };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlog");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
        /// <summary>
        /// 根据文章编号  文章类别查询上下篇
        /// </summary>
        /// <param name="model">{"blogNum":"ajksdj","BlogType":1}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetPrenextBlog([FromBody]PrenextModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.BlogNum))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var dic = new Dictionary<string, object>
                {
                    {nameof(Bloginfo.IsDeleted),0 },
                    {nameof(Bloginfo.BlogNum),model.BlogNum }
                };
                var retlist = BlogHelper.GetPrenextBlog(model.BlogNum, model.BlogType);

                return new JsonResponse { Code = retlist.IsSucceed ? 0 : 1, Data = retlist };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetPrenext");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }

        /// <summary>
        /// 根据文章编号  文章类别查询上下篇
        /// </summary>
        /// <param name="model">{}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetListBlogTypes([FromBody]PrenextModel model)
        {
            try
            {
                var dic = new Dictionary<string, object>
                {
                    {nameof(Category.IsDeleted),0 },
                    {nameof(Category.States),0 }
                };
                var retlist = BlogHelper.GetList_Category(dic);
                retlist = retlist?.OrderByDescending(x => x.CreateTime)?.ToList();

                return new JsonResponse { Code = retlist != null ? 0 : 1, Data = retlist };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/GetListBlogTypes");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
        /// <summary>
        /// 文章点赞
        /// </summary>
        /// <param name="model">{}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse AddPraise([FromBody]CommentView model)
        {
            var res = new Result { IsSucceed = false };
            try
            {
                if (string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.BlogNum))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var mem = UtilConst.GetLoginOpt(model.Token);

                var dic = new Dictionary<string, object>
                {
                    {nameof(ArticlePraise.MemberId),mem.UserId },
                    {nameof(ArticlePraise.BlogNum),model.BlogNum }
                };
                var ap = BlogHelper.GetModelByWhere_ArticlePraise(dic);
                var opt = new OpertionUser();
                //取消点赞
                if (ap != null && ap?.IsDeleted == 0)
                {
                    dic.Add(nameof(ArticlePraise.IsDeleted), 1);
                    dic = UtilConst.AddBaseInfo<ArticlePraise>(dic, model.Token, false, ref opt);
                    res = BlogHelper.Update_ArticlePraise(dic, ap.KID, opt);
                }
                //点赞
                else
                {
                    dic = UtilConst.AddBaseInfo<ArticlePraise>(dic, model.Token, true, ref opt);
                    res = BlogHelper.Add_ArticlePraise(dic, opt);
                }

                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Data = res };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/AddPraise");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
        /// <summary>
        /// 查询是否点赞
        /// </summary>
        /// <param name="model">{}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse IsOrNotPraise([FromBody]UpdateView model)
        {
            try
            {
                if (model == null || model.Where == null)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                model.Where.Add(nameof(ArticlePraise.IsDeleted), 0);


                var ret = BlogHelper.GetModelByWhere_ArticlePraise(model.Where);

                return new JsonResponse { Code = 0, Data = ret?.KID > 0 };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/IsOrNotPraise");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
    }
}
