using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.ViewModels
{
    public class GetQrcodeView
    {
        public string UserAccount { get; set; }
        public string QrcodeKey { get; set; }
    }
}