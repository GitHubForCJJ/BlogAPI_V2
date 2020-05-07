using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.ViewModels
{
    /// <summary>
    /// api查询基类
    /// </summary>
    public class UpdateView
    {
        /// <summary>
        /// kid主键
        /// </summary>
        public int KID { get; set; }
        /// <summary>
        /// dic 条件
        /// </summary>
        public Dictionary<string, object> Where { get; set; }
        /// <summary>
        /// 更新
        /// </summary>
        public Dictionary<string, object> Update { get; set; }
        /// <summary>
        /// 博客编号  单个string
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string OrderBy { get; set; }
    }
}