using CJJ.Blog.Apiv2.ViewModels;
using FastDev.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FastDev.Common;
using FastDev.Common.Extension;
using CJJ.Blog.NetWork.WcfHelper;
using FastDev.Common.Code;
using CJJ.Blog.Service.Models.Data;
using CJJ.Blog.Service.Models.View;

namespace CJJ.Blog.Apiv2.Controllers
{
    /// <summary>
    /// 管理端博客相关  需验签
    /// </summary>
    public class AdminBlogController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetListBlog([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView up = model?.Data?.ToString().DeserializeObject<UpdateView>();
                if (up == null)
                {
                    up = new UpdateView();
                    up.Where = new Dictionary<string, object>();
                }
                if (up.Where == null)
                {
                    up.Where = new Dictionary<string, object>();
                }
                var list = BlogHelper.GetJsonListPage_Bloginfo(model.Page, model.Limit, "CreateTime desc", up.Where);
                return FastResponse(list.data, model.Token, list.code.Toint());

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminBlogController/GetList_Blog");
                return new JsonResponse { Code = 1, Msg = "程序视乎开小差" + ex.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">{num:"96fcdfabc9b0445dab10f2971bf4b127 博客编号"}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetItemBlog([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView up = model?.Data?.ToString().DeserializeObject<UpdateView>();
                if (up == null || string.IsNullOrEmpty(up.Num))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }

                var entity = BlogHelper.GetModelByNum(up.Num);
                if (entity == null)
                {
                    return new JsonResponse { Code = 1, Msg = $"暂未查询到{up.KID}该信息" };
                }

                return FastResponse(entity, model.Token);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminBlogController/GetList_Blog");
                return new JsonResponse { Code = 1, Msg = "程序视乎开小差" + ex.Message };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">{update:{}}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse AddItemBlog([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView up = model?.Data?.ToString().DeserializeObject<UpdateView>();
                if (up == null || up.Update == null)
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var opt = new OpertionUser();
                var content = up.Update[nameof(Blogcontent.Content)].ToString();
                var blognum = Guid.NewGuid().ToString().Replace("-", "");

                var infodic = AddBaseInfo<Bloginfo>(up.Update, model.Token, true, ref opt);
                infodic.Add(nameof(Bloginfo.BlogNum), blognum);
                var contdic = AddBaseInfo<Blogcontent>(up.Update, model.Token, true, ref opt);
                contdic.Add(nameof(Blogcontent.BloginfoNum), blognum);

                var res1 = BlogHelper.Add_Bloginfo(infodic, opt);
                var res2 = BlogHelper.Add_Blogcontent(contdic, opt);
                if (!res1.IsSucceed || !res2.IsSucceed)
                {
                    return new JsonResponse { Code = 1, Msg = $"添加失败{res1.SerializeObject()};{res2.SerializeObject()}" };
                }
                return FastResponse("", model.Token, 0, "添加成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminBlogController/GetList_Blog");
                return new JsonResponse { Code = 1, Msg = "程序视乎开小差" + ex.Message };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">{update:{},"num":"96fcdfabc9b0445dab10f2971bf4b127 博客编号"}</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse UpdateItemBlog([FromBody]JsonRequest model)
        {
            try
            {
                UpdateView up = model?.Data?.ToString().DeserializeObject<UpdateView>();
                if (up == null || up.Update == null || string.IsNullOrEmpty(up.Num))
                {
                    return new JsonResponse { Code = 1, Msg = "参数不合法" };
                }
                var opt = new OpertionUser();
                var content = up.Update[nameof(Blogcontent.Content)].ToString();

                var infodic = AddBaseInfo<Bloginfo>(up.Update, model.Token, false, ref opt);

                var contdic = AddBaseInfo<Blogcontent>(up.Update, model.Token, false, ref opt);
                var dicwhere = new Dictionary<string, object>()
                {
                    {nameof(Bloginfo.BlogNum),up.Num }
                };
                var dicwhere2 = new Dictionary<string, object>()
                {
                    {nameof(Blogcontent.BloginfoNum),up.Num }
                };

                var res1 = BlogHelper.UpdateByWhere_Bloginfo(infodic, dicwhere, opt);
                var res2 = BlogHelper.UpdateByWhere_Blogcontent(contdic,dicwhere2, opt);
                if (!res1.IsSucceed || !res2.IsSucceed)
                {
                    return new JsonResponse { Code = 1, Msg = $"编辑失败{res1.SerializeObject()};{res2.SerializeObject()}" };
                }
                return FastResponse("", model.Token, 0, "编辑成功");
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "AdminBlogController/UpdateItemBlog");
                return new JsonResponse { Code = 1, Msg = "程序视乎开小差" + ex.Message };
            }
        }
    }
}
