using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace DAL
{
    public class Companies
    {
        public ObjectId _id { get; set; }
        public string name { get; set; }
        public string province { get; set; }
        public string city { get; set; }
        public bool useSystem { get; set; }
        public string monitorLevel { get; set; }
        public string status { get; set; }
        public double account { get; set; }
        public string userType { get; set; }
        public DateTime createdOn { get; set; }
        public string companyType { get; set; }

    }

    public class DataService
    {
        /// <summary>
        /// 获取所有经销商的统计建卡信息
        /// </summary>
        public List<Companies> GetCompanies(int page, int pageSize)
        {
            var listBsonDocument = DBHelper.MongoHelper.GetBsonDocumentByQuery("companies", null, page, pageSize, SortBy.Ascending("_id"));
            List<Companies> list = new List<Companies>();
            foreach (var item in listBsonDocument)
            {
                Companies obj = new Companies();
                obj._id = item["_id"] != null ? item["_id"].AsObjectId : new ObjectId();

                if (item.Contains("name"))
                    obj.name = item["name"] != null ? item["name"].AsString : "";

                if (item.Contains("address"))
                {
                    BsonDocument bson = item["address"].AsBsonDocument;

                    if (bson.Contains("province"))
                        obj.province = bson["province"] != null ? bson["province"].AsString : "";
                    if (item.Contains("city"))
                        obj.city = bson["city"] != null ? bson["city"].AsString : "";
                }

                if (item.Contains("monitorLevel"))
                    obj.monitorLevel = item["monitorLevel"] != null ? item["monitorLevel"].AsString : "";
                if (item.Contains("status"))
                    obj.status = item["status"] != null ? item["status"].AsString : "";
                if (item.Contains("userType"))
                    obj.userType = item["userType"] != null ? item["userType"].AsString : "";
                if (item.Contains("companyType"))
                    obj.companyType = item["companyType"] != null ? item["companyType"].AsString : "";

                if (item.Contains("useSystem"))
                    obj.useSystem = item["useSystem"] != null && item["useSystem"].AsBoolean;
                if (item.Contains("account") && item["account"] != null)
                {
                    if (item["account"].IsInt32)
                        obj.account = item["account"].AsInt32;
                    if (item["account"].IsDouble)
                        obj.account = item["account"].AsDouble;
                }
                if (item.Contains("createdOn"))
                    obj.createdOn = item["createdOn"] != null ? item["createdOn"].AsDateTime : DateTime.MinValue;

                list.Add(obj);
            }
            return list;
        }
    }
    //public class DAL
    //{
    //    //表名
    //    private const string companiesCollectionName = "companies";
    //    private const string customersCollectionName = "customers";
    //    private const string usersCollectionName = "users";
    //    public const string customersHistoryCollectionName = "customerHistory";

    //    private int GetDataCount = Int32.MaxValue;

    //    //库名
    //    private static string database_Default = ConfigurationManager.AppSettings["Database_mongoDB_Ant"];
    //    private static string ConnectionString_Write = System.Configuration.ConfigurationManager.AppSettings["ConnectionString_mongoDB"];
    //    private static string ConnectionString_Read = System.Configuration.ConfigurationManager.AppSettings["ConnectionString_mongoDBRead"];
    //    DBHelper MongoHelper = null;
    //    public DAL()
    //    {
    //        MongoHelper = new DBHelper(ConnectionString_Write, ConnectionString_Read, database_Default);
    //    }


    //    /// <summary>
    //    /// 获取所有经销商的统计建卡信息
    //    /// </summary>
    //    public List<Companies> GetCompanies(int page, int pageSize)
    //    {
    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery(companiesCollectionName, null, page, pageSize, SortBy.Ascending("_id"));
    //        List<Companies> list = new List<Companies>();
    //        foreach (var item in listBsonDocument)
    //        {
    //            Companies obj = new Companies();
    //            obj._id = item["_id"] != null ? item["_id"].AsObjectId : new ObjectId();

    //            if (item.Contains("name"))
    //                obj.name = item["name"] != null ? item["name"].AsString : "";

    //            if (item.Contains("address"))
    //            {
    //                BsonDocument bson = item["address"].AsBsonDocument;

    //                if (bson.Contains("province"))
    //                    obj.province = bson["province"] != null ? bson["province"].AsString : "";
    //                if (item.Contains("city"))
    //                    obj.city = bson["city"] != null ? bson["city"].AsString : "";
    //            }

    //            if (item.Contains("monitorLevel"))
    //                obj.monitorLevel = item["monitorLevel"] != null ? item["monitorLevel"].AsString : "";
    //            if (item.Contains("status"))
    //                obj.status = item["status"] != null ? item["status"].AsString : "";
    //            if (item.Contains("userType"))
    //                obj.userType = item["userType"] != null ? item["userType"].AsString : "";
    //            if (item.Contains("companyType"))
    //                obj.companyType = item["companyType"] != null ? item["companyType"].AsString : "";

    //            if (item.Contains("useSystem"))
    //                obj.useSystem = item["useSystem"] != null && item["useSystem"].AsBoolean;
    //            if (item.Contains("account") && item["account"] != null)
    //            {
    //                if (item["account"].IsInt32)
    //                    obj.account = item["account"].AsInt32;
    //                if (item["account"].IsDouble)
    //                    obj.account = item["account"].AsDouble;
    //            }
    //            if (item.Contains("createdOn"))
    //                obj.createdOn = item["createdOn"] != null ? item["createdOn"].AsDateTime : DateTime.MinValue;

    //            list.Add(obj);
    //        }
    //        return list;
    //    }

    //    public List<Company> GetALLNewJingXiaoShangData_FromFile()
    //    {
    //        var path = System.AppDomain.CurrentDomain.BaseDirectory;
    //        path = Path.Combine(path, "Model");
    //        path = Path.Combine(path, "Companies.json");
    //        var text = File.ReadAllText(path);
    //        JavaScriptSerializer serializer = new JavaScriptSerializer();
    //        var tempObj = serializer.Deserialize<JingXiaoShangCollection>(text);
    //        return tempObj.companies.ToList();
    //    }

    //    public List<string[]> LoadJingXiaoShangDic()
    //    {
    //        var path = System.AppDomain.CurrentDomain.BaseDirectory;
    //        path = Path.Combine(path, "data");
    //        path = Path.Combine(path, "dic");
    //        path = Path.Combine(path, "JingXiaoShangDic.txt");
    //        var text = File.ReadAllText(path);

    //        var arr = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    //        List<string[]> list = new List<string[]>();
    //        foreach (var row in arr)
    //        {
    //            //数据问题 报了bug！
    //            //if (row.Contains("54bb3a02600700d0c2c44be5"))
    //            //{
    //            //    var i = 0;
    //            //}
    //            var rowArr = row.Split(new string[] { "\t" }, StringSplitOptions.None);
    //            list.Add(rowArr);
    //        }
    //        return list;
    //    }

    //    public List<Company> GetALLNewJingXiaoShangData_FromDB()
    //    {
    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery(companiesCollectionName, null, 1, GetDataCount, SortBy.Ascending("_id"));

    //        List<string[]> jingxiaoshangStrArray = LoadJingXiaoShangDic();

    //        List<Company> list = new List<Company>();
    //        foreach (var item in listBsonDocument)
    //        {
    //            Company obj = new Company();
    //            obj.CompanyID = item["_id"] != null ? item["_id"].AsObjectId.ToString() : "";

    //            //get jingxiaoshang other infos
    //            var arrOtherInfo = jingxiaoshangStrArray.FirstOrDefault(n => n[0] == obj.CompanyID);
    //            if (arrOtherInfo != null)
    //            {
    //                obj.Brand = arrOtherInfo[2];
    //                obj.Region = arrOtherInfo[3];
    //                obj.Project = arrOtherInfo[6];
    //                obj.KaiFaFangShi = arrOtherInfo[7];
    //                obj.KaiTongMoKuai = arrOtherInfo[9];
    //                obj.PeiXunQingKuang = arrOtherInfo[10];
    //                obj.KaiFaRenYuan = arrOtherInfo[11];
    //                obj.XiaoShouFuZeRen = arrOtherInfo[12];
    //                obj.FuWuRenYuan = arrOtherInfo[13];
    //                obj.FuWuFuZeRen = arrOtherInfo[14];

    //                obj.province = arrOtherInfo[4];
    //                obj.city = arrOtherInfo[5];



    //                if (item.Contains("name"))
    //                    obj.name = item["name"] != null ? item["name"].AsString : "";

    //                if (item.Contains("address"))
    //                {
    //                    BsonDocument bson = item["address"].AsBsonDocument;

    //                    if (bson.Contains("province"))
    //                        obj.province = bson["province"] != null ? bson["province"].AsString : "";
    //                    if (item.Contains("city"))
    //                        obj.city = bson["city"] != null ? bson["city"].AsString : "";
    //                }

    //                if (item.Contains("monitorLevel"))
    //                    obj.monitorLevel = item["monitorLevel"] != null ? item["monitorLevel"].AsString : "";
    //                if (item.Contains("status"))
    //                    obj.status = item["status"] != null ? item["status"].AsString : "";
    //                if (item.Contains("userType"))
    //                    obj.userType = item["userType"] != null ? item["userType"].AsString : "";
    //                if (item.Contains("companyType"))
    //                    obj.companyType = item["companyType"] != null ? item["companyType"].AsString : "";

    //                if (item.Contains("useSystem"))
    //                    obj.useSystem = item["useSystem"] != null && item["useSystem"].AsBoolean;
    //                if (item.Contains("account") && item["account"] != null)
    //                {
    //                    if (item["account"].IsInt32)
    //                        obj.account = item["account"].AsInt32;
    //                    if (item["account"].IsDouble)
    //                        obj.account = item["account"].AsDouble;
    //                }
    //                if (item.Contains("createdOn"))
    //                    obj.createdOn = item["createdOn"] != null ? item["createdOn"].AsDateTime.ToString("yyyy-MM-dd") : "";
    //                else
    //                    obj.createdOn = "";



    //                list.Add(obj);
    //            }
    //        }
    //        return list;
    //    }

    //    public List<User> GetALLNewUsersData()
    //    {
    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery(usersCollectionName, Query.And(Query.EQ("isValid", true), Query.EQ("roles", "顾问")), 1, GetDataCount, SortBy.Ascending("companyIds"));
    //        List<User> listUsers = new List<User>();
    //        foreach (var item in listBsonDocument)
    //        {
    //            User obj = new User();
    //            obj.UserID = item["_id"].ToString();
    //            if (item.Contains("company"))
    //            {
    //                var bson = item["company"].AsBsonDocument;
    //                if (bson.Contains("_id"))
    //                {
    //                    obj.CompanyId = bson["_id"].AsString;
    //                }
    //                else
    //                {
    //                    obj.CompanyId = " ";
    //                }
    //                if (bson.Contains("name"))
    //                {
    //                    obj.CompanyName = bson["name"].AsString;
    //                }
    //                else
    //                {
    //                    obj.CompanyName = " ";
    //                }
    //            }
    //            else
    //            {
    //                obj.CompanyId = " ";
    //                obj.CompanyName = " ";
    //            }
    //            if (item.Contains("profile"))
    //            {
    //                var childBson = item["profile"].AsBsonDocument;
    //                if (childBson.Contains("name"))
    //                    obj.UserName = childBson["name"].AsString;
    //                else
    //                {
    //                    obj.UserName = " ";
    //                }
    //            }
    //            else
    //            {
    //                obj.UserName = " ";
    //            }
    //            if (item.Contains("createdOn"))
    //            {
    //                obj.RuZhiSTime = item["createdOn"].AsDateTime.ToString("yyyy-MM-dd");
    //            }
    //            listUsers.Add(obj);
    //        }
    //        return listUsers;
    //    }

    //    /// <summary>
    //    /// 统计每个经销商的数据
    //    /// </summary>
    //    public List<Company> GetAllCustomersStatisticsData(int page, int pageSize, List<User> listUsers)
    //    {
    //        //因为要计算累计值，所以这里使用的是从每个月的第一天开始到 今天结束的时候的数据
    //        var setdate =
    //            DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-01" +
    //                           " 07:59:59");
    //        var enddate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 07:59:59");

    //        //for test data
    //        //setdate = DateTime.Parse("2015-04-13 07:59:59");
    //        //enddate = DateTime.Parse("2015-04-14 07:59:59");

    //        /*
    //         // var agg1 = db.customers.count({"companyIds" : b._id,"createdOn":{$gt:new Date(setdate),$lt:new  Date(enddate)}})
    //        // var agg2 = db.customerHistory.count({"companyIds" : b._id,"reason" : {$in : ["去电","来电"]},"createdOn":{$gt:new Date(setdate),$lt:new  Date(enddate)}})
    //        // var agg3 = db.customerHistory.count({"companyIds" : b._id,"reason" : "到店","createdOn":{$gt:new Date(setdate),$lt:new  Date(enddate)}})
    //         */

    //        List<Company> listCompany = GetALLNewJingXiaoShangData_FromDB();

    //        //for test only
    //        //pageSize = 500;

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery(customersCollectionName,
    //            Query.And(Query.GT("createdOn", setdate), Query.LT("createdOn", enddate)), page, pageSize,
    //            SortBy.Ascending("_id"));


    //        var listCustomersHistory = MongoHelper.GetBsonDocumentByQuery(customersHistoryCollectionName,
    //            Query.And(Query.GT("createdOn", setdate), Query.LT("createdOn", enddate)), page, pageSize,
    //            SortBy.Ascending("_id"));



    //        foreach (Company item in listCompany)
    //        {
    //            var currentId = item.CompanyID;

    //            var listBelongsToThisCompany = listBsonDocument.Where(n =>
    //            {
    //                if (n.Contains("companyIds") && n["companyIds"].AsBsonArray.Contains(currentId))
    //                    return true;
    //                return false;
    //            }).ToList();

    //            var listCustoersBelongsToThisCompany = listCustomersHistory.Where(n =>
    //            {
    //                if (n.Contains("companyIds") && n["companyIds"].AsBsonArray.Contains(currentId))
    //                    return true;
    //                return false;
    //            }).ToList();

    //            /*
    //            item.AccJianKaCount =
    //                listBsonDocument.Where(m => m.Contains("companyIds"))
    //                    .Count(n => n["companyIds"].AsBsonArray.Contains(currentId));
    //             */

    //            item.AccJianKaCount = listBelongsToThisCompany.Count();

    //            item.AvgJianKaCount = item.AccJianKaCount * 1.0 / DateTime.Now.Day;
    //            item.AccDianHuaCount =
    //                listCustoersBelongsToThisCompany.Where(m => m.Contains("reason"))
    //                    .Count(n => (n["reason"].ToString().Contains("去电") || n["reason"].ToString().Contains("来电")));
    //            item.AvgDianHuaCount = item.AccDianHuaCount * 1.0 / DateTime.Now.Day;
    //            item.AccDaoDianCount =
    //                listCustoersBelongsToThisCompany.Where(m => m.Contains("reason")).Count(n => n["reason"].ToString().Contains("到店"));
    //            item.AvgDaoDianCount = item.AccDaoDianCount * 1.0 / DateTime.Now.Day;

    //            //积分天数
    //            item.JiFenTianShu = item.ZhuCeShiJian <=
    //                                  DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1")
    //                ? DateTime.Now.Day
    //                : (LastDayOfMonth(DateTime.Now) - item.ZhuCeShiJian).Days + 1;



    //            //当前月累计顾问数
    //            item.AccGuWenCount = listBelongsToThisCompany.GroupBy(n =>
    //            {
    //                var ownerbd = n["owner"].AsBsonDocument;
    //                return ownerbd["_id"].AsString;
    //            }).Sum(group => group.Key.Count());
    //            item.AvgGuWenCount = item.AccGuWenCount / item.JiFenTianShu;
    //            item.AccJiFen = listUsers.Where(n => n.CompanyId == currentId).Sum(n => n.AccJiFen);

    //            if (item.AvgGuWenCount != 0.0)
    //            {
    //                item.AvgJiFen = item.AccJiFen / item.AvgGuWenCount;
    //            }
    //            else
    //            {
    //                item.AvgJiFen = 0.0;
    //            }
    //            item.Classify = GetUserClassify(item.AvgJiFen);
    //        }

    //        return listCompany.OrderByDescending(n => n.AccJianKaCount).ToList();
    //    }

    //    /// <summary>
    //    /// 统计每个销售顾问的数据
    //    /// </summary>
    //    public List<User> GetAllSalesRepresentiveStatisticsData(int page, int pageSize)
    //    {
    //        //因为要计算累计值，所以这里使用的是从每个月的第一天开始到 今天结束的时候的数据
    //        var setdate = DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-01" + " 07:59:59");
    //        var enddate = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " 07:59:59");

    //        List<User> listUsers = GetALLNewUsersData();

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery(customersCollectionName, Query.And(Query.GT("createdOn", setdate), Query.LT("createdOn", enddate)), page, pageSize, SortBy.Ascending("companyIds"));
    //        //todo:here to continue!!!
    //        foreach (User user in listUsers)
    //        {
    //            var uid = user.UserID;
    //            var listBelongToThisUser = listBsonDocument.Where(m => m.Contains("owner")).Where(n =>
    //            {
    //                var bd = n["owner"].AsBsonDocument;
    //                if (bd.Contains("_id"))
    //                {
    //                    if (bd["_id"].AsString == uid)
    //                        return true;
    //                }
    //                return false;
    //            }).ToList();
    //            user.AccJianKaCount = listBelongToThisUser.Count();
    //            user.AvgJianKaCount = user.AccJianKaCount * 1.0 / DateTime.Now.Day;
    //            user.AccDianHuaCount = listBelongToThisUser.Where(m => m.Contains("reason")).Count(n => (n["reason"].ToString().Contains("去电") || n["reason"].ToString().Contains("来电")));
    //            user.AvgDianHuaCount = user.AccDianHuaCount * 1.0 / DateTime.Now.Day;
    //            user.AccDaoDianCount = listBelongToThisUser.Where(m => m.Contains("reason")).Count(n => n["reason"].ToString().Contains("到店"));
    //            user.AvgDaoDianCount = user.AccDaoDianCount * 1.0 / DateTime.Now.Day;

    //            /*
    //                销顾当月累计活动天数：当月范围内销顾存在建卡/到店/电话任一操作之一的活动天数总和。
    //             */
    //            user.AccHuoDongTianShu = listBelongToThisUser.Count(n =>
    //            {
    //                if (n.Contains("reason") && n["reason"].ToString().Contains("到店"))
    //                    return true;
    //                if (n.Contains("reason") && (n["reason"].ToString().Contains("来电") || n["reason"].ToString().Contains("去电")))
    //                    return true;

    //                return false;
    //            });
    //            /*
    //             销顾应活动天数（根据销顾入职时间不同，销顾应活动天数将存在两种情况）：
    //                    情况1：销顾开通软件时间小于或等于本月首日
    //                            销顾应活动天数=本月实际自然天数
    //                    情况2：销顾开通软件时间大于本月首日
    //                            销顾应活动天数=本月最末一日 – 销顾开通软件日期 + 1
    //             */
    //            user.HuoDongTianShu = DateTime.Parse(user.RuZhiSTime) <=
    //                                  DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month + "-1")
    //                ? DateTime.Now.Day
    //                : (LastDayOfMonth(DateTime.Now) - DateTime.Parse(user.RuZhiSTime)).Days + 1;
    //            user.HuoDongLv = user.AccHuoDongTianShu * 1.0 / user.HuoDongTianShu;

    //            user.JiFenTianShu = user.HuoDongTianShu;


    //            //统计所有建卡积分之和
    //            user.AccJianKaFen = listBelongToThisUser.GroupBy(n => n["createdOn"].AsDateTime).Sum(group =>
    //            {
    //                var count = group.Count();
    //                if (count <= 5)
    //                {
    //                    return 0.4 * count;
    //                }
    //                else
    //                {
    //                    return 2.0;
    //                }
    //            });

    //            //统计所有商谈积分之和
    //            user.AccDaoDianFen = listBelongToThisUser.Where(n =>
    //            {
    //                if (n.Contains("reson") && n["reason"].ToString().Contains("到店"))
    //                {
    //                    return true;
    //                }
    //                return false;
    //            }).GroupBy(n => n["createdOn"].AsDateTime).Sum(group =>
    //            {
    //                var count = group.Count();
    //                if (count <= 5)
    //                {
    //                    return 0.4 * count;
    //                }
    //                else
    //                {
    //                    return 2.0;
    //                }
    //            });

    //            //统计所有电话积分之和
    //            user.AccDianHuaFen = listBelongToThisUser.Where(n =>
    //            {
    //                if (n.Contains("reason") && (n["reason"].ToString().Contains("来电") || n["reason"].ToString().Contains("去电")))
    //                {
    //                    return true;
    //                }
    //                return false;
    //            }).GroupBy(n => n["createdOn"].AsDateTime).Sum(group =>
    //            {
    //                var count = group.Count();
    //                if (count <= 10)
    //                {
    //                    return 0.6 * count;
    //                }
    //                else
    //                {
    //                    return 6.0;
    //                }
    //            });

    //            user.AccJiFen = user.AccJianKaFen + user.AccDaoDianFen + user.AccDianHuaFen;
    //            user.AvgJiFen = user.AccJiFen / user.JiFenTianShu;

    //            user.ZongHeJiFen = user.AvgJiFen * user.HuoDongLv;
    //            user.Classify = GetUserClassify(user.ZongHeJiFen);
    //        }

    //        return listUsers.OrderByDescending(n => n.AccJianKaCount).ToList();
    //    }
    //    public List<DataUser> GetAllTheEmployeesDataOfThisCompany(string companyId, string date)
    //    {
    //        var paramDate = DateTime.Parse(date);
    //        var setdate = DateTime.Parse(paramDate.Year + "-" + paramDate.Month + "-" + paramDate.Day + " 00:00:00");
    //        var enddate = DateTime.Parse(paramDate.Year + "-" + paramDate.Month + "-" + paramDate.Day + " 23:59:59");

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("UserStat", Query.And(Query.And(Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)), Query.EQ("CompanyId", companyId)), 1, Int32.MaxValue, SortBy.Ascending("CompanyId"));
    //        List<DataUser> listDataUsers = new List<DataUser>();


    //        //统计每个员工的数据
    //        listBsonDocument.Where(m => m.Contains("UserName"))
    //            .Where(n => n.Contains("UserName"))
    //            //.GroupBy(k =>k["UserName"].AsString)
    //            .ToList().ForEach(c =>
    //            {
    //                var dataUser = new DataUser();
    //                dataUser.username = c["UserName"].AsString;
    //                dataUser.DianHuaCount = c["AccDianHuaCount"].AsInt32;
    //                dataUser.DaoDianCount = c["AccDaoDianCount"].AsInt32;
    //                dataUser.JianKaCount = c["AccJianKaCount"].AsInt32;
    //                dataUser.ShangTanCount = Math.Max(dataUser.JianKaCount, Math.Max(dataUser.DianHuaCount, dataUser.DaoDianCount));

    //                listDataUsers.Add(dataUser);
    //            });



    //        return listDataUsers;
    //    }


    //    public List<User> GetAllGuWenDataOfWhichDay(int page, int pageSize, string day)
    //    {
    //        var date = DateTime.Parse(day);
    //        var setdate = DateTime.Parse(date.Year + "-" + date.Month + "-" + date.Day + " 00:00:00");
    //        var enddate = DateTime.Parse(date.Year + "-" + date.Month + "-" + date.Day + " 23:59:59");
    //        List<User> listUser = new List<User>();
    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("UserStat", Query.And(Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)), page, pageSize, SortBy.Ascending("CompanyName"));
    //        foreach (var item in listBsonDocument)
    //        {
    //            User user = new User()
    //            {
    //                UserID = item.Contains("UserID") ? item["UserID"].AsString : "",
    //                UserName = item.Contains("UserName") ? item["UserName"].AsString : "",
    //                CompanyId = item.Contains("CompanyId") ? item["CompanyId"].AsString : "",
    //                CompanyName = item.Contains("CompanyName") ? item["CompanyName"].AsString : "",
    //                AccJianKaCount = item.Contains("AccJianKaCount") ? item["AccJianKaCount"].AsInt32 : 0,
    //                AvgJianKaCount = item.Contains("AvgJianKaCount") ? item["AvgJianKaCount"].AsDouble : 0.0,
    //                AccDaoDianCount = item.Contains("AccDaoDianCount") ? item["AccDaoDianCount"].AsInt32 : 0,
    //                AvgDaoDianCount = item.Contains("AvgDaoDianCount") ? item["AvgDaoDianCount"].AsDouble : 0,
    //                AccDianHuaCount = item.Contains("AccDianHuaCount") ? item["AccDianHuaCount"].AsInt32 : 0,
    //                AvgDianHuaCount = item.Contains("AvgDianHuaCount") ? item["AvgDianHuaCount"].AsDouble : 0,

    //                RuZhiSTime = item.Contains("RuZhiSTime") ? item["RuZhiSTime"].AsDateTime.ToString("yyyy-MM-dd") : "",
    //                AccJiFen = item.Contains("AccJiFen") ? item["AccJiFen"].AsDouble : 0,
    //                AvgJiFen = item.Contains("AvgJiFen") ? item["AvgJiFen"].AsDouble : 0,
    //                JiFenTianShu = item.Contains("JiFenTianShu") ? item["JiFenTianShu"].AsInt32 : 0,

    //                AccHuoDongTianShu = item.Contains("AccHuoDongTianShu") ? item["AccHuoDongTianShu"].AsInt32 : 0,
    //                HuoDongTianShu = item.Contains("HuoDongTianShu") ? item["HuoDongTianShu"].AsInt32 : 0,

    //                Classify = item.Contains("Classify") ? (ClassifyEnum)Int32.Parse(item["Classify"].ToString()) : ClassifyEnum.NotQualified,
    //                HuoDongLv = item.Contains("HuoDongLv") ? item["HuoDongLv"].AsDouble : 0.0,
    //                ZongHeJiFen = item.Contains("ZongHeJiFen") ? item["ZongHeJiFen"].AsDouble : 0.0,
    //                AccDaoDianFen = item.Contains("AccDaoDianFen") ? item["AccDaoDianFen"].AsDouble : 0.0,
    //                AccDianHuaFen = item.Contains("AccDianHuaFen") ? item["AccDianHuaFen"].AsDouble : 0.0,
    //                AccJianKaFen = item.Contains("AccJianKaFen") ? item["AccJianKaFen"].AsDouble : 0.0,
    //            };
    //            listUser.Add(user);
    //        }

    //        return listUser;
    //    }
    //    public List<Company> GetAllJingXiaoShangDataOfWhichDay(int page, int pageSize, string day)
    //    {
    //        var date = DateTime.Parse(day);
    //        var setdate = DateTime.Parse(date.Year + "-" + date.Month + "-" + date.Day + " 00:00:00");
    //        var enddate = DateTime.Parse(date.Year + "-" + date.Month + "-" + date.Day + " 23:59:59");

    //        List<Company> listCompany = new List<Company>();

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("CompanyStat", Query.And(Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)), page, pageSize, SortBy.Ascending("Brand"));
    //        foreach (var item in listBsonDocument)
    //        {
    //            Company company = new Company()
    //            {
    //                name = item.Contains("name") ? item["name"].AsString : "",
    //                CompanyID = item.Contains("CompanyID") ? item["CompanyID"].AsString : "",
    //                DayOfMonth = item.Contains("DayOfMonth") ? item["DayOfMonth"].AsInt32 : 0,

    //                AccJianKaCount = item.Contains("AccJianKaCount") ? item["AccJianKaCount"].AsInt32 : 0,
    //                AvgJianKaCount = item.Contains("AvgJianKaCount") ? item["AvgJianKaCount"].AsDouble : 0.0,
    //                AccDaoDianCount = item.Contains("AccDaoDianCount") ? item["AccDaoDianCount"].AsInt32 : 0,
    //                AvgDaoDianCount = item.Contains("AvgDaoDianCount") ? item["AvgDaoDianCount"].AsDouble : 0,
    //                AccDianHuaCount = item.Contains("AccDianHuaCount") ? item["AccDianHuaCount"].AsInt32 : 0,
    //                AvgDianHuaCount = item.Contains("AvgDianHuaCount") ? item["AvgDianHuaCount"].AsDouble : 0,

    //                AccJiFen = item.Contains("AccJiFen") ? item["AccJiFen"].AsDouble : 0,
    //                AvgJiFen = item.Contains("AvgJiFen") ? item["AvgJiFen"].AsDouble : 0,

    //                AccGuWenCount = item.Contains("AccGuWenCount") ? item["AccGuWenCount"].AsDouble : 0,
    //                AvgGuWenCount = item.Contains("AvgGuWenCount") ? item["AvgGuWenCount"].AsDouble : 0,

    //                Classify = item.Contains("Classify") ? (ClassifyEnum)Int32.Parse(item["Classify"].ToString()) : ClassifyEnum.NotQualified,

    //                JiFenTianShu = item.Contains("JiFenTianShu") ? item["JiFenTianShu"].AsInt32 : 0,
    //                ZhuCeShiJian = item.Contains("ZhuCeShiJian") ? item["ZhuCeShiJian"].AsDateTime : DateTime.MinValue,

    //                province = item.Contains("province") ? item["province"].AsString : "",
    //                city = item.Contains("city") ? item["city"].AsString : "",
    //                useSystem = item.Contains("useSystem") ? item["useSystem"].AsBoolean : false,
    //                monitorLevel = item.Contains("monitorLevel") ? item["monitorLevel"].AsString : "",
    //                status = item.Contains("status") ? item["status"].AsString : "",
    //                account = item.Contains("account") ? item["account"].AsDouble : 0.0,
    //                userType = item.Contains("userType") ? item["userType"].AsString : "",
    //                createdOn = item.Contains("createdOn") ? item["createdOn"].AsDateTime.ToString("yyyy-MM-dd") : "",
    //                companyType = item.Contains("companyType") ? item["companyType"].AsString : "",
    //                Brand = item.Contains("Brand") ? item["Brand"].AsString : "",
    //                Region = item.Contains("Region") ? item["Region"].AsString : "",
    //                Project = item.Contains("Project") ? item["Project"].AsString : "",
    //                FuWuFuZeRen = item.Contains("FuWuFuZeRen") ? item["FuWuFuZeRen"].AsString : "",
    //                FuWuRenYuan = item.Contains("FuWuRenYuan") ? item["FuWuRenYuan"].AsString : "",
    //                KaiFaFangShi = item.Contains("KaiFaFangShi") ? item["KaiFaFangShi"].AsString : "",
    //                KaiFaRenYuan = item.Contains("KaiFaRenYuan") ? item["KaiFaRenYuan"].AsString : "",
    //                KaiTongMoKuai = item.Contains("KaiTongMoKuai") ? item["KaiTongMoKuai"].AsString : "",
    //                PeiXunQingKuang = item.Contains("PeiXunQingKuang") ? item["PeiXunQingKuang"].AsString : "",
    //                XiaoShouFuZeRen = item.Contains("XiaoShouFuZeRen") ? item["XiaoShouFuZeRen"].AsString : "",
    //            };
    //            listCompany.Add(company);
    //        }

    //        return listCompany.ToList();
    //    }
    //    public LineChartData GetLineChartDataOfThisCompany(string companyId, string date)
    //    {
    //        //+ " 07:59:59"
    //        var setdate =
    //            DateTime.Parse(DateTime.Now.Year + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "-01" + " 00:00:00");
    //        var enddate = DateTime.Parse(DateTime.Parse(date).ToString("yyyy-MM-dd") + " 23:59:59");

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("CompanyStat",
    //            Query.And(Query.And(Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)),
    //                Query.EQ("CompanyID", companyId)), 1, Int32.MaxValue, SortBy.Ascending("CompanyID"));

    //        LineChartData data = new LineChartData();
    //        List<string> keysList = new List<string>();
    //        for (int i = 1; i <= enddate.Day; i++)
    //        {
    //            keysList.Add(DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + i.ToString().PadLeft(2, '0'));

    //            //按当天的时间降序排列后获取第一条记录 
    //            var obj = listBsonDocument.Where(m => m["StatisticDateTime"].AsDateTime.ToString("yyyy-MM-dd") == (enddate.Year + "-" + enddate.Month.ToString().PadLeft(2, '0') + "-" + i.ToString().PadLeft(2, '0'))).OrderByDescending(n => n["StatisticDateTime"].AsDateTime).FirstOrDefault();

    //            if (data.JianKaArr == null)
    //                data.JianKaArr = new int[enddate.Day];
    //            if (data.DaoDianArr == null)
    //                data.DaoDianArr = new int[enddate.Day];
    //            if (data.DianHuaArr == null)
    //                data.DianHuaArr = new int[enddate.Day];
    //            if (data.ShangTanArr == null)
    //                data.ShangTanArr = new int[enddate.Day];

    //            if (i == 1)
    //            {
    //                data.JianKaArr[i - 1] = obj != null ? obj["AccJianKaCount"].AsInt32 : 0;
    //                data.DaoDianArr[i - 1] = obj != null ? obj["AccDaoDianCount"].AsInt32 : 0;
    //                data.DianHuaArr[i - 1] = obj != null ? obj["AccDianHuaCount"].AsInt32 : 0;
    //                data.ShangTanArr[i - 1] = Math.Max(data.DianHuaArr[i - 1], Math.Max(data.JianKaArr[i - 1], data.DaoDianArr[i - 1]));
    //            }
    //            else
    //            {
    //                //获取前一天的数据
    //                var previousDay = listBsonDocument.Where(m => m["StatisticDateTime"].AsDateTime.ToString("yyyy-MM-dd") == (enddate.Year + enddate.Month.ToString().PadLeft(2, '0') + (i - 1).ToString().PadLeft(2, '0'))).OrderByDescending(n => n["StatisticDateTime"].AsDateTime).FirstOrDefault();
    //                if (previousDay != null)
    //                {
    //                    if (obj != null)
    //                    {
    //                        data.JianKaArr[i - 1] = obj["AccJianKaCount"].AsInt32 - previousDay["AccJianKaCount"].AsInt32;
    //                        data.DaoDianArr[i - 1] = obj["AccDaoDianCount"].AsInt32 - previousDay["AccDaoDianCount"].AsInt32;
    //                        data.DianHuaArr[i - 1] = obj["AccDianHuaCount"].AsInt32 - previousDay["AccDianHuaCount"].AsInt32;
    //                        data.ShangTanArr[i - 1] = Math.Max(data.DianHuaArr[i - 1], Math.Max(data.JianKaArr[i - 1], data.DaoDianArr[i - 1]));
    //                    }
    //                    else
    //                    {
    //                        data.JianKaArr[i - 1] = 0;
    //                        data.DaoDianArr[i - 1] = 0;
    //                        data.DianHuaArr[i - 1] = 0;
    //                        data.ShangTanArr[i - 1] = 0;
    //                    }
    //                }
    //                else
    //                {
    //                    if (obj != null)
    //                    {
    //                        data.JianKaArr[i - 1] = obj["AccJianKaCount"].AsInt32;
    //                        data.DaoDianArr[i - 1] = obj["AccDaoDianCount"].AsInt32;
    //                        data.DianHuaArr[i - 1] = obj["AccDianHuaCount"].AsInt32;
    //                        data.ShangTanArr[i - 1] = Math.Max(data.DianHuaArr[i - 1], Math.Max(data.JianKaArr[i - 1], data.DaoDianArr[i - 1]));
    //                    }
    //                    else
    //                    {
    //                        data.JianKaArr[i - 1] = 0;
    //                        data.DaoDianArr[i - 1] = 0;
    //                        data.DianHuaArr[i - 1] = 0;
    //                        data.ShangTanArr[i - 1] = 0;
    //                    }
    //                }
    //            }
    //        }
    //        data.LegendArr = keysList.ToArray();

    //        //统计整个公司的数据 包括 建卡总量，到店总量。。这样的数据
    //        //var result = listBsonDocument.Where(m => m.Contains("StatisticDateTime"))
    //        //    .GroupBy(k =>
    //        //    {
    //        //        var dt = k["StatisticDateTime"].AsDateTime;
    //        //        return dt.ToString("MM-dd");
    //        //    }).Select(group => new
    //        //    {
    //        //        key = @group.Key,
    //        //        DianHua = @group.Count(
    //        //            n => (n.Contains("reason") &&
    //        //                  (n["reason"].ToString().Contains("来电") || n["reason"].ToString().Contains("去电")))),
    //        //        ShangTan = group.Count(
    //        //            n => (n.Contains("reason") && (n["reason"].ToString().Contains("来电") || n["reason"].ToString().Contains("去电") || n["reason"].ToString().Contains("到店")))),
    //        //        DaoDian = group.Count(
    //        //            n => (n.Contains("reason") && n["reason"].ToString().Contains("到店"))),
    //        //        JianKa = group.Count()
    //        //    }).ToList();


    //        return data;
    //    }

    //    private ClassifyEnum GetUserClassify(double p)
    //    {
    //        if (p >= 0 && p < 0.21)
    //            return ClassifyEnum.NotQualified;
    //        if (p >= 0.21 && p < 0.7)
    //            return ClassifyEnum.LowestQualified;
    //        if (p >= 0.7 && p < 2.94)
    //            return ClassifyEnum.Qualified;
    //        if (p >= 2.94 && p < 10)
    //            return ClassifyEnum.Excellent;

    //        return ClassifyEnum.Excellent;
    //    }

    //    private DateTime LastDayOfMonth(DateTime datetime)
    //    {
    //        return datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1);
    //    }


    //    //private List<JiTuan> GetAllJiTuan()
    //    //{
    //    //    var listBsonDocument = MongoHelper.GetBsonDocumentByQuery(companiesCollectionName,
    //    //        Query.And(Query.And(Query.GT("createdOn", setdate), Query.LT("createdOn", enddate)),
    //    //            Query.EQ("companyIds", companyId)), 1, Int32.MaxValue, SortBy.Ascending("companyIds"));
    //    //}

    //    public List<MapData> GetMapDataThisCorp(string corpName)
    //    {
    //        var paramDate = DateTime.Now.AddDays(-1);
    //        var setdate =
    //            DateTime.Parse(paramDate.Year + "-" + paramDate.Month.ToString().PadLeft(2, '0') + "-" + paramDate.Day.ToString().PadLeft(2, '0') + " 00:00:00");
    //        var enddate = DateTime.Parse(paramDate.Year + "-" + paramDate.Month.ToString().PadLeft(2, '0') + "-" + paramDate.Day.ToString().PadLeft(2, '0') + " 23:59:59");

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("CompanyStat", Query.And(
    //                Query.EQ("Project", corpName), Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)), 1, Int32.MaxValue, null);
    //        var result = listBsonDocument.Where(m => m.Contains("province"))
    //            .GroupBy(k => k["province"])
    //            .Select(group => new MapData()
    //            {
    //                name = group.Key.ToString(),
    //                value = group.Count(),
    //            }).ToList();
    //        return result;
    //    }

    //    public List<PieData> GetPieDataThisCorp(string corpName)
    //    {
    //        var paramDate = DateTime.Now.AddDays(-1);
    //        var setdate =
    //            DateTime.Parse(paramDate.Year + "-" + paramDate.Month.ToString().PadLeft(2, '0') + "-" + paramDate.Day.ToString().PadLeft(2, '0') + " 00:00:00");
    //        var enddate = DateTime.Parse(paramDate.Year + "-" + paramDate.Month.ToString().PadLeft(2, '0') + "-" + paramDate.Day.ToString().PadLeft(2, '0') + " 23:59:59");

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("CompanyStat", Query.And(
    //                Query.EQ("Project", corpName), Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)), 1, Int32.MaxValue, null);

    //        var result = listBsonDocument.Where(m => m.Contains("Brand"))
    //            .GroupBy(k => k["Brand"])
    //            .Select(group => new PieData()
    //            {
    //                name = group.Key.ToString(),
    //                value = group.Count(),
    //            }).ToList();
    //        return result;
    //    }

    //    public LineChartData_JiTuan getlinechart_jituan(string corpName)
    //    {
    //        LineChartData_JiTuan data = new LineChartData_JiTuan();

    //        var paramDate = DateTime.Now.AddDays(-1);
    //        var setdate =
    //            DateTime.Parse(paramDate.Year + "-" + paramDate.Month.ToString().PadLeft(2, '0') + "-" + "01" + " 00:00:00");
    //        var enddate = DateTime.Parse(paramDate.Year + "-" + paramDate.Month.ToString().PadLeft(2, '0') + "-" + paramDate.Day.ToString().PadLeft(2, '0') + " 23:59:59");

    //        var listBsonDocument = MongoHelper.GetBsonDocumentByQuery("CompanyStat",
    //            Query.And(Query.And(Query.GT("StatisticDateTime", setdate), Query.LT("StatisticDateTime", enddate)),
    //                Query.EQ("Project", corpName)), 1, Int32.MaxValue, SortBy.Ascending("name"));

    //        List<string> keysList = new List<string>();
    //        for (int i = 1; i <= enddate.Day; i++)
    //        {
    //            keysList.Add(DateTime.Now.Month.ToString().PadLeft(2, '0') + "-" + i.ToString().PadLeft(2, '0'));

    //            //按当天的时间获取记录 
    //            var obj = listBsonDocument.Where(m => m["StatisticDateTime"].AsDateTime.ToString("yyyy-MM-dd") == (enddate.Year + "-" + enddate.Month.ToString().PadLeft(2, '0') + "-" + i.ToString().PadLeft(2, '0'))).ToList();

    //            if (data.JianKaArr == null)
    //                data.JianKaArr = new int[enddate.Day];
    //            if (data.DaoDianArr == null)
    //                data.DaoDianArr = new int[enddate.Day];
    //            if (data.DianHuaArr == null)
    //                data.DianHuaArr = new int[enddate.Day];
    //            if (data.ShangTanArr == null)
    //                data.ShangTanArr = new int[enddate.Day];

    //            if (i == 1)
    //            {
    //                data.JianKaArr[i - 1] = obj.Count > 0 ? obj.Sum(n => n["AccJianKaCount"].AsInt32) : 0;
    //                data.DaoDianArr[i - 1] = obj.Count > 0 ? obj.Sum(n => n["AccDaoDianCount"].AsInt32) : 0;
    //                data.DianHuaArr[i - 1] = obj.Count > 0 ? obj.Sum(n => n["AccDianHuaCount"].AsInt32) : 0;
    //                data.ShangTanArr[i - 1] = Math.Max(data.DianHuaArr[i - 1], Math.Max(data.JianKaArr[i - 1], data.DaoDianArr[i - 1]));
    //            }
    //            else
    //            {
    //                //获取前一天的数据
    //                var previousDay = listBsonDocument.Where(m => m["StatisticDateTime"].AsDateTime.ToString("yyyy-MM-dd") == (enddate.Year + enddate.Month.ToString().PadLeft(2, '0') + (i - 1).ToString().PadLeft(2, '0'))).ToList();
    //                if (previousDay.Count > 0)
    //                {
    //                    if (obj.Count > 0)
    //                    {
    //                        data.JianKaArr[i - 1] = obj.Sum(n=>n["AccJianKaCount"].AsInt32) - previousDay.Sum(n=>n["AccJianKaCount"].AsInt32);
    //                        data.DaoDianArr[i - 1] = obj.Sum(n=>n["AccDaoDianCount"].AsInt32) - previousDay.Sum(n=>n["AccDaoDianCount"].AsInt32);
    //                        data.DianHuaArr[i - 1] = obj.Sum(n=>n["AccDianHuaCount"].AsInt32) - previousDay.Sum(n=>n["AccDianHuaCount"].AsInt32);
    //                        data.ShangTanArr[i - 1] = Math.Max(data.DianHuaArr[i - 1], Math.Max(data.JianKaArr[i - 1], data.DaoDianArr[i - 1]));
    //                    }
    //                    else
    //                    {
    //                        data.JianKaArr[i - 1] = 0;
    //                        data.DaoDianArr[i - 1] = 0;
    //                        data.DianHuaArr[i - 1] = 0;
    //                        data.ShangTanArr[i - 1] = 0;
    //                    }
    //                }
    //                else
    //                {
    //                    if (obj.Count > 0)
    //                    {
    //                        data.JianKaArr[i - 1] = obj.Sum(n=>n["AccJianKaCount"].AsInt32);
    //                        data.DaoDianArr[i - 1] = obj.Sum(n=>n["AccDaoDianCount"].AsInt32);
    //                        data.DianHuaArr[i - 1] = obj.Sum(n => n["AccDianHuaCount"].AsInt32);
    //                        data.ShangTanArr[i - 1] = Math.Max(data.DianHuaArr[i - 1], Math.Max(data.JianKaArr[i - 1], data.DaoDianArr[i - 1]));
    //                    }
    //                    else
    //                    {
    //                        data.JianKaArr[i - 1] = 0;
    //                        data.DaoDianArr[i - 1] = 0;
    //                        data.DianHuaArr[i - 1] = 0;
    //                        data.ShangTanArr[i - 1] = 0;
    //                    }
    //                }
    //            }
    //        }
    //        data.LegendArr = keysList.ToArray();
    //        return data;

    //    }


    //}
}