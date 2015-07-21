using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HaiMaApp.Web.Models
{
    //public class TongZhi
    //{
    //    public int id { get; set; }
    //    public string title { get; set; }
    //    public string content { get; set; }
    //    public string Time { get; set; }
    //    public string sender { get; set; }
    //}

    public class ReturnCode
    {
        public string retid { get; set; }
    }

    public class JingXiaoShangTongXunLu
    {
        public string xiaoshoufuwudian { get; set; }
        public string zongjingli { get; set; }
        public string zongjinglimobile { get; set; }
        public string shichangjingli { get; set; }
        public string shichangjinglimobile { get; set; }
        public string xiaoshoujingli { get; set; }
        public string xiaoshoujinglimobile { get; set; }
    }

    public class JXSTongXunLu
    {
        public string dianming { get; set; }
        public string position { get; set; }
        public string renming { get; set; }
        public string dianhua { get; set; }
    }

    public class MyChuChaiRiZhi : ChuChaiRiZhi
    {
        public MyChuChaiRiZhi()
        {
            if (PiFuContentList != null)
                PiFuContentList = new List<string>();
        }
        public new string riqi { get; set; }
        public string PiFuCount { get; set; }
        public string newriqi { get; set; }
        public List<string> PiFuContentList { get; set; }


        public List<string> imgList { get; set; }
    }

    public class MyPaiZhao : TakePictureUpload
    {
        public string newriqi { get; set; }

        //modify by Lee 20150721 pifu
        public MyPaiZhao()
        {
            if (PiFuContentList != null)
                PiFuContentList = new List<string>();
        }
        public string PiFuCount { get; set; }
        public List<string> PiFuContentList { get; set; }
    }

    public class Tongxunlu
    {
        public string zhuanyinggongsimingcheng { get; set; }

        public string quyu { get; set; }

        public string xiaoshoufuwudian { get; set; }
    }

    public class Daqu
    {
        public string Name
        { get; set; }

        public string Code
        { get; set; }

        public bool Status
        { get; set; }

        public List<Tongxunlu> TongxunluList
        { get; set; }
    
    }




}