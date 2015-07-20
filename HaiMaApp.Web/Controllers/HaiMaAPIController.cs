using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HaiMaApp.Web.Controllers
{
    public class HaiMaAPIController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        public IEnumerable<TongZhi> GetTongZhiList()
        {
            //List<TongZhi> lists = new List<TongZhi>();
            //var datasource = SqlHelper.ExecuteDataset(ConfigurationManager.ConnectionStrings["conn"].ToString(), CommandType.Text, "select top 10 * from TongZhi");
            //if (datasource != null && datasource.Tables.Count > 0)
            //{

            //    for (int i = 0; i < datasource.Tables[0].Rows.Count; i++)
            //    {
            //        lists.Add(new TongZhi()
            //        {
            //            id = Int32.Parse(datasource.Tables[0].Rows[i]["id"].ToString()),
            //            title = datasource.Tables[0].Rows[i]["title"].ToString(),
            //            content = datasource.Tables[0].Rows[i]["content"].ToString(),
            //            time = datasource.Tables[0].Rows[i]["time"].ToString(),
            //            //sender = datasource.Tables[0].Rows[i]["sender"].ToString(),
            //        });
            //    }

            //}
            HaiMaApp db = new HaiMaApp();
            return db.TongZhi.ToList();
        }

        [HttpGet]
        public IEnumerable<NewTongXunLu> GettxlList()
        {
            HaiMaApp db = new HaiMaApp();
            var list = db.NewTongXunLu.ToList();
            return list;
        }
       
        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value" + id;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}