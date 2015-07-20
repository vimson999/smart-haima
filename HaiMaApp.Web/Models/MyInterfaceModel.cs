using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HaiMaApp.Web.Models
{
    public class TongZhiReturn
    {
        public string id { get; set; }
        public string title { get; set; }
        public string releasetime { get; set; }

    }

    public class TongZhiDetail
    {
        public string content { get; set; }
    }
}