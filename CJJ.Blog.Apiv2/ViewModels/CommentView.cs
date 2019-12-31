using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CJJ.Blog.Apiv2
{
    public class CommentView
    {
        public int KID { get; set; }
        public string ToMemberid { get; set; }
        public string MemberName { get; set; }
        public string Content { get; set; }
        public string BlogNum { get; set; }
        public string Commentid { get; set; }
        public string Token { get; set; }

    }
}