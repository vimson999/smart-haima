
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using X3;

namespace HaiMaApp.Web
{
    public class LoginAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = UtilX3.GetCookie("hmToken"); 
            if (string.IsNullOrWhiteSpace(user))	//沒登陆时
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "home", action = "Contact" }));                
            }
        }
    }
}

