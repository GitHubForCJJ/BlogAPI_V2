using CJJ.Blog.Apiv2.Models;
using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Model.Data;
using CJJ.Blog.Service.Models.View;
using FastDev.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Web.Http;
using FastDev.Common.Code;
using System.Threading.Tasks;
using CJJ.Blog.Service.Models.Data;
using Blog.Com.Helpers;
using System.Web.Caching;
using CJJ.Blog.Service.Model.View;

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
        public JsonResponse GetBlogComments([FromBody]BaseViewModel model)
        {
            try
            {
                if (model == null || model.Num.Length != 32)
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

        /// <summary>
        /// 添加一个评论
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse AddItem(CommentView model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Token))
                {
                    return new JsonResponse() { Code = 1, Msg = "请从新登录" };
                }

                var opt = new OpertionUser();
                var member = UtilConst.Memberinfo(model.Token);
                var comment = new Dictionary<string, object>()
                {
                    {nameof(Comment.Memberid),member.MemberModel.KID },
                    {nameof(Comment.MemberName),member.MemberModel.UserName },
                    {nameof(Comment.ToMemberid),model.ToMemberid},
                    {nameof(Comment.Content),model.Content},
                    {nameof(Comment.BlogNum),model.BlogNum},
                    {nameof(Comment.Avatar),member.MemberModel.UserIcon },
                };
                var sessionid = model.Commentid;
                if (!string.IsNullOrEmpty(model.Commentid))
                {
                    comment.Add(nameof(Comment.Commentid), model.Commentid);
                }
                else
                {
                    sessionid = Guid.NewGuid().ToString("N");
                    comment.Add(nameof(Comment.Commentid), sessionid);
                }
                var dic = UtilConst.AddBaseInfo<Comment>(comment, model.Token, true, ref opt);
                Result res = BlogHelper.Add_Comment(dic, opt);

                #region 处理list、item缓存问题

                Task.Run(() =>
                {
                    if (res.IsSucceed && model.ToMemberid.Toint()==0)
                    {
                        List<Bloginfo> bloglist = CacheHelper.GetCacheItem(ConfigUtil.BlogListCacheKey)?.ToString()?.DeserialObjectToList<Bloginfo>();
                        Bloginfo info = bloglist.FirstOrDefault(x => x.BlogNum == model.BlogNum);
                        if (bloglist != null && bloglist.Count > 0 && info != null && info.KID > 0)
                        {
                            bloglist.FirstOrDefault(x => x.BlogNum == model.BlogNum).Comments += 1;
                            CacheHelper.AddCacheItem(ConfigUtil.BlogListCacheKey, bloglist.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                        }
                        var blogview=CacheHelper.GetCacheItem($"{ConfigUtil.BlogItemCacheKeyPrefix}{model.BlogNum}")?.DeserialObject<BloginfoView>();
                        if (blogview != null && blogview.KID > 0)
                        {
                            blogview.Comments += 1;
                            CacheHelper.AddCacheItem($"{ConfigUtil.BlogItemCacheKeyPrefix}{model.BlogNum}", blogview.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                        }

                    }
                });

                #endregion

                return new JsonResponse() { Code = res.IsSucceed ? 0 : 1, Data = sessionid };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CommentController/additem");
                return new JsonResponse() { Code = 1, Msg = "程序视乎开小差了" + ex.Message };
            }
        }
    }
}
