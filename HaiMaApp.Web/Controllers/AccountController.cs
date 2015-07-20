using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using X3;

namespace HaiMaApp.Web.Controllers
{
    public class AccountController : Controller
    {
        HaiMaApp db = new HaiMaApp();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string UserName, string password)
        {
            /*修改逻辑，1.判断是否区域经理 2.判断是否督导   by Lee 20150714*/
            var user = db.SuperAdmin.Where(s => s.username == UserName && s.password == password && s.Status == true).FirstOrDefault();
            if (user != null)
            {
                UtilX3.SetCookie("hmToken", UserName, 24 * 60);
                return RedirectToAction("contact", "Home");
            }

            var userDuDao = db.DuDaoRights.Where(u => u.mobile == UserName && u.Password == password && u.HasRights == true).FirstOrDefault();
            if (userDuDao != null)
            {
                UtilX3.SetCookie("hmToken", UserName, 24 * 60);
                return RedirectToAction("contact", "Home");
            }
            else
            {
                ViewBag.islogin = 0;
            }
            
            return View();
        }

        public ActionResult logout()
        {
            UtilX3.RemoveCookie("hmToken");
            return RedirectToAction("login", "account");
        }
    }
}