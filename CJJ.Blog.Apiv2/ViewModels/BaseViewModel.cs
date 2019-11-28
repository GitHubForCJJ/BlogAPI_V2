using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2
{
    public class BaseViewModel
    {
        public int KID { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}