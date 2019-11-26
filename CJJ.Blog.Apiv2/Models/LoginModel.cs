using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2.Models
{
    public class LoginModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 验证码key
        /// </summary>
        public string CodeKey { get; set; }
    }
}