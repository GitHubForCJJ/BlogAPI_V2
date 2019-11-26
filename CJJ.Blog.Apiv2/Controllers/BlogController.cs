using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CJJ.Blog.Apiv2.Controllers
{
    public class BlogController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public void GetList_Blog([FromBody]JsonRequest model)
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
        }
    }
}
