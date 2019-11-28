using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.ViewModels
{
    public class UpdateView
    {
        public int KID { get; set; }
        public Dictionary<string, object> Where { get; set; }
        public Dictionary<string, object> Update { get; set; }
        public string Num { get; set; }
    }
}