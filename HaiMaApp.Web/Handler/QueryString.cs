using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


    public class QueryString
    {


        public static string GetQueryString(string name)
        {
            if (HttpContext.Current.Request.QueryString[name] != null)
            {
                //return HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString[name]);
                return HttpContext.Current.Request.QueryString[name];
            }
            else
            {
                return "";
            }
        }

        public static string GetUrlReferrer()
        {
            if (HttpContext.Current.Request != null && HttpContext.Current.Request.UrlReferrer != null)
            {
                return HttpContext.Current.Request.UrlReferrer.ToString();
            }
            return "";
        }

        public static object GetSession(string name)
        {
            if (HttpContext.Current.Session[name] != null)
            {
                return HttpContext.Current.Session[name];
            }
            else
            {
                return "";
            }
        }

        public static string GetQuery(HttpContext context, string QueryName)
        {
            return context.Request[QueryName] == null ? "" :
                       context.Request[QueryName].ToString();
        }

        //public static string RetUrl(string returl) 
        //{
        //    if (HttpContext.Current.Request.Url.ToString().ToLower().Contains("webview")) 
        //    { 
        //        return I.Utility.Util.GetConfigByKey("AppMainUrl");
        //    }
        //    return returl;
        //}
    }
