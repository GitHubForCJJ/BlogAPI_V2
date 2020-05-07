using Blog.Com.Helpers;
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
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 博客相关
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
                 string key = ConfigUtil.BlogListCacheKey;
                List<Bloginfo> alllist = CacheHelper.GetCacheItem(key)?.DeserialObjectToList<Bloginfo>();
                int cut = 0;
                if (alllist == null || alllist.Count == 0)
                {
                    var chaxun = BlogHelper.GetJsonListPage_Bloginfo(1, 1000, "CreateTime desc", new Dictionary<string, object>() {
                        {nameof(Bloginfo.IsDeleted),0 },
                        {nameof(Bloginfo.States),0 }
                    });
                    if (chaxun.code.Toint() != 0)
                    {
                        return new JsonResponse { Code = chaxun.code.Toint(), Data = "", Msg = chaxun.msg };
                    }
                    if (chaxun.data != null && chaxun.data.Count > 0)
                    {
                        alllist = chaxun.data;
                        CacheHelper.AddCacheItem(ConfigUtil.BlogListCacheKey, chaxun.data.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                    }
                }
                List<Bloginfo> retlist = null;
                if (model.KID > 0)
                {
                    retlist = alllist.Where(x => x.Type == model.KID)?.ToList();
                }
                else
                {
                    retlist = alllist;
                }
                cut = retlist.Count;
                retlist =retlist?.Skip((model.Page - 1) * model.Limit).Take(model.Limit).ToList();
          

                #region 统计访问地址信息
                Task.Run(() =>
                {
                    if (model.KID <= 0)
                    {
                        var adddic = new Dictionary<string, object>()
                    {
                        {nameof(Access.AccessType),0 },
                        {nameof(Access.IpAddress),GetIP() },
                        {nameof(Access.CreateTime),DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") },
                        {nameof(Access.CreateUserId),1 },
                        {nameof(Access.CreateUserName),"system" }
                    };
                        BlogHelper.Add_Access(adddic, new OpertionUser { });
                    }

                });
                #endregion

                return new JsonResponse { Code = 0, Data = retlist, Count = cut };

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
                string key = $"{CJJ.Blog.Apiv2.Models.ConfigUtil.BlogItemCacheKeyPrefix}{model.Num}";
                BloginfoView bloginfoView = CacheHelper.GetCacheItem(key)?.ToString()?.DeserialObject<BloginfoView>();
                if (bloginfoView == null)
                {
                    bloginfoView = BlogHelper.GetModelByNum(model.Num);
                    CacheHelper.AddCacheItem(key, bloginfoView.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                }
                bloginfoView.Views += 1;

                #region 处理list 和 item 缓存
                Task.Run(() =>
                {
                    //CacheHelper.DelCacheItem(key);
                    CacheHelper.AddCacheItem(key, bloginfoView.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                    string alllistkey = ConfigUtil.BlogListCacheKey;
                    string allinfo = CacheHelper.GetCacheItem(alllistkey)?.ToString();
                    List<Bloginfo> cachelist = allinfo?.DeserialObjectToList<Bloginfo>();
                    Bloginfo info = cachelist.FirstOrDefault(x => x.BlogNum == model.Num);

                    if (cachelist != null && cachelist.Count > 0&& info!=null &&info.KID>0)
                    {
                        cachelist.FirstOrDefault(x => x.BlogNum == model.Num).Views += 1;
                        //CacheHelper.DelCacheItem(alllistkey);
                        CacheHelper.AddCacheItem(alllistkey, cachelist.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                    }

                });
                #endregion


                #region 异步添加访问次数,统计访问地址信息

                Task.Run(() =>
                {
                    try
                    {
                        var dic = new Dictionary<string, object>()
                    {
                        {nameof(Bloginfo.BlogNum),model.Num },
                        {nameof(Bloginfo.IsDeleted),0 }
                    };
                        var bloginfomodel = BlogHelper.GetModelByWhere_Bloginfo(dic);
                        var updic = new Dictionary<string, object>()
                    {
                        {nameof(Bloginfo.Views),bloginfomodel.Views+1 },
                    };
                        BlogHelper.UpdateByWhere_Bloginfo(updic, dic, new Service.Models.View.OpertionUser());

                        var adddic = new Dictionary<string, object>()
                    {
                        {nameof(Access.AccessType),1 },
                        {nameof(Access.IpAddress),GetIP() },
                        {nameof(Access.BlogName), bloginfoView.Title},
                        {nameof(Access.BlogNum),model.Num },
                        {nameof(Access.CreateTime),DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") },
                            {nameof(Access.CreateUserId),1 },
                            {nameof(Access.CreateUserName),"system" }
                    };
                        BlogHelper.Add_Access(adddic, new OpertionUser { });
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(ex, "BlogController/GetItemBlog 访问次数");
                    }

                });

                #endregion

                return new JsonResponse { Code = bloginfoView != null ? 0 : 1, Data = bloginfoView != null ? bloginfoView : null };
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
                string key = ConfigUtil.BlogTypeListCacheKey;
                List<Category> retlist = CacheHelper.GetCacheItem(key)?.DeserialObjectToList<Category>();
                if (retlist == null || retlist.Count == 0)
                {
                    retlist = BlogHelper.GetList_Category(dic);
                    CacheHelper.AddCacheItem(key, retlist.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                }

                retlist = retlist?.OrderByDescending(x => x.CreateTime)?.OrderByDescending(x => x.Sort)?.ToList();

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
        public JsonResponse AddPraise([FromBody]JsonRequest model)
        {
            var res = new Result { IsSucceed = false };
            try
            {
                CommentView item = model?.Data.ToString()?.DeserialObject<CommentView>();
                if (item == null || string.IsNullOrEmpty(item.BlogNum))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var mem = UtilConst.GetLoginOpt(model.Token);

                var dic = new Dictionary<string, object>
                {
                    {nameof(ArticlePraise.MemberId),mem.UserId },
                    {nameof(ArticlePraise.BlogNum),item.BlogNum },
                    {nameof(ArticlePraise.IsDeleted),0 }
                };
                var dodic = new Dictionary<string, object>
                {
                    {nameof(ArticlePraise.MemberId),mem.UserId },
                    {nameof(ArticlePraise.BlogNum),item.BlogNum },
                };
                var ap = BlogHelper.GetModelByWhere_ArticlePraise(dodic);
                var opt = new OpertionUser();
                bool isadd = true;//记录点赞还是取消点赞

                #region 点赞处理
                //取消点赞
                if (ap != null && ap.KID > 0)
                {
                    isadd = false;
                    dodic.Add(nameof(ArticlePraise.IsDeleted), 1);
                    dodic = UtilConst.AddBaseInfo<ArticlePraise>(dodic, model.Token, false, ref opt);
                    res = BlogHelper.Update_ArticlePraise(dodic, ap.KID, opt);

                }
                //点赞
                else
                {
                    dodic.Add(nameof(ArticlePraise.IpAddress), GetIP());
                    dodic = UtilConst.AddBaseInfo<ArticlePraise>(dodic, model.Token, true, ref opt);
                    res = BlogHelper.Add_ArticlePraise(dodic, opt);
                }
                #endregion

                #region 处理list 和 item 缓存
                Task.Run(() =>
                {
                    if (res.IsSucceed)
                    {
                        string key = $"{CJJ.Blog.Apiv2.Models.ConfigUtil.BlogItemCacheKeyPrefix}{item.BlogNum}";
                        BloginfoView bloginfoView = CacheHelper.GetCacheItem(key)?.ToString()?.DeserialObject<BloginfoView>();
                        bloginfoView.Start += isadd ? 1 : -1;
                        CacheHelper.AddCacheItem(key, bloginfoView.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                        string alllistkey = ConfigUtil.BlogListCacheKey;
                        string allinfo =CacheHelper.GetCacheItem(alllistkey)?.ToString();
                        List<Bloginfo> cachelist = allinfo?.DeserialObjectToList<Bloginfo>();
                        Bloginfo info = cachelist.FirstOrDefault(x => x.BlogNum == item.BlogNum);
                        if (cachelist != null && cachelist.Count > 0 && info!=null && info.KID>0)
                        {
                            cachelist.FirstOrDefault(x => x.BlogNum == item.BlogNum).Start += isadd ? 1 : -1;
                            CacheHelper.AddCacheItem(alllistkey, cachelist.SerializObject(), DateTime.Now.AddDays(2), Cache.NoSlidingExpiration, CacheItemPriority.High);
                        }
                    }

                });
                #endregion

                return new JsonResponse { Code = res.IsSucceed ? 0 : 1, Data = res };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/AddPraise");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetIP()
        {
            try
            {
                string result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = HttpContext.Current.Request.UserHostAddress;
                }

                if (string.IsNullOrEmpty(result))
                {
                    return "0.0.0.0";
                }

                return result;
            }
            catch (Exception)
            {
                return "0.0.0.1";
            }
        }
        /// <summary>
        /// 查询是否点赞
        /// </summary>
        /// <param name="model">{"where":{"BlogNum":""}}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse IsOrNotPraise([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView item = model?.Data?.ToString().DeserialObject<UpdateView>();
                if (item == null || item.Where == null)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                item.Where.Add(nameof(ArticlePraise.IsDeleted), 0);
                string blognum = item.Where["BlogNum"].ToString();

                int count = BlogHelper.GetCount_ArticlePraise(new Dictionary<string, object>() {
                    {nameof(ArticlePraise.BlogNum),blognum },
                    {nameof(ArticlePraise.IsDeleted),0 }
                });

                var ret = BlogHelper.GetModelByWhere_ArticlePraise(item.Where);

                return new JsonResponse { Code = 0, Data = new{ ArticlePraise= ret?.KID > 0 ,Count=count} };
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "BlogController/IsOrNotPraise");
                return new JsonResponse { Code = 1, Msg = "程序好像开小差了" + ex.Message };
            }
        }
    }
}
