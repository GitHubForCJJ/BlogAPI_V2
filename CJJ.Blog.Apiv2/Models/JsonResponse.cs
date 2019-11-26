using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2
{
    public class JsonResponse
    {
        /// <summary>
        /// 0成功 其它失败
        /// </summary>
        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}