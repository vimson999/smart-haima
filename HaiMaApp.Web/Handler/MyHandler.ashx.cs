using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using HaiMaApp.Web.Models;
using NSTool.UMengPush.Core;
using NSTool.UMengPush;

namespace HaiMaApp.Web.Hanlder
{
    /// <summary>
    /// MyHandler 的摘要说明
    /// </summary>
    public class MyHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var action = context.Request["opertype"] == null ? "" :
                      context.Request["opertype"].ToString();

            switch (action.ToLower())
            {
                case "getjingxiaoshangtongxunlulist":
                    getjingxiaoshangtongxunlulist(context);
                    break;
                case "addnewtongxunlu":
                    addnewtongxunlu(context);
                    break;
                case "updatetongxunlu":
                    updatetongxunlu(context);
                    break;
                case "deletetongxunlu":
                    deletetongxunlu(context);
                    break;
                case "gettongzhilist":
                    gettongzhilist(context);
                    break;
                case "gettongzhilistnew":
                    gettongzhilistnew(context);
                    break;
                case "takepictureupload_data":
                    takepictureupload_data(context);
                    break;
                case "uploadimage":
                    uploadimage(context);
                    break;
                //case "getdudaotongxunlulist":
                //    getdudaotongxunlulist(context);
                //    break;
                case "getdianminglistofthisdudao":
                    getdianminglistofthisdudao(context);
                    break;
                case "chuchairizhi_data":
                    chuchairizhi_data(context);
                    break;
                case "chuchairizhi_image":
                    chuchairizhi_image(context);
                    break;
                case "chuchairizhi_multiimage":
                    chuchairizhi_multiimage(context);
                    break;
                case "gettongzhidetail":
                    gettongzhidetail(context);
                    break;
                case "getmychuchailist":
                    getmychuchailist(context);
                    break;
                case "getmypaizhaolist":
                    getmypaizhaolist(context);
                    break;
                case "logincheck":
                    logincheck(context);
                    break;

                //interface for xuyan 
                case "createttakephoto":
                    createttakephoto(context);
                    break;
                case "businesstravel":
                    businesstravel(context);
                    break;
                case "getstaffpositionlist":            //getstaffpositionlist by Lee 20150712 页面用，非对外接口
                    getStaffPositionList(context);
                    break;
                case "getstafflist":                    //getstafflist by Lee 20150712 页面用，非对外接口
                    getStaffList(context);
                    break;
                case "replySuperviserPhotosUpload":      //replySuperviserPhotosUpload by Lee 20150721 
                    replySuperviserPhotosUpload(context);
                    break;
                case "getarealist":                    //getAreaList by vim 20150719 
                    getAreaList(context);
                    break;
                case "updatepassword":                    //updatepassword by vim 20150719 
                    updatePassword(context);
                    break;
                case "testtuisong":
                    testTui(context);
                    break;
                default:
                    break;
            }
        }

        #region 批复照片上传 by Lee 20150721
        private void replySuperviserPhotosUpload(HttpContext context)
        {
            var id = QueryString.GetQuery(context, "id");
            var content = QueryString.GetQuery(context, "content");

            var commandText = string.Format("INSERT INTO PiFuOnPictureUpload(PictureUploadID,PiFuContent) VALUES({0},'{1}') ", id, context);
            var result = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, commandText);
            string returnMsg = "";
            if (Int32.Parse(result.ToString()) > 0)
            {
                returnMsg = "success";
            }
            else
            {
                returnMsg = "fail";
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(returnMsg);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }
        #endregion

        private void logincheck(HttpContext context)
        {
            var mobile = QueryString.GetQuery(context, "mobile");
            var password = QueryString.GetQuery(context, "password");

            var result = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, string.Format("select COUNT(*) as totalcount from DuDaoRights where mobile='{0}' and Password='{1}' and HasRights=1", mobile, password));
            string returnMsg = "";
            if (Int32.Parse(result.ToString()) > 0)
            {
                returnMsg = "success";
            }
            else
            {
                returnMsg = "fail";
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(returnMsg);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }

        private void getmychuchailist(HttpContext context)
        {
            var phone = QueryString.GetQuery(context, "phone");

            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                string.Format("select top 100 * from ChuChaiRiZhi a left join PiFuOnChuChai b on a.id=b.chuchaiid where a.uploaderphone='{0}' order by riqi desc", phone));
            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<MyChuChaiRiZhi> lists = new List<MyChuChaiRiZhi>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var tempid = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString());
                    var obj = lists.FirstOrDefault(item => item.id == tempid);
                    if (obj != null)
                    {
                        var pifucontent = datasource.Tables[0].Rows[i]["PiFuContent"] != null
                            ? datasource.Tables[0].Rows[i]["PiFuContent"].ToString()
                            : "";
                        obj.PiFuContentList.Add(pifucontent);
                    }
                    else
                    {
                        var imageSource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                                            string.Format("select top 100 * from [ChuChaiImages] where chuchai_id='{0}' order by id desc", Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString())));

                        var imageList = new List<string>();
                        if (imageSource != null && imageSource.Tables.Count > 0)
                        {
                            for (var j = 0; j < imageSource.Tables[0].Rows.Count; j++)
                            {
                                imageList.Add(imageSource.Tables[0].Rows[j]["pic_name"].ToString());
                            }
                        }

                        lists.Add(new MyChuChaiRiZhi()
                        {
                            id = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString()),
                            riqi = datasource.Tables[0].Rows[i]["riqi"].ToString(),
                            newriqi = DateTime.Parse(datasource.Tables[0].Rows[i]["riqi"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                            position = datasource.Tables[0].Rows[i]["position"].ToString(),
                            miaoshu = datasource.Tables[0].Rows[i]["miaoshu"].ToString(),
                            zhenggai = datasource.Tables[0].Rows[i]["zhenggai"].ToString(),
                            xietiao = datasource.Tables[0].Rows[i]["xietiao"].ToString(),
                            dianming = datasource.Tables[0].Rows[i]["dianming"] != null ? datasource.Tables[0].Rows[i]["dianming"].ToString() : "",
                            PiFuContentList = new List<string>()
                            {
                                datasource.Tables[0].Rows[i]["PiFuContent"] != null ? 
                                datasource.Tables[0].Rows[i]["PiFuContent"].ToString() : ""
                            },
                            imgList = imageList
                        });
                    }
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }
        }
        
        //modify by Lee 20150721 添加批复内容
        private void getmypaizhaolist(HttpContext context)
        {
            var phone = QueryString.GetQuery(context, "phone");

            var commandText = string.Format("select top 100 * from TakePictureUpload a left join PiFuOnPictureUpload b on a.id=b.PictureUploadID where a.uploaderphone='{0}' order by riqi desc", phone);
            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, commandText);
            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<MyPaiZhao> lists = new List<MyPaiZhao>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var tempid = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString());
                    var obj = lists.FirstOrDefault(item => item.id == tempid);
                    if (obj != null)
                    {
                        var pifucontent = datasource.Tables[0].Rows[i]["PiFuContent"] != null ? 
                                          datasource.Tables[0].Rows[i]["PiFuContent"].ToString() : "";
                        obj.PiFuContentList.Add(pifucontent);
                    }
                    else
                    {
                        lists.Add(new MyPaiZhao()
                        {
                            id = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString()),
                            newriqi = DateTime.Parse(datasource.Tables[0].Rows[i]["riqi"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                            mainproject = datasource.Tables[0].Rows[i]["mainproject"].ToString(),
                            childproject = datasource.Tables[0].Rows[i]["childproject"].ToString(),
                            position = datasource.Tables[0].Rows[i]["position"].ToString(),
                            description = datasource.Tables[0].Rows[i]["description"].ToString(),
                            dianming = datasource.Tables[0].Rows[i]["dianming"] != null ? datasource.Tables[0].Rows[i]["dianming"].ToString() : "",
                            picname = datasource.Tables[0].Rows[i]["picname"].ToString(),
                            PiFuContentList = new List<string>()
                            {
                                datasource.Tables[0].Rows[i]["PiFuContent"] != null
                            ? datasource.Tables[0].Rows[i]["PiFuContent"].ToString()
                            : ""
                            }

                        });
                    }

                    // old code
                    //lists.Add(new MyPaiZhao()
                    //{
                    //    id = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString()),
                    //    newriqi = DateTime.Parse(datasource.Tables[0].Rows[i]["riqi"].ToString()).ToString("yyyy-MM-dd HH:mm:ss"),
                    //    mainproject = datasource.Tables[0].Rows[i]["mainproject"].ToString(),
                    //    childproject = datasource.Tables[0].Rows[i]["childproject"].ToString(),
                    //    position = datasource.Tables[0].Rows[i]["position"].ToString(),
                    //    description = datasource.Tables[0].Rows[i]["description"].ToString(),
                    //    dianming = datasource.Tables[0].Rows[i]["dianming"] != null ? datasource.Tables[0].Rows[i]["dianming"].ToString() : "",
                    //    picname = datasource.Tables[0].Rows[i]["picname"].ToString()

                    //});
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }
        }
        private void getmychuchaidetail(HttpContext context)
        {
            //var id = QueryString.GetQuery(context, "id");
            ////var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            //var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "  select content from tongzhi where id=" + id);
            //if (datasource != null && datasource.Tables.Count > 0)
            //{

            //    var tongzhidetail = new TongZhiDetail();
            //    tongzhidetail.content = datasource.Tables[0].Rows[0]["content"].ToString();

            //    JavaScriptSerializer serializer = new JavaScriptSerializer();
            //    var str = serializer.Serialize(tongzhidetail);
            //    context.Response.ContentType = "text/json;charset=UTF-8;";
            //    context.Response.Write(str);
            //}
        }

        private void gettongzhilist(HttpContext context)
        {
            //var pageIndex = QueryString.GetQuery(context, "pageIndex");
            //var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            var commandText = "  SELECT TOP 10 * FROM tongzhi WHERE IsPublish = 1 ORDER BY [time] DESC ";  //modify by Lee 20150720 ispublish = 1
            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, commandText);
            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<TongZhiReturn> lists = new List<TongZhiReturn>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    lists.Add(new TongZhiReturn()
                    {

                        id = datasource.Tables[0].Rows[i]["id"].ToString(),
                        title = datasource.Tables[0].Rows[i]["title"].ToString(),
                        releasetime = datasource.Tables[0].Rows[i]["time"].ToString()
                    });
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }


        }

        /// <summary>
        /// 获取新的通知，更新的通知！！！
        /// </summary>
        private void gettongzhilistnew(HttpContext context)
        {
            var maxid = QueryString.GetQuery(context, "maxid");
            //var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            var commandText = "SELECT id, title,time FROM tongzhi WHERE id>" + maxid + " AND IsPublish = 1 ORDER BY [time] DESC";   //modify by Lee 20150720 ispublish = 1
            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, commandText);
            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<TongZhiReturn> lists = new List<TongZhiReturn>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    lists.Add(new TongZhiReturn()
                    {
                        id = datasource.Tables[0].Rows[i]["id"].ToString(),
                        title = datasource.Tables[0].Rows[i]["title"].ToString(),
                        releasetime = datasource.Tables[0].Rows[i]["time"].ToString()
                    });
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }


        }

        private void gettongzhidetail(HttpContext context)
        {
            var id = QueryString.GetQuery(context, "id");
            //var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "  select content from tongzhi where id=" + id);
            if (datasource != null && datasource.Tables.Count > 0)
            {

                var tongzhidetail = new TongZhiDetail();
                tongzhidetail.content = datasource.Tables[0].Rows[0]["content"].ToString();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(tongzhidetail);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }


        }

        /// <summary>
        /// 这个获取督导的通讯录暂时不要了，只要经销商的通讯录
        /// </summary>
        private void getdudaotongxunlulist(HttpContext context)
        {
            //var pageIndex = QueryString.GetQuery(context, "pageIndex");
            //var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "select * from TongXunLu");

            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<TongXunLu> lists = new List<TongXunLu>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    lists.Add(new TongXunLu()
                    {
                        id = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString().Trim()),
                        BuMen = datasource.Tables[0].Rows[i]["BuMen"].ToString().Trim(),
                        Mobile = datasource.Tables[0].Rows[i]["Mobile"].ToString().Trim(),
                        ZhiWei = datasource.Tables[0].Rows[i]["ZhiWei"].ToString().Trim(),
                        email = datasource.Tables[0].Rows[i]["email"].ToString().Trim(),
                        realname = datasource.Tables[0].Rows[i]["realname"].ToString().Trim(),
                        username = datasource.Tables[0].Rows[i]["username"].ToString().Trim(),
                        DianMing = datasource.Tables[0].Rows[i]["dianming"] != null ? datasource.Tables[0].Rows[i]["dianming"].ToString().Trim() : ""
                    });
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }


        }

        private void getdianminglistofthisdudao(HttpContext context)
        {
            var mobile = QueryString.GetQuery(context, "mobile");
            //var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                @"select zhuanyinggongsimingcheng,quyu from newtongxunlu where mobile='" + mobile + "' and ZhuanYingGongSiMingCheng is not null and ZhuanYingGongSiMingCheng<>'' ");
            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<Tongxunlu> lists = new List<Tongxunlu>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var zhuanyinggongsimingcheng = datasource.Tables[0].Rows[i]["zhuanyinggongsimingcheng"].ToString();
                    var quyu = datasource.Tables[0].Rows[i]["quyu"].ToString();

                    var tongxulu = new Tongxunlu()
                    {
                        zhuanyinggongsimingcheng = zhuanyinggongsimingcheng,
                        quyu = quyu
                    };

                    lists.Add(tongxulu);
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }
        }

        private void getjingxiaoshangtongxunlulist(HttpContext context)
        {
            //var pageIndex = QueryString.GetQuery(context, "pageIndex");
            //var pageSize = ConfigurationManager.AppSettings["pageSize"].ToString();

            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, @"select xiaoshoufuwudian,zongjingli,zongjinglimobile,xiaoshoujingli,xiaoshoujinglimobile,shichangjingli,shichangjinglimobile
   from newtongxunlu order by xiaoshoufuwudian ");
            if (datasource != null && datasource.Tables.Count > 0)
            {
                List<JXSTongXunLu> lists = new List<JXSTongXunLu>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var xiaoshoufuwudian = datasource.Tables[0].Rows[i]["xiaoshoufuwudian"].ToString();
                    var zongjingli = datasource.Tables[0].Rows[i]["zongjingli"].ToString();
                    var zongjinglimobile = datasource.Tables[0].Rows[i]["zongjinglimobile"].ToString();
                    var shichangjingli = datasource.Tables[0].Rows[i]["shichangjingli"].ToString();
                    var shichangjinglimobile = datasource.Tables[0].Rows[i]["shichangjinglimobile"].ToString();
                    var xiaoshoujingli = datasource.Tables[0].Rows[i]["xiaoshoujingli"].ToString();
                    var xiaoshoujinglimobile = datasource.Tables[0].Rows[i]["xiaoshoujinglimobile"].ToString();
                    if (!string.IsNullOrEmpty(zongjingli) && !string.IsNullOrEmpty(zongjinglimobile))
                    {
                        lists.Add(new JXSTongXunLu()
                        {
                            dianming = xiaoshoufuwudian,
                            position = "总经理",
                            dianhua = zongjinglimobile,
                            renming = zongjingli
                        });
                    }
                    if (!string.IsNullOrEmpty(shichangjingli) && !string.IsNullOrEmpty(shichangjinglimobile))
                    {
                        lists.Add(new JXSTongXunLu()
                        {
                            dianming = xiaoshoufuwudian,
                            position = "市场经理",
                            dianhua = shichangjinglimobile,
                            renming = shichangjingli
                        });
                    }
                    if (!string.IsNullOrEmpty(xiaoshoujingli) && !string.IsNullOrEmpty(xiaoshoujinglimobile))
                    {
                        lists.Add(new JXSTongXunLu()
                        {
                            dianming = xiaoshoufuwudian,
                            position = "销售经理",
                            dianhua = xiaoshoujinglimobile,
                            renming = xiaoshoujingli
                        });
                    }
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(lists);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }


        }

        public void uploadimage(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                var id = QueryString.GetQueryString("id");
                context.Response.ContentType = "image/jpeg";
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile img = files.Get(0);
                //byte[] bytes = new byte[img.ContentLength];
                //using (BinaryReader reader = new BinaryReader(img.InputStream, Encoding.UTF8))
                //{
                //    bytes = reader.ReadBytes(0);
                //}
                ////我这里要把接收的图片 转为2进制，存到数据库
                //context.Response.OutputStream.Write(bytes, 0, img.ContentLength); //这句话的意思是，我吧图片转成了2进制然后返回到安卓，安卓接收了，看看能不能把二进制在转到图片，结果就失败了，就出现了上面那问题

                System.Drawing.Image image = System.Drawing.Image.FromStream(img.InputStream);

                //int newWidth = 300, newHeight = 200;
                //if ((decimal)image.Width / image.Height > (decimal)newWidth / newHeight)
                //{
                //    newHeight = Convert.ToInt32((decimal)image.Height * newWidth / image.Width);
                //}
                //else if ((decimal)image.Width / image.Height < (decimal)newWidth / newHeight)
                //{
                //    newWidth = Convert.ToInt32((decimal)image.Width * newHeight / image.Height);
                //}                
                //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight);

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                g.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
                var picName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                bmp.Save(context.Server.MapPath("~/UploadImages/") + picName);
                bmp.Dispose();
                image.Dispose();


                var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "update TakePictureUpload set picname='" + picName + "' where id=" + id);
            }
            context.Response.Write("OK");
        }

        private void takepictureupload_data(HttpContext context)
        {
            var dianming = QueryString.GetQuery(context, "dianming");
            var riqi = QueryString.GetQuery(context, "riqi");
            string DateStr = "";
            try
            {
                DateTime time = DateTime.Parse(riqi);
                DateStr = time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {
                var firstpart = riqi.Substring(0, 10);
                var lastpart = riqi.Substring(11);
                DateStr = DateTime.Parse(firstpart + " " + lastpart).ToString("yyyy-MM-dd HH:mm:ss");
            }

            //var project = QueryString.GetQuery(context, "project");
            var position = QueryString.GetQuery(context, "position");
            var description = QueryString.GetQuery(context, "description");

            var mainproject = QueryString.GetQuery(context, "mainproject");
            var childproject = QueryString.GetQuery(context, "childproject");

            var picname = QueryString.GetQuery(context, "picname");
            var uploaderphone = QueryString.GetQuery(context, "mobile");

            var ret = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, String.Format("insert into TakePictureUpload(dianming,riqi,position,description,mainproject,childproject,uploaderphone) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ;select @@identity", dianming, DateStr, position, description, mainproject, childproject, uploaderphone));
            //var msg = "";
            //if (ret > 0)
            //{
            //    msg = "success";
            //}
            //else
            //{
            //    msg = "fail";
            //}
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(new ReturnCode() { retid = ret.ToString() });
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }



        //徐颜的拍照上传接口
        private void createttakephoto(HttpContext context)
        {
            try
            {
                //if (context.Request.Files.Count > 0)
                //{
                //    //遍历客户端上传的图片

                //    HttpPostedFile file = context.Request.Files[0];
                //    file.SaveAs(Server.MapPath(Guid.NewGuid().ToString() + ".jpg")); //路径

                //}

                string phone = context.Request.Form["phone"];//手机
                string name = context.Request.Form["name"];//店名
                string mname = context.Request.Form["mname"];//主项目
                string cname = context.Request.Form["cname"];//子项目
                string position = context.Request.Form["position"];//位置
                string time = context.Request.Form["timenow"];//时间
                string description = context.Request.Form["description"];//描述

                //str = "[手机 : " + phone + " ,店名 : " + name + " ,主项目 :" + mname + " ,子项目 : " + cname + " ,时间 : " + time + " ,位置 : " + position + " ,描述 :" + description + "]";

                var ret = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                    String.Format("insert into TakePictureUpload(dianming,riqi,position,description,mainproject,childproject,uploaderphone) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ;select @@identity",
                    name, time, position, description, mname, cname, phone));

                uploadOneImage(ret.ToString(), context);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(new ReturnCode() { retid = ret.ToString() });
                context.Response.ContentType = "text/json;charset=UTF-8;";

                context.Response.Write("ok");
                context.Response.End();
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
                //context.Response.Write("fail");
                context.Response.End();
            }


        }
        public void uploadOneImage(string id, HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                //var id = QueryString.GetQueryString("id");
                context.Response.ContentType = "image/jpeg";
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile img = files.Get(0);
                //byte[] bytes = new byte[img.ContentLength];
                //using (BinaryReader reader = new BinaryReader(img.InputStream, Encoding.UTF8))
                //{
                //    bytes = reader.ReadBytes(0);
                //}
                ////我这里要把接收的图片 转为2进制，存到数据库
                //context.Response.OutputStream.Write(bytes, 0, img.ContentLength); //这句话的意思是，我吧图片转成了2进制然后返回到安卓，安卓接收了，看看能不能把二进制在转到图片，结果就失败了，就出现了上面那问题

                System.Drawing.Image image = System.Drawing.Image.FromStream(img.InputStream);

                //int newWidth = 300, newHeight = 200;
                //if ((decimal)image.Width / image.Height > (decimal)newWidth / newHeight)
                //{
                //    newHeight = Convert.ToInt32((decimal)image.Height * newWidth / image.Width);
                //}
                //else if ((decimal)image.Width / image.Height < (decimal)newWidth / newHeight)
                //{
                //    newWidth = Convert.ToInt32((decimal)image.Width * newHeight / image.Height);
                //}                
                //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight);

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                g.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
                var picName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                bmp.Save(context.Server.MapPath("~/UploadImages/") + picName);
                bmp.Dispose();
                image.Dispose();


                var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "update TakePictureUpload set picname='" + picName + "' where id=" + id);
            }
            //context.Response.Write("OK");
        }

        public void uploadMultipleImages(string id, HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                //var id = QueryString.GetQueryString("id");
                context.Response.ContentType = "image/jpeg";
                HttpFileCollection files = context.Request.Files;
                //HttpPostedFile img = files.Get(0);
                //byte[] bytes = new byte[img.ContentLength];
                //using (BinaryReader reader = new BinaryReader(img.InputStream, Encoding.UTF8))
                //{
                //    bytes = reader.ReadBytes(0);
                //}
                ////我这里要把接收的图片 转为2进制，存到数据库
                //context.Response.OutputStream.Write(bytes, 0, img.ContentLength); //这句话的意思是，我吧图片转成了2进制然后返回到安卓，安卓接收了，看看能不能把二进制在转到图片，结果就失败了，就出现了上面那问题

                var count = files.Count;
                for (var i = 0; i < count; i++)
                {
                    HttpPostedFile img = files.Get(i);
                    System.Drawing.Image image = System.Drawing.Image.FromStream(img.InputStream);

                    //int newWidth = 300, newHeight = 200;
                    //if ((decimal)image.Width / image.Height > (decimal)newWidth / newHeight)
                    //{
                    //    newHeight = Convert.ToInt32((decimal)image.Height * newWidth / image.Width);
                    //}
                    //else if ((decimal)image.Width / image.Height < (decimal)newWidth / newHeight)
                    //{
                    //    newWidth = Convert.ToInt32((decimal)image.Width * newHeight / image.Height);
                    //}                
                    //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight);

                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height);

                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                    g.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
                    var picName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg";
                    bmp.Save(context.Server.MapPath("~/ChuChaiRiZhi/") + picName);
                    bmp.Dispose();
                    image.Dispose();


                    //var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "update ChuChaiRiZhi set picname='" + picName + "' where id=" + id);

                    var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "insert into ChuChaiImages values(" + id + ",'" + picName + "')");
                }
            }
        }

        private void businesstravel(HttpContext context)
        {
            try
            {
                string phone = context.Request.Form["phone"]; //手机号
                string name = context.Request.Form["name"]; //店名
                string shijian = context.Request.Form["shijian"]; //时间
                string positon = context.Request.Form["position"]; //位置
                string miaoshu = context.Request.Form["miaoshu"]; //店的描述
                string shenqing = context.Request.Form["shenqing"]; //协调申请
                string yaoqiu = context.Request.Form["yaoqiu"]; //整改要求

                var ret = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(),
                    CommandType.Text,
                    String.Format(
                        "insert into ChuChaiRiZhi(dianming,riqi,position,miaoshu,zhenggai,xietiao,uploaderphone) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ;select @@identity",
                        name, shijian, positon, miaoshu, yaoqiu, shenqing, phone));

                uploadMultipleImages(ret.ToString(), context);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(new ReturnCode() { retid = ret.ToString() });
                context.Response.ContentType = "text/json;charset=UTF-8;";

                context.Response.Write("ok");
                context.Response.End();
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
                context.Response.End();
            }
        }



        //出差日志
        public void chuchairizhi_data(HttpContext context)
        {
            var dianming = QueryString.GetQuery(context, "dianming");
            var riqi = QueryString.GetQuery(context, "riqi");

            string DateStr = "";
            try
            {
                DateTime time = DateTime.Parse(riqi);
                DateStr = time.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception)
            {

            }

            //var project = QueryString.GetQuery(context, "project");
            var position = QueryString.GetQuery(context, "position");

            var miaoshu = QueryString.GetQuery(context, "miaoshu");
            var zhenggai = QueryString.GetQuery(context, "zhenggai");
            var xietiao = QueryString.GetQuery(context, "xietiao");
            var mobile = QueryString.GetQuery(context, "mobile");

            var ret = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, String.Format("insert into ChuChaiRiZhi(dianming,riqi,position,miaoshu,zhenggai,xietiao,uploaderphone) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}') ;select @@identity", dianming, DateStr, position, miaoshu, zhenggai, xietiao, mobile));
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(new ReturnCode() { retid = ret.ToString() });
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }
        public void chuchairizhi_image(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                var id = QueryString.GetQueryString("id");
                context.Response.ContentType = "image/jpeg";
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile img = files.Get(0);
                //byte[] bytes = new byte[img.ContentLength];
                //using (BinaryReader reader = new BinaryReader(img.InputStream, Encoding.UTF8))
                //{
                //    bytes = reader.ReadBytes(0);
                //}
                ////我这里要把接收的图片 转为2进制，存到数据库
                //context.Response.OutputStream.Write(bytes, 0, img.ContentLength); //这句话的意思是，我吧图片转成了2进制然后返回到安卓，安卓接收了，看看能不能把二进制在转到图片，结果就失败了，就出现了上面那问题

                System.Drawing.Image image = System.Drawing.Image.FromStream(img.InputStream);

                //int newWidth = 300, newHeight = 200;
                //if ((decimal)image.Width / image.Height > (decimal)newWidth / newHeight)
                //{
                //    newHeight = Convert.ToInt32((decimal)image.Height * newWidth / image.Width);
                //}
                //else if ((decimal)image.Width / image.Height < (decimal)newWidth / newHeight)
                //{
                //    newWidth = Convert.ToInt32((decimal)image.Width * newHeight / image.Height);
                //}                
                //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight);

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                g.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
                var picName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg";
                bmp.Save(context.Server.MapPath("~/ChuChaiRiZhi/") + picName);
                bmp.Dispose();
                image.Dispose();


                //var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "update ChuChaiRiZhi set picname='" + picName + "' where id=" + id);

                var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "insert into ChuChaiImages values(" + id + ",'" + picName + "')");
            }
            context.Response.Write("OK");
        }
        //出差日志

        public void chuchairizhi_multiimage(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                var id = QueryString.GetQueryString("id");
                context.Response.ContentType = "image/jpeg";
                HttpFileCollection files = context.Request.Files;
                HttpPostedFile img = files.Get(0);
                //byte[] bytes = new byte[img.ContentLength];
                //using (BinaryReader reader = new BinaryReader(img.InputStream, Encoding.UTF8))
                //{
                //    bytes = reader.ReadBytes(0);
                //}
                ////我这里要把接收的图片 转为2进制，存到数据库
                //context.Response.OutputStream.Write(bytes, 0, img.ContentLength); //这句话的意思是，我吧图片转成了2进制然后返回到安卓，安卓接收了，看看能不能把二进制在转到图片，结果就失败了，就出现了上面那问题

                System.Drawing.Image image = System.Drawing.Image.FromStream(img.InputStream);

                //int newWidth = 300, newHeight = 200;
                //if ((decimal)image.Width / image.Height > (decimal)newWidth / newHeight)
                //{
                //    newHeight = Convert.ToInt32((decimal)image.Height * newWidth / image.Width);
                //}
                //else if ((decimal)image.Width / image.Height < (decimal)newWidth / newHeight)
                //{
                //    newWidth = Convert.ToInt32((decimal)image.Width * newHeight / image.Height);
                //}                
                //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newWidth, newHeight);

                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(image.Width, image.Height);

                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
                g.DrawImage(image, rectDestination, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
                var picName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg";
                bmp.Save(context.Server.MapPath("~/ChuChaiRiZhi/") + picName);
                bmp.Dispose();
                image.Dispose();


                //var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "update ChuChaiRiZhi set picname='" + picName + "' where id=" + id);

                var retid = SqlHelper.ExecuteScalar(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "insert into ChuChaiImages values(" + id + ",'" + picName + "')");
            }
            context.Response.Write("OK");
        }


        private void addnewtongxunlu(HttpContext context)
        {
            var username = QueryString.GetQuery(context, "username");
            var realname = QueryString.GetQuery(context, "realname");
            var bumen = QueryString.GetQuery(context, "bumen");
            var zhiwei = QueryString.GetQuery(context, "zhiwei");
            var mobile = QueryString.GetQuery(context, "mobile");
            var email = QueryString.GetQuery(context, "email");

            var ret = SqlHelper.ExecuteNonQuery(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, string.Format("insert into TongXunLu values('{0}','{1}','{2}','{3}','{4}','{5}')", username, realname, zhiwei, bumen, mobile, email));

            string returnMsg = "";
            if (ret > 0)
            {
                returnMsg = "success";
            }
            else
                returnMsg = "fail";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(returnMsg);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);


        }
        private void deletetongxunlu(HttpContext context)
        {
            var id = QueryString.GetQuery(context, "id");

            var ret = SqlHelper.ExecuteNonQuery(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, string.Format("delete TongXunLu where id={0}", id));

            string returnMsg = "";
            if (ret > 0)
            {
                returnMsg = "success";
            }
            else
                returnMsg = "fail";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(returnMsg);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }

        private void updatetongxunlu(HttpContext context)
        {
            var username = QueryString.GetQuery(context, "username");
            var realname = QueryString.GetQuery(context, "realname");
            var bumen = QueryString.GetQuery(context, "bumen");
            var zhiwei = QueryString.GetQuery(context, "zhiwei");
            var mobile = QueryString.GetQuery(context, "mobile");
            var email = QueryString.GetQuery(context, "email");
            var id = QueryString.GetQuery(context, "id");

            var ret = SqlHelper.ExecuteNonQuery(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, string.Format("update TongXunLu set username='{0}',realname='{1}',zhiwei='{2}',bumen='{3}',mobile='{4}',email='{5}' where id={6}", username, realname, zhiwei, bumen, mobile, email, id));

            string returnMsg = "";
            if (ret > 0)
            {
                returnMsg = "success";
            }
            else
                returnMsg = "fail";

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(returnMsg);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }

        #region getStaffPositionList by Lee 20150712

        private void getStaffPositionList(HttpContext context)
        {
            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, @"SELECT Id,PositionName FROM StaffPosition WHERE Status = 1 ORDER BY CreateTime");
            if (datasource != null && datasource.Tables.Count > 0)
            {
                var list = new List<StaffPosition>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var position = new StaffPosition();
                    var Id = Convert.ToInt32(datasource.Tables[0].Rows[i]["Id"]);
                    var positionName = datasource.Tables[0].Rows[i]["PositionName"].ToString();

                    position.Id = Id;
                    position.PositionName = positionName;
                    list.Add(position);
                }
                var serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(list);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }
        }

        #endregion

        #region getstafflist by Lee 20150712

        private void getStaffList(HttpContext context)
        {

            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                @"SELECT StaffName,Mobile FROM Staff WHERE Position = '督导' ORDER BY CreateTime");
            if (datasource != null && datasource.Tables.Count > 0)
            {
                var list = new List<Staff>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var staff = new Staff();
                    var staffName = datasource.Tables[0].Rows[i]["StaffName"].ToString();
                    var mobile = datasource.Tables[0].Rows[i]["Mobile"].ToString();

                    staff.StaffName = staffName;
                    staff.Mobile = mobile;
                    list.Add(staff);
                }
                var serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(list);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }
        }

        #endregion

        private void getAreaList(HttpContext context)
        {
            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                    @"select top 100 * from area where satus = 1 order by ID desc");
            if (datasource != null && datasource.Tables.Count > 0)
            {
                var list = new List<Daqu>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    var area = new Daqu();
                    var Code = datasource.Tables[0].Rows[i]["AreaCode"].ToString();
                    var Name = datasource.Tables[0].Rows[i]["AreaName"].ToString();

                    area.Code = Code;
                    area.Name = Name;

                    var dianSource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                                        @"select 
                                            quyu,zhuanyinggongsimingcheng ,xiaoshoufuwudian
                                            from newtongxunlu 
                                            where quyu='" + Name + "' and ZhuanYingGongSiMingCheng is not null and ZhuanYingGongSiMingCheng<>''");
                    if (dianSource != null && dianSource.Tables[0].Rows.Count > 0)
                    {
                        var dianList = new List<Tongxunlu>();
                        for (int j = 0; j < dianSource.Tables[0].Rows.Count; j++)
                        {
                            var dian = new Tongxunlu()
                            {
                                quyu = dianSource.Tables[0].Rows[j]["quyu"].ToString(),
                                xiaoshoufuwudian = dianSource.Tables[0].Rows[j]["xiaoshoufuwudian"].ToString(),
                                zhuanyinggongsimingcheng = dianSource.Tables[0].Rows[j]["zhuanyinggongsimingcheng"].ToString()
                            };

                            dianList.Add(dian);
                        }

                        area.TongxunluList = dianList;
                    }

                    list.Add(area);
                }

                var serializer = new JavaScriptSerializer();
                var str = serializer.Serialize(list);
                context.Response.ContentType = "text/json;charset=UTF-8;";
                context.Response.Write(str);
            }
        }

        private void updatePassword(HttpContext context)
        {
            var username = QueryString.GetQuery(context, "username");
            var oldpass = QueryString.GetQuery(context, "oldpass");
            var newpass = QueryString.GetQuery(context, "newpass");

            string returnMsg = "用户名或者原密码不正确";
            var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                    @"select top 100 * from DuDaoRights where mobile = '" + username + "' and password = '" + oldpass + "' order by ID desc");
            if (datasource != null && datasource.Tables[0].Rows.Count > 0)
            {
                var ret = SqlHelper.ExecuteNonQuery(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text,
                    string.Format("update [DuDaoRights] set password='{0}' where mobile={1}", newpass, username));
                if (ret > 0)
                {
                    returnMsg = "success";
                }
                else
                {
                    returnMsg = "更新失败";
                }
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(returnMsg);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }


        private void testTui(HttpContext context)
        {
            UMengMessagePush umPush = new UMengMessagePush("559f209e67e58e9460006075", "exh3fjeoumuk4tbqngbsnxown8uqyhdz");
            PostUMengJson postJson = new PostUMengJson();

            //Atw_PkXzlbY0FkeJsx773xEcFol1Hp4ue3Fp0-7Fzg-p

            postJson.type = "broadcast";
            postJson.payload = new Payload();
            postJson.payload.display_type = "notification";
            postJson.payload.body = new ContentBody();
            postJson.payload.body.ticker = "ticker";
            postJson.payload.body.title = "侬好哇";
            postJson.payload.body.text = "text。。侬好哇。。侬好哇。";
            postJson.payload.body.after_open = "go_custom";
            postJson.payload.body.custom = "comment-notify";

            postJson.description = "description-UID:" + 123;

            postJson.thirdparty_id = "COMMENT";

            ReturnJsonClass resu = umPush.SendMessage(postJson);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(resu);
            context.Response.ContentType = "text/json;charset=UTF-8;";
            context.Response.Write(str);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}