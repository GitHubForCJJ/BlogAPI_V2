using CJJ.Blog.NetWork.WcfHelper;
using CJJ.Blog.Service.Models.Data;
using FastDev.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    public class CategoryController : BaseController
    {
        /// <summary>
        /// 管理端获取所有的分类
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResponse GetListCartgory([FromBody]JsonRequest model)
        {
            try
            {
                var list = BlogHelper.GetList_Category(new Dictionary<string, object>() { { nameof(Category.States), 0 } });
                return FastResponse(list, model.Token, 0);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex, "CategoryController/GetListCartgory");
                return new JsonResponse { Code = 1, Msg = "程序视乎开小差了" };
            }
        }
    }
}
