﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using EntityFramework.Extensions;
using X3;
using System.Text;
using System.IO;
using NSTool.UMengPush;
using NSTool.UMengPush.Core;
using System.Web.Script.Serialization;

namespace HaiMaApp.Web.Controllers
{
    [Login]
    public class HomeController : Controller
    {
        HaiMaApp db = new HaiMaApp();

        string inputKeyword = "请输入关键字";

        #region tongxunlu
        public ActionResult Contact(int pn = 1, string kword = null, string quyuorder = null, string dudaomingziorder = null, string zongjingliorder = null,
                                    string xiaoshoujingliorder = null, string shichangjingliorder = null)
        {
            var query = db.NewTongXunLu.AsQueryable();
            if (!string.IsNullOrWhiteSpace(kword))
                query = query.Where(s => s.XiaoShouFuWuDian.Contains(kword)
                    || s.DuDaoMingZi.Contains(kword)
                    || s.Mobile.Contains(kword)
                    || s.QuYu.Contains(kword)
                    || s.ShengFen.Contains(kword)
                    || s.ZongJingLi.Contains(kword)
                    || s.ZongJingLiMobile.Contains(kword)
                    || s.XiaoShouJingLi.Contains(kword)
                    || s.XiaoShouJingLiMobile.Contains(kword)
                    || s.ShiChangJingLi.Contains(kword)
                    || s.ShiChangJingLiMobile.Contains(kword)
                    );
            if (!string.IsNullOrWhiteSpace(quyuorder))
            {
                if (quyuorder.ToLower() == "desc")
                    query = query.OrderByDescending(c => c.QuYu);
                else
                    query = query.OrderBy(c => c.QuYu);
            }
            if (!string.IsNullOrWhiteSpace(dudaomingziorder))
            {
                if (dudaomingziorder.ToLower() == "desc")
                    query = query.OrderByDescending(c => c.DuDaoMingZi);
                else
                    query = query.OrderBy(c => c.DuDaoMingZi);
            }
            if (!string.IsNullOrWhiteSpace(zongjingliorder))
            {
                if (zongjingliorder.ToLower() == "desc")
                    query = query.OrderByDescending(c => c.ZongJingLi);
                else
                    query = query.OrderBy(c => c.ZongJingLi);
            }
            if (!string.IsNullOrWhiteSpace(xiaoshoujingliorder))
            {
                if (xiaoshoujingliorder.ToLower() == "desc")
                    query = query.OrderByDescending(c => c.XiaoShouJingLi);
                else
                    query = query.OrderBy(c => c.XiaoShouJingLi);
            }
            if (!string.IsNullOrWhiteSpace(shichangjingliorder))
            {
                if (shichangjingliorder.ToLower() == "desc")
                    query = query.OrderByDescending(c => c.ShiChangJingLi);
                else
                    query = query.OrderBy(c => c.ShiChangJingLi);
            }

            if (string.IsNullOrWhiteSpace(quyuorder) && string.IsNullOrWhiteSpace(dudaomingziorder) &&
                string.IsNullOrWhiteSpace(zongjingliorder) && string.IsNullOrWhiteSpace(xiaoshoujingliorder) &&
                string.IsNullOrWhiteSpace(shichangjingliorder))
            {
                query = query.OrderByDescending(c => c.id);
            }

            var model = query.ToPagedList(pn, 10);

            ViewBag.kword = string.IsNullOrWhiteSpace(kword) ? inputKeyword : kword;

            return View(model);
        }

        public ActionResult AddContact()
        {
            //var curentUser = UtilX3.GetCookie("hmToken");
            //ViewBag.Mobile = curentUser;

            ////modify by Lee 20150722
            //var result = db.SuperAdmin.Where(d => d.username.ToString().ToLower() == curentUser.ToLower()).FirstOrDefault();
            //if (result != null)
            //{
            //    ViewBag.DuDaoMingZi = result.username;
            //}
            //else
            //{
            //    var resultX = db.DuDaoRights.Where(d => d.mobile == curentUser && d.HasRights == true).FirstOrDefault();
            //    if (resultX != null)
            //    {
            //        ViewBag.DuDaoMingZi = resultX.DuDaoMingZi;
            //    }
            //    else
            //    {

            //    }
            //}

            //if (curentUser.ToLower() != "admin")
            //{
            //    ViewBag.Mobile = curentUser;
            //    ViewBag.DuDaoMingZi = db.DuDaoRights.Where(d => d.mobile == curentUser).FirstOrDefault().DuDaoMingZi;
            //}
            return View();
        }

        [HttpPost]
        public ActionResult AddContact(string QuYu, string ShengFen, string DuDaoMingZi, string Mobile,
                                       string XiaoShouFuWuDian, string ZhuanYingGongSiMingCheng, string Address,
                                       string ZongJingLi, string ZongJingLiMobile, string XiaoShouJingLi, string XiaoShouJingLiMobile,
                                       string ShiChangJingLi, string ShiChangJingLiMobile)
        {
            var array = DuDaoMingZi.Contains('/') ? DuDaoMingZi.Split('/') : new string[] { "", "" };
            var dudao = array[0];
            var mobile = array[1];
            var model = new NewTongXunLu
            {
                QuYu = QuYu,
                ShengFen = ShengFen,
                DuDaoMingZi = dudao,
                Mobile = mobile,
                XiaoShouFuWuDian = XiaoShouFuWuDian,
                ZhuanYingGongSiMingCheng = ZhuanYingGongSiMingCheng,
                Address = Address,
                ZongJingLi = ZongJingLi,
                ZongJingLiMobile = ZongJingLiMobile,
                XiaoShouJingLi = XiaoShouJingLi,
                XiaoShouJingLiMobile = XiaoShouJingLiMobile,
                ShiChangJingLi = ShiChangJingLi,
                ShiChangJingLiMobile = ShiChangJingLiMobile
            };
            db.NewTongXunLu.Add(model);
            db.SaveChanges();
            //return RedirectToAction("contact");

            return Json("1");
        }

        public ActionResult EditContact(int id)
        {
            var model = db.NewTongXunLu.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditContact(int id, string QuYu, string ShengFen, string DuDaoMingZi, string Mobile,
                                       string XiaoShouFuWuDian, string ZhuanYingGongSiMingCheng, string Address,
                                       string ZongJingLi, string ZongJingLiMobile, string XiaoShouJingLi, string XiaoShouJingLiMobile,
                                       string ShiChangJingLi, string ShiChangJingLiMobile)
        {
            var array = DuDaoMingZi.Contains('/') ? DuDaoMingZi.Split('/') : new string[] { "", "" };
            var dudao = array[0];
            var mobile = array[1];
            db.NewTongXunLu.Where(t => t.id == id).Update(t => new NewTongXunLu
            {
                QuYu = QuYu,
                ShengFen = ShengFen,
                DuDaoMingZi = dudao,
                Mobile = mobile,
                XiaoShouFuWuDian = XiaoShouFuWuDian,
                ZhuanYingGongSiMingCheng = ZhuanYingGongSiMingCheng,
                Address = Address,
                ZongJingLi = ZongJingLi,
                ZongJingLiMobile = ZongJingLiMobile,
                XiaoShouJingLi = XiaoShouJingLi,
                XiaoShouJingLiMobile = XiaoShouJingLiMobile,
                ShiChangJingLi = ShiChangJingLi,
                ShiChangJingLiMobile = ShiChangJingLiMobile
            });

            //return RedirectToAction("contact");
            return Json("1");
        }

        public ActionResult DeleteContact(int id)
        {
            db.NewTongXunLu.Where(c => c.id == id).Delete();
            return Json("1");
        }
        #endregion

        #region tongzhi
        public ActionResult notice(int pn = 1, string kword = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var query = db.TongZhi.AsQueryable();
            if (!string.IsNullOrWhiteSpace(kword))
                query = query.Where(s => s.title.Contains(kword));
            if (!string.IsNullOrWhiteSpace(starttime.ToStringX3()))
                query = query.Where(s => s.time >= starttime);
            if (!string.IsNullOrWhiteSpace(endtime.ToStringX3()))
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(s => s.time < endtime);
            }
            var model = query.OrderByDescending(c => c.id).ToPagedList(pn, 10);
            model.ForEach(m =>
            {
                var obj = db.DuDaoRights.Where(d => d.mobile == m.sender).FirstOrDefault();
                if (obj != null)
                    m.sender = obj.DuDaoMingZi;
                else
                    m.sender = "";
            });

            ViewBag.kword = string.IsNullOrWhiteSpace(kword) ? inputKeyword : kword;
            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";

            return View(model);
        }

        public ActionResult Addnotice()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Addnotice(string title, string content, int ispublish)
        {
            var model = new TongZhi
            {
                title = title,
                content = content,
                time = DateTime.Now,
                sender = UtilX3.GetCookie("hmToken"),
                IsPublish = ispublish == 1 ? true : false
            };
            db.TongZhi.Add(model);
            db.SaveChanges();
            // return RedirectToAction("notice");

            //调通知接口
            if (ispublish == 1)
            {
                try
                {
                    testAndroidTui(title, content);
                    testIOSTui(title, content);
                }
                catch (Exception ex)
                {
                    LogX3.Error("Addnotice-testTui:" + ex.Message + ex.StackTrace);
                }
            }

            return Json("1");
        }

        public ActionResult Editnotice(int id)
        {
            var model = db.TongZhi.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Editnotice(int id, string title, string content, int ispublish)
        {
            db.TongZhi.Where(t => t.id == id).Update(t => new TongZhi
            {
                title = title,
                content = content,
                IsPublish = ispublish == 1 ? true : false
            });

            //  return RedirectToAction("notice");

            //调通知接口
            if (ispublish == 1)
            {
                try
                {
                    testAndroidTui(title, content);
                    testIOSTui(title, content);
                }
                catch (Exception ex)
                {
                    LogX3.Error("Editnotice-testTui:" + ex.Message + ex.StackTrace);
                }
            }


            return Json("1");
        }

        //add by Lee 20150716 publish or cancel
        public ActionResult PublishNotice(int id, int ispublish)
        {
            db.TongZhi.Where(c => c.id == id).Update(t => new TongZhi
            {
                title = t.title,
                content = t.content,
                IsPublish = ispublish == 1 ? false : true
            });

            //调通知接口
            if (ispublish != 1)
            {
                var tongzhi = db.TongZhi.Where(c => c.id == id).FirstOrDefault();
                if (tongzhi != null)
                {
                    try
                    {
                        testAndroidTui(tongzhi.title, tongzhi.content);
                        testIOSTui(tongzhi.title, tongzhi.content);
                    }
                    catch (Exception ex)
                    {
                        LogX3.Error("PublishNotice-testTui:" + ex.Message + ex.StackTrace);
                    }
                }
            }


            return Json("1");
        }

        public ActionResult DeleteNotice(int id)
        {
            db.TongZhi.Where(c => c.id == id).Delete();
            return Json("1");
        }

        #region testTui  只推title
        //copy from myhandler 
        private void testAndroidTui(string title, string text)
        {
            //push andriod
            UMengMessagePush umPush = new UMengMessagePush("559f209e67e58e9460006075", "exh3fjeoumuk4tbqngbsnxown8uqyhdz");
            PostUMengJson postJson = new PostUMengJson();

            //Atw_PkXzlbY0FkeJsx773xEcFol1Hp4ue3Fp0-7Fzg-p

            postJson.type = "broadcast";
            postJson.payload = new Payload();
            postJson.payload.display_type = "notification";
            postJson.payload.body = new ContentBody();
            postJson.payload.body.ticker = "ticker";
            postJson.payload.body.title = title;//"侬好哇";
            postJson.payload.body.text = "";    //text;  //"text。。侬好哇。。侬好哇。";
            postJson.payload.body.after_open = "go_custom";
            postJson.payload.body.custom = "comment-notify";

            postJson.description = "description-UID:" + 123;
            postJson.thirdparty_id = "COMMENT";

            ReturnJsonClass resu = umPush.SendMessage(postJson);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(resu);

            LogX3.Info(DateTime.Now + "-testAndroidTui" + str);
        }

        private void testIOSTui(string title,string text)
        {
            //////// apple
            var umPush = new UMengMessagePush("55a239b067e58e1a080075b7", "nqj7zbo88lfifcagcvs5adewnvkxqioi");
            var postJson = new PostUMengJson();

            //Atw_PkXzlbY0FkeJsx773xEcFol1Hp4ue3Fp0-7Fzg-p

            postJson.type = "broadcast";
            postJson.payload = new Payload();
            postJson.payload.display_type = "notification";

            postJson.payload.aps = new Aps();
            postJson.payload.aps.badge = 0;
            postJson.payload.aps.alert = title; 
            postJson.payload.aps.sound = "chime";

            postJson.production_mode = "true";
            postJson.description = "description-UID:" + 123;

            postJson.thirdparty_id = "COMMENT";

            var resu = umPush.SendMessage(postJson);

            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(resu);

            LogX3.Info(DateTime.Now + "-testIOSTui" + str);
        }

        #endregion

        #endregion

        #region dudaoPictureUpload  & dudaolog
        public ActionResult dudaoPictureUpload(int pn = 1, string dm = null, string zhuxm = null, string zixm = null, string dudaomz = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var query = db.TakePictureUpload_dudao.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dm))
            {
                query = query.Where(s => s.dianming.Contains(dm));
                ViewBag.dm = dm;
            }
            if (!string.IsNullOrWhiteSpace(zhuxm) && zhuxm != "0")
            {
                query = query.Where(s => s.mainproject.Contains(zhuxm));
                ViewBag.zhuxm = zhuxm;
            }
            if (!string.IsNullOrWhiteSpace(zixm) && zixm != "0")
            {
                query = query.Where(s => s.childproject.Contains(zixm));
                ViewBag.zixm = zixm;
            }
            if (!string.IsNullOrWhiteSpace(starttime.ToStringX3()))
            {
                query = query.Where(s => s.riqi >= starttime);
                ViewBag.starttime = starttime;
            }
            if (!string.IsNullOrWhiteSpace(endtime.ToStringX3()))
            {
                ViewBag.endtime = endtime;
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(s => s.riqi < endtime);
            }
            var model = query.OrderByDescending(c => c.id).ToPagedList(pn, 10);

            ViewBag.dm = string.IsNullOrWhiteSpace(dm) ? inputKeyword : dm;
            ViewBag.zhuxm = (string.IsNullOrWhiteSpace(zhuxm) || zhuxm == "0") ? "0" : zhuxm;
            ViewBag.zixm = (string.IsNullOrWhiteSpace(zixm) || zixm == "0") ? "0" : zixm;
            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";

            return View(model);
        }

        public ActionResult DeleteTakePicture(int id)
        {
            db.TakePictureUpload.Where(c => c.id == id).Delete();
            return Json("1");
        }

        public ActionResult dudaolog(int pn = 1, string dm = null, string dudaomz = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var query = db.ChuChaiRiZhi_dudao.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dm))
            {
                query = query.Where(s => s.dianming.Contains(dm));
                ViewBag.dm = dm;
            }
            if (!string.IsNullOrWhiteSpace(dudaomz))
            {
                query = query.Where(s => s.DuDaoMingZi.Contains(dudaomz));
                ViewBag.dudaomz = dudaomz;
            }
            if (!string.IsNullOrWhiteSpace(starttime.ToStringX3()))
            {
                query = query.Where(s => s.riqi >= starttime);
                ViewBag.starttime = starttime;
            }
            if (!string.IsNullOrWhiteSpace(endtime.ToStringX3()))
            {
                ViewBag.endtime = endtime;
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(s => s.riqi < endtime);
            }
            var model = query.OrderByDescending(c => c.id).ToPagedList(pn, 10);
            return View(model);
        }

        public ActionResult DeleteLog(int id)
        {
            db.ChuChaiRiZhi.Where(c => c.id == id).Delete();
            return Json("1");
        }

        public string tipifu(int id, string content)
        {
            var model = new PiFuOnChuChai { chuchaiID = id, PiFuContent = content };
            db.PiFuOnChuChai.Add(model);
            db.SaveChangesAsync();
            return "";
        }

        public ActionResult test()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult dudaoPictureUploadToExcel(string dm = null, string zhuxm = null, string zixm = null, string dudaomz = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "店名", "主项目", "子项目", "地址", "描述", "日期", "督导" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");

            var query = db.TakePictureUpload_dudao.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dm))
                query = query.Where(s => s.dianming.Contains(dm));
            if (!string.IsNullOrWhiteSpace(zhuxm))
                query = query.Where(s => s.mainproject.Contains(zhuxm));
            if (!string.IsNullOrWhiteSpace(zixm))
                query = query.Where(s => s.childproject.Contains(zixm));
            if (!string.IsNullOrWhiteSpace(starttime.ToStringX3()))
                query = query.Where(s => s.riqi >= starttime);
            if (!string.IsNullOrWhiteSpace(endtime.ToStringX3()))
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(s => s.riqi < endtime);
            }
            foreach (var p in query.ToList())
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.dianming);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.mainproject);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.childproject);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.position);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.description);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.riqi.ToDateString());
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.DuDaoMingZi);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return ExportExcel(sbHtml.ToString(), "督导拍照上传列表");
        }

        public ActionResult dudaologToExcel(string dm = null, string dudaomz = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "店名", "地址", "店描述", "整改要求", "协调申请", "日期", "督导" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");

            var query = db.ChuChaiRiZhi_dudao.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dm))
            {
                query = query.Where(s => s.dianming.Contains(dm));
                ViewBag.dm = dm;
            }
            if (!string.IsNullOrWhiteSpace(dudaomz))
            {
                query = query.Where(s => s.DuDaoMingZi.Contains(dudaomz));
                ViewBag.dudaomz = dudaomz;
            }
            if (!string.IsNullOrWhiteSpace(starttime.ToStringX3()))
            {
                query = query.Where(s => s.riqi >= starttime);
                ViewBag.starttime = starttime;
            }
            if (!string.IsNullOrWhiteSpace(endtime.ToStringX3()))
            {
                ViewBag.endtime = endtime;
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(s => s.riqi < endtime);
            }
            foreach (var p in query.ToList())
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.dianming);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.position);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.miaoshu);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.zhenggai);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.xietiao);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.riqi.ToDateString());
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", p.DuDaoMingZi);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return ExportExcel(sbHtml.ToString(), "出差日志列表");
        }

        public FileResult ExportExcel(string htmlStr, string filename)
        {
            //var sbHtml = new StringBuilder();
            //sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            //sbHtml.Append("<tr>");
            //var lstTitle = new List<string> { "编号", "姓名", "年龄", "创建时间" };
            //foreach (var item in lstTitle)
            //{
            //    sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            //}
            //sbHtml.Append("</tr>");

            //for (int i = 0; i < 1000; i++)
            //{
            //    sbHtml.Append("<tr>");
            //    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", i);
            //    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>屌丝{0}号</td>", i);
            //    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", new Random().Next(20, 30) + i);
            //    sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", DateTime.Now);
            //    sbHtml.Append("</tr>");
            //}
            //sbHtml.Append("</table>");

            //第一种:使用FileContentResult
            byte[] fileContents = Encoding.Default.GetBytes(htmlStr);
            return File(fileContents, "application/ms-excel", filename + ".xls");

            ////第二种:使用FileStreamResult
            //var fileStream = new MemoryStream(fileContents);
            //return File(fileStream, "application/ms-excel", "fileStream.xls");

            ////第三种:使用FilePathResult
            ////服务器上首先必须要有这个Excel文件,然会通过Server.MapPath获取路径返回.
            //var fileName = Server.MapPath("~/Files/fileName.xls");
            //return File(fileName, "application/ms-excel", "fileName.xls");
        }

        #endregion

        #region staff by Lee 20150712

        //根据员工姓名|电话|大区|职位进行搜索
        public ActionResult StaffManage(int pn = 1, string keyword = null)
        {
            var query = db.Staff.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(s => s.StaffName.Contains(keyword) ||
                        s.Mobile.Contains(keyword) ||
                        s.Area.Contains(keyword) ||
                        s.Position.Contains(keyword));

            var model = query.OrderByDescending(c => c.Position)
                             .OrderByDescending(c => c.Area)
                             .ToPagedList(pn, 10);
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;

            return View(model);
        }

        public ActionResult AddStaff()
        {
            //string curentUser = UtilX3.GetCookie("hmToken");
            //if (curentUser.ToLower() != "admin")
            //{
            //    ViewBag.Mobile = curentUser;
            //    ViewBag.DuDaoMingZi = db.DuDaoRights.Where(d => d.mobile == curentUser).FirstOrDefault().DuDaoMingZi;
            //}
            return View();
        }

        [HttpPost]
        public ActionResult AddStaff(string staffName, string mobile, string area, string position)
        {
            var password = string.IsNullOrWhiteSpace(mobile) ? "" : mobile.Substring(mobile.Length - 6);
            var model = new Staff()
                {
                    StaffName = staffName,
                    Mobile = mobile,
                    Area = area,
                    Position = position,
                    Creator = string.IsNullOrEmpty(UtilX3.GetCookie("hmToken")) ? "" : UtilX3.GetCookie("hmToken"),
                    Createtime = DateTime.Now,
                    LoginId = mobile,
                    LoginPassword = password
                };
            //有，删
            db.Staff.Where(t => t.Mobile == mobile).Delete();
            //删完，增
            db.Staff.Add(model);

            //DuDaoRights，SuperAdmin都先置无效，再新增
            db.DuDaoRights.Where(t => t.mobile == mobile).Update(t => new DuDaoRights
            {
                HasRights = false
            });
            db.SuperAdmin.Where(t => t.username == mobile).Update(t => new SuperAdmin
            {
                Status = false
            });

            //如果是督导，向DuDaoRights表写数据；如果是大区经理，向SuperAdmin表写数据
            if (position.Contains("督导"))
            {
                var dudao = new DuDaoRights()
                {
                    DuDaoMingZi = staffName,
                    mobile = mobile,
                    Password = password,
                    HasRights = true
                };

                db.DuDaoRights.Add(dudao);
            }
            else if (position.Contains("经理"))
            {
                var admin = new SuperAdmin()
                {
                    username = mobile,
                    password = password,
                    Status = true,
                    memo = staffName
                };

                db.SuperAdmin.Add(admin);
            }

            db.SaveChanges();

            //return RedirectToAction("StaffManage");
            return Json("1");
        }

        public ActionResult EditStaff(int id)
        {
            var model = db.Staff.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditStaff(int id, string staffName, string mobile, string area, string position)
        {
            //找到原始的数据，置无效
            var oldData = db.Staff.Where(t => t.Id == id).FirstOrDefault();
            var password = string.IsNullOrWhiteSpace(mobile) ? "" : mobile.Substring(mobile.Length - 6);

            //更新staff表
            db.Staff.Where(t => t.Id == id).Update(t => new Staff
            {
                StaffName = staffName,
                Mobile = mobile,
                Area = area,
                Position = position,
                Modifier = string.IsNullOrEmpty(UtilX3.GetCookie("hmToken")) ? "" : UtilX3.GetCookie("hmToken"),
                Modifytime = DateTime.Now,
                LoginId = mobile,
                LoginPassword = password
            });

            //如果是督导，向DuDaoRights表更新数据；如果是大区经理，向SuperAdmin表更新数据
            //DuDaoRights，SuperAdmin都先置无效，再新增
            db.DuDaoRights.Where(t => t.mobile == oldData.Mobile).Update(t => new DuDaoRights
            {
                HasRights = false
            });
            db.SuperAdmin.Where(t => t.username == oldData.Mobile).Update(t => new SuperAdmin
            {
                Status = false
            });

            if (position.Contains("督导"))
            {
                var dudao = new DuDaoRights()
                {
                    DuDaoMingZi = staffName,
                    mobile = mobile,
                    Password = password,
                    HasRights = true
                };
                db.DuDaoRights.Add(dudao);
            }
            else if (position.Contains("经理"))
            {
                var admin = new SuperAdmin()
                {
                    username = mobile,
                    password = password,
                    Status = true,
                    memo = staffName
                };
                db.SuperAdmin.Add(admin);
            }

            db.SaveChanges();

            // return RedirectToAction("StaffManage");
            return Json("1");
        }

        public ActionResult DeleteStaff(int id)
        {
            var staff = db.Staff.Where(c => c.Id == id).FirstOrDefault();

            db.Staff.Where(c => c.Id == id).Delete();

            //如果是督导，向DuDaoRights表更新数据；如果是大区经理，向SuperAdmin表更新数据
            if (staff.Position.Contains("督导"))
            {
                db.DuDaoRights.Where(t => t.mobile == staff.Mobile).Update(t => new DuDaoRights
                {
                    HasRights = false
                });
            }
            else if (staff.Position.Contains("经理"))
            {
                db.SuperAdmin.Where(t => t.username == staff.Mobile).Update(t => new SuperAdmin
                {
                    Status = false
                });
            }

            return Json("1");
        }

        //更新员工登录密码 by Lee 20150720  
        public ActionResult ModifyPassword(string mobile, string password)
        {
            var staff = db.Staff.Where(c => c.Mobile == mobile).FirstOrDefault();

            db.Staff.Where(c => c.Mobile == mobile).Update(t => new Staff
            {
                LoginPassword = password,
                Modifier = string.IsNullOrEmpty(UtilX3.GetCookie("hmToken")) ? "" : UtilX3.GetCookie("hmToken"),
                Modifytime = DateTime.Now
            });

            //如果是督导，向DuDaoRights表更新数据；如果是大区经理，向SuperAdmin表更新数据
            if (staff.Position.Contains("督导"))
            {
                db.DuDaoRights.Where(t => t.mobile == staff.Mobile).Update(t => new DuDaoRights
                {
                    Password = password
                });
            }
            else if (staff.Position.Contains("经理"))
            {
                db.SuperAdmin.Where(t => t.username == staff.Mobile).Update(t => new SuperAdmin
                {
                    password = password
                });
            }

            return Json("1");
        }

        #endregion

        #region superviser's trip log by Lee 20150714
        //按区域维度统计督导出差日志
        public ActionResult AreaStatistics(int pn = 1, string keyword = null, DateTime? starttime = null, DateTime? endtime = null,
                                           string areaorder = null, string countlogorder = null)
        {
            var areastr = !string.IsNullOrWhiteSpace(keyword) ? string.Format("AND tmp.Area LIKE '%{0}%'", keyword) : "";
            var orderstr = " tmp.CountLog DESC";
            orderstr = !string.IsNullOrWhiteSpace(areaorder) ? string.Format(" tmp.Area {0}", areaorder) : orderstr;
            orderstr = !string.IsNullOrWhiteSpace(countlogorder) ? string.Format(" tmp.CountLog {1}", countlogorder) : orderstr;

            var starttimestr = "";
            var endtimestr = "";
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Today.AddDays(1);
                starttime = now.AddMonths(-1);
                endtime = now;
            }

            starttimestr = starttime.HasValue ? string.Format("AND UploadDate >= '{0}'", starttime) : "";
            endtimestr = endtime.HasValue ? string.Format("AND UploadDate < '{0}'", Convert.ToDateTime(endtime)) : "";

            var commandText = string.Format(@"SELECT * FROM
                                            (
	                                            SELECT A.AreaName AS Area,ISNULL(SUM(S.CountLog),0) AS CountLog
	                                            FROM Area A
	                                            LEFT JOIN 
	                                            (
		                                            SELECT * 
		                                            FROM View_AreaStatisticsTripLog
		                                            WHERE 1 = 1
                                                    {0} {1}
	                                            ) S ON A.AreaName = S.Area
	                                            WHERE  A.Satus = 1
	                                            GROUP BY A.AreaName
                                            )tmp 
                                            WHERE 1=1 {2}
                                            ORDER BY {3}", starttimestr, endtimestr, areastr, orderstr);
            var datasource = SqlHelper.ExecuteDataset(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString(), System.Data.CommandType.Text, commandText);

            var model = new List<View_AreaStatisticsTripLog>();

            if (datasource != null && datasource.Tables.Count > 0)
            {
                var lists = new List<View_AreaStatisticsTripLog>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    lists.Add(new View_AreaStatisticsTripLog()
                    {
                        Area = datasource.Tables[0].Rows[i]["Area"].ToString(),
                        CountLog = Int32.Parse(datasource.Tables[0].Rows[i]["CountLog"].ToString())
                    });
                }

                model = lists.AsEnumerable().ToPagedList(pn, 10);
            }

            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;

            return View(model);

            #region todo delete
            /*
            var query = db.View_AreaStatisticsTripLog.AsQueryable();
            if (starttime.HasValue)
                query = query.Where(t => t.UploadDate >= starttime);
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(t => t.UploadDate < endtime);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Now;
                starttime = now.AddMonths(-1);
                endtime = now;
                query = query.Where(t => t.UploadDate >= starttime &&
                                       t.UploadDate < endtime);
            }

            //groupby
            var queryY = (from r in db.Area
                          join s in query on r.AreaName equals s.Area into union
                          from v in union.DefaultIfEmpty()
                          select new
                          {
                              Area = r.AreaName,
                              CountLog = v == null ? 0 : v.CountLog
                          }).GroupBy(t => new { t.Area, t.CountLog });

            var queryX = from q in queryY.AsEnumerable()
                         group q by q.Key.Area into ultimate
                         select new
                         {
                             Area = ultimate.Key,
                             CountLog = ultimate.Sum(q => q.Key.CountLog)
                         };

            if (!string.IsNullOrWhiteSpace(keyword))
                queryX = queryX.Where(t => t.Area == keyword);


            queryX.ToList().ForEach(t => new View_AreaStatisticsTripLog
            {
                Area = t.Area,
                CountLog = t.CountLog
            });

            if (string.IsNullOrWhiteSpace(areaorder) && string.IsNullOrWhiteSpace(countlogorder))
                queryX = queryX.OrderBy(t => t.Area);

            var list = new List<View_AreaStatisticsTripLog>();
            foreach (var item in queryX.AsEnumerable())
            {
                var area = new View_AreaStatisticsTripLog();
                area.Area = item.Area;
                area.CountLog = item.CountLog;
                list.Add(area);
            };

            if (!string.IsNullOrWhiteSpace(areaorder))
            {
                if (areaorder == "desc")
                    list = list.OrderByDescending(t => t.Area).ToList();
                else
                    list = list.OrderBy(t => t.Area).ToList();
            }
            if (!string.IsNullOrWhiteSpace(countlogorder))
            {
                if (countlogorder == "desc")
                    list = list.OrderByDescending(t => t.CountLog).ToList();
                else
                    list = list.OrderBy(t => t.CountLog).ToList();
            }

            var model = list.AsEnumerable().ToPagedList(pn, 10);

            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;

            return View(model);*/
            #endregion
        }

        //按督导维度统计督导出差日志
        public ActionResult SuperviserStatistics(int pn = 1, string keyword = null, string area = null, DateTime? starttime = null, DateTime? endtime = null,
                                                string stafforder = null, string countlogorder = null)
        {
            var query = db.View_SuperviserStatisticsTripLog.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(t => t.StaffName.Contains(keyword) ||
                                    t.Mobile.Contains(keyword));
            if (!string.IsNullOrWhiteSpace(area))
                query = query.Where(t => t.Area == area);

            if (starttime.HasValue)
                query = query.Where(t => t.UploadDate >= starttime);
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(t => t.UploadDate < endtime);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Today.AddDays(1);
                starttime = now.AddMonths(-1);
                endtime = now;
                query = query.Where(t => t.UploadDate >= starttime &&
                                       t.UploadDate < endtime);
            }

            //groupby
            var queryX = from q in query.AsEnumerable()
                         group q by new { q.Area, q.StaffName, q.Mobile } into ultimate
                         select new
                         {
                             Area = ultimate.Key.Area,
                             StaffName = ultimate.Key.StaffName,
                             Mobile = ultimate.Key.Mobile,
                             CountLog = ultimate.Sum(q => q.CountLog)
                         };

            if (string.IsNullOrWhiteSpace(stafforder) && string.IsNullOrWhiteSpace(countlogorder))
                queryX = queryX.OrderBy(t => t.StaffName);


            var list = new List<View_SuperviserStatisticsTripLog>();
            foreach (var item in queryX.AsEnumerable())
            {
                var superviser = new View_SuperviserStatisticsTripLog();
                superviser.Area = item.Area;
                superviser.StaffName = item.StaffName;
                superviser.Mobile = item.Mobile;
                superviser.CountLog = item.CountLog;
                list.Add(superviser);
            };

            if (!string.IsNullOrWhiteSpace(stafforder))
            {
                if (stafforder == "desc")
                    list = list.OrderByDescending(t => t.StaffName).ToList();
                else
                    list = list.OrderBy(t => t.StaffName).ToList();
            }
            if (!string.IsNullOrWhiteSpace(countlogorder))
            {
                if (countlogorder == "desc")
                    list = list.OrderByDescending(t => t.CountLog).ToList();
                else
                    list = list.OrderBy(t => t.CountLog).ToList();
            }


            var model = list.AsEnumerable().ToPagedList(pn, 10);
            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;
            ViewBag.area = string.IsNullOrWhiteSpace(area) ? "" : area;

            return View(model);
        }

        //督导出差日志详细
        public ActionResult TripLogDetail(int pn = 1, string area = null, string dm = null, string dudaomz = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var query = db.View_SuperviserTripLog.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dm))
            {
                query = query.Where(t => t.dianming.Contains(dm));
            }
            if (!string.IsNullOrWhiteSpace(dudaomz))
            {
                query = query.Where(t => t.StaffName.Contains(dudaomz));

            }
            if (starttime.HasValue)
            {
                query = query.Where(t => t.uploaddate >= starttime);
                ViewBag.starttime = starttime;
            }
            if (endtime.HasValue)
            {
                ViewBag.endtime = endtime;
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(t => t.uploaddate < endtime);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Now;
                starttime = now.AddMonths(-1);
                endtime = now;
                query = query.Where(t => t.uploaddate >= starttime &&
                                       t.uploaddate < endtime);
            }

            var model = query.OrderByDescending(t => t.uploaddate)
                             .ToPagedList(pn, 10);

            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.dm = string.IsNullOrWhiteSpace(dm) ? inputKeyword : dm;
            ViewBag.dudaomz = string.IsNullOrWhiteSpace(dudaomz) ? inputKeyword : dudaomz;
            ViewBag.area = string.IsNullOrWhiteSpace(area) ? "" : area;

            return View(model);
        }



        #endregion

        #region superviser's upload photos by Lee 20150721
        //按区域维度统计督导上传照片
        public ActionResult AreaPhotosStatistics(int pn = 1, string keyword = null, DateTime? starttime = null, DateTime? endtime = null,
                                                 string areaorder = null, string countlogorder = null)
        {
            var areastr = !string.IsNullOrWhiteSpace(keyword) ? string.Format("AND tmp.Area LIKE '%{0}%'", keyword) : "";
            var orderstr = " tmp.CountLog DESC";
            orderstr = !string.IsNullOrWhiteSpace(areaorder) ? string.Format(" tmp.Area {0}", areaorder) : orderstr;
            orderstr = !string.IsNullOrWhiteSpace(countlogorder) ? string.Format(" tmp.CountLog {1}", countlogorder) : orderstr;

            var starttimestr = "";
            var endtimestr = "";
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Today.AddDays(1);
                starttime = now.AddMonths(-1);
                endtime = now;
            }

            starttimestr = starttime.HasValue ? string.Format("AND UploadDate >= '{0}'", starttime) : "";
            endtimestr = endtime.HasValue ? string.Format("AND UploadDate < '{0}'", Convert.ToDateTime(endtime)) : "";

            var commandText = string.Format(@"SELECT * FROM
                                            (
	                                            SELECT A.AreaName AS Area,ISNULL(SUM(S.CountLog),0) AS CountLog
	                                            FROM Area A
	                                            LEFT JOIN 
	                                            (
		                                            SELECT * 
		                                            FROM View_AreaPhotosStatistics
		                                            WHERE 1 = 1
                                                    {0} {1}
	                                            ) S ON A.AreaName = S.Area
	                                            WHERE  A.Satus = 1
	                                            GROUP BY A.AreaName
                                            )tmp 
                                            WHERE 1=1 {2}
                                            ORDER BY {3}", starttimestr, endtimestr, areastr, orderstr);
            var datasource = SqlHelper.ExecuteDataset(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ToString(), System.Data.CommandType.Text, commandText);

            var model = new List<View_AreaPhotosStatistics>();

            if (datasource != null && datasource.Tables.Count > 0)
            {
                var lists = new List<View_AreaPhotosStatistics>();
                for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
                {
                    lists.Add(new View_AreaPhotosStatistics()
                    {
                        Area = datasource.Tables[0].Rows[i]["Area"].ToString(),
                        CountLog = Int32.Parse(datasource.Tables[0].Rows[i]["CountLog"].ToString())
                    });
                }

                model = lists.AsEnumerable().ToPagedList(pn, 10);
            }

            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;

            return View(model);

            #region todo delete
            /*
            var query = db.View_AreaPhotosStatistics.AsQueryable();
            if (starttime.HasValue)
                query = query.Where(t => t.UploadDate >= starttime);
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(t => t.UploadDate < endtime);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Now;
                starttime = now.AddMonths(-1);
                endtime = now;
                query = query.Where(t => t.UploadDate >= starttime &&
                                       t.UploadDate < endtime);
            }

            //groupby
            var queryY = (from r in db.Area
                          join s in query on r.AreaName equals s.Area into union
                          from v in union.DefaultIfEmpty()
                          select new
                          {
                              Area = r.AreaName,
                              CountLog = v == null ? 0 : v.CountLog
                          }).GroupBy(t => new { t.Area, t.CountLog });


            var queryX = from q in queryY.AsEnumerable()
                         group q by q.Key.Area into ultimate
                         select new
                         {
                             Area = ultimate.Key,
                             CountLog = ultimate.Sum(q => q.Key.CountLog)
                         };

            if (!string.IsNullOrWhiteSpace(keyword))
                queryX = queryX.Where(t => t.Area == keyword);

            queryX.ToList().ForEach(t => new View_AreaPhotosStatistics
            {
                Area = t.Area,
                CountLog = t.CountLog
            });

            if (string.IsNullOrWhiteSpace(areaorder) && string.IsNullOrWhiteSpace(countlogorder))
                queryX = queryX.OrderBy(t => t.Area);

            var list = new List<View_AreaPhotosStatistics>();
            foreach (var item in queryX.AsEnumerable())
            {
                var area = new View_AreaPhotosStatistics();
                area.Area = item.Area;
                area.CountLog = item.CountLog;
                list.Add(area);
            };

            if (!string.IsNullOrWhiteSpace(areaorder))
            {
                if (areaorder == "desc")
                    list = list.OrderByDescending(t => t.Area).ToList();
                else
                    list = list.OrderBy(t => t.Area).ToList();
            }
            if (!string.IsNullOrWhiteSpace(countlogorder))
            {
                if (countlogorder == "desc")
                    list = list.OrderByDescending(t => t.CountLog).ToList();
                else
                    list = list.OrderBy(t => t.CountLog).ToList();
            }

            var model = list.AsEnumerable().ToPagedList(pn, 10);

            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;

            return View(model);*/
            #endregion
        }

        //按督导维度统计督导上传照片
        public ActionResult SuperviserPhotosStatistics(int pn = 1, string keyword = null, string area = null, DateTime? starttime = null, DateTime? endtime = null,
                                                       string stafforder = null, string countlogorder = null)
        {
            var query = db.View_SuperviserPhotosStatistics.AsQueryable();

            if (!string.IsNullOrWhiteSpace(area))
                query = query.Where(t => t.Area == area);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(t => t.StaffName.Contains(keyword) ||
                                    t.Mobile.Contains(keyword));
            if (starttime.HasValue)
                query = query.Where(t => t.UploadDate >= starttime);
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(t => t.UploadDate < endtime);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Today.AddDays(1);
                starttime = now.AddMonths(-1);
                endtime = now;
                query = query.Where(t => t.UploadDate >= starttime && t.UploadDate < endtime);
            }

            //groupby
            var queryX = from q in query.AsEnumerable()
                         group q by new { q.Area, q.StaffName, q.Mobile } into ultimate
                         select new
                         {
                             Area = ultimate.Key.Area,
                             StaffName = ultimate.Key.StaffName,
                             Mobile = ultimate.Key.Mobile,
                             CountLog = ultimate.Sum(q => q.CountLog)
                         };

            if (string.IsNullOrWhiteSpace(stafforder) && string.IsNullOrWhiteSpace(countlogorder))
                queryX = queryX.OrderBy(t => t.StaffName);

            var list = new List<View_SuperviserPhotosStatistics>();
            foreach (var item in queryX.AsEnumerable())
            {
                var superviser = new View_SuperviserPhotosStatistics();
                superviser.Area = item.Area;
                superviser.StaffName = item.StaffName;
                superviser.Mobile = item.Mobile;
                superviser.CountLog = item.CountLog;
                list.Add(superviser);
            };

            if (!string.IsNullOrWhiteSpace(stafforder))
            {
                if (stafforder == "desc")
                    list = list.OrderByDescending(t => t.StaffName).ToList();
                else
                    list = list.OrderBy(t => t.StaffName).ToList();
            }
            if (!string.IsNullOrWhiteSpace(countlogorder))
            {
                if (countlogorder == "desc")
                    list = list.OrderByDescending(t => t.CountLog).ToList();
                else
                    list = list.OrderBy(t => t.CountLog).ToList();
            }


            var model = list.AsEnumerable().ToPagedList(pn, 10);
            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.keyword = string.IsNullOrWhiteSpace(keyword) ? inputKeyword : keyword;
            ViewBag.area = string.IsNullOrWhiteSpace(area) ? "" : area;

            return View(model);
        }
        /// <summary>
        /// 督导上传照片详细
        /// </summary>
        /// <param name="pn">pagesize</param>
        /// <param name="area">区域</param>
        /// <param name="dm">店名</param>
        /// <param name="dudaomz">督导名字</param>
        /// <param name="mainproject">主项目</param>
        /// <param name="childproject">子项目</param>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public ActionResult PhotosDetail(int pn = 1, string area = null, string dm = null, string dudaomz = null,
                                         string mainproject = null, string childproject = null,
                                         DateTime? starttime = null, DateTime? endtime = null)
        {
            var query = db.View_SuperviserUploadPhotos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(dm))
            {
                query = query.Where(t => t.dianming.Contains(dm));
            }
            if (!string.IsNullOrWhiteSpace(dudaomz))
            {
                query = query.Where(t => t.StaffName.Contains(dudaomz));

            }
            if (!string.IsNullOrWhiteSpace(mainproject) && mainproject != "0")
            {
                query = query.Where(t => t.mainproject == mainproject);
            }
            if (!string.IsNullOrWhiteSpace(childproject) && childproject != "0")
            {
                query = query.Where(t => t.childproject == childproject);
            }
            if (starttime.HasValue)
            {
                query = query.Where(t => t.uploaddate >= starttime);
            }
            if (endtime.HasValue)
            {
                endtime = Convert.ToDateTime(endtime).AddDays(1);
                query = query.Where(t => t.uploaddate < endtime);
            }
            //默认近一个月
            if (!starttime.HasValue && !endtime.HasValue)
            {
                var now = DateTime.Today.AddDays(1);
                starttime = now.AddMonths(-1);
                endtime = now;
                query = query.Where(t => t.uploaddate >= starttime &&
                                       t.uploaddate < endtime);
            }

            var model = query.OrderByDescending(t => t.uploaddate)
                             .ToPagedList(pn, 10);

            ViewBag.starttime = starttime.HasValue ? starttime.Value.ToString("yyyy-MM-dd") : "";
            ViewBag.endtime = endtime.HasValue ? endtime.Value.AddDays(-1).ToString("yyyy-MM-dd") : "";
            ViewBag.dm = string.IsNullOrWhiteSpace(dm) ? inputKeyword : dm;
            ViewBag.dudaomz = string.IsNullOrWhiteSpace(dudaomz) ? inputKeyword : dudaomz;
            ViewBag.area = string.IsNullOrWhiteSpace(area) ? "" : area;
            ViewBag.mainproject = string.IsNullOrWhiteSpace(mainproject) ? "0" : mainproject;
            ViewBag.childproject = string.IsNullOrWhiteSpace(childproject) ? "0" : childproject;

            return View(model);
        }

        //批复督导上传照片 by Lee 20150721
        public string tipifupictureupload(int id, string content)
        {
            var model = new PiFuOnPictureUpload { PictureUploadID = id, PiFuContent = content };
            db.PiFuOnPictureUpload.Add(model);
            db.SaveChangesAsync();
            return "";
        }


        #endregion
    }

}