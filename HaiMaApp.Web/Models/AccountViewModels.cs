using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X3;

namespace HaiMaApp.Web.Models
{
    public class LoginViewModel
    {        
        public string UserName { get; set; }
        
        public bool IsSuPerAdmin { get; set; }

        
    }

    public class User
    {
        #region 缓存

        /// <summary>
        /// 当前用户缓存key前缀 E2Online_
        /// Mike 20121115
        /// </summary>
        private static string OnlineCachePrefix = "Online_";

        /// <summary>
        /// 本应用缓存key规则 "E2Online_"{ cookieValue拼接AppKey得到md5值 } ，防止伪造，
        /// 特别注意：这个缓存key不能修改，用于中心同步退出各应用，
        /// Mike 20121121
        /// </summary>
        public static string GetCacheKey(string cookieValue)
        {            
            string cacheKey = OnlineCachePrefix + cookieValue;
            return cacheKey;
        }

        /// <summary>
        /// 根域相同或不同的应用，设置本应用当前登陆用户缓存30分钟，
        /// CookieValue值不能保存在缓存中
        /// Mike 20121122
        /// </summary> 
        public static string SetOnlineCache(string cookieValue, LoginViewModel vm)
        {
            string cacheKey = GetCacheKey(cookieValue);
            if (!string.IsNullOrEmpty(cookieValue) && vm != null)
            {
                //if (AppConfig.IsDebug)//日志
                //    Log.Info("SetOnlineCache() 设置本应用缓存key：" + cookieValue + " 缓存值：" + vm.ToJsonItem());

                //保存到本应用缓存30分钟
                CacheX3.Set(cacheKey, vm, 30, CacheExpiresTypeX3.Absolute);
                return "1";
            }
            return "1";
        }

        /// <summary>
        /// 移除本应用当前用户缓存，用于中心后台调用sso页退出各个根域不同的应用，
        /// Mike 20121122
        /// </summary> 
        public static string RemoveOnlineCache(string cookieValue)
        {
            //string cacheKey = "E2Online_" + token;
            string cacheKey = GetCacheKey(cookieValue);
            CacheX3.Remove(cacheKey);
            return "1";
        }

        #endregion
    }
}