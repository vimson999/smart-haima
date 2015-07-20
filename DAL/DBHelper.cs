using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Configuration;

namespace DAL
{
    /// <summary>
    /// mongodb的封装类。


    /// add by yancong2008@gmail.com 2011.05.14
    /// 
    ///为数据安全起见 可以增加 SafeMode 
    ///没有safemode可能会有数据丢失的情况，
    ///    SafeMode== false（默认）： mongodb把数据会先写入内存，然后再持久化到硬盘，断电有丢失数据的现象，此方式速度快


    ///    SafeMode== true： mongodb把数据持久化到硬盘，然后再处理后面的数据，此方式慢


    ///result = myCollection.Remove/insert/update(query, SafeMode.W2); 
    /// </summary>
    public sealed class DBHelper : IDisposable
    //public sealed class DBHelper<T>
    //where T :class
    {
        public string connectionString_DefaultRead = System.Configuration.ConfigurationManager.AppSettings["ConnectionString_mongoDBRead"];
        public string connectionString_Default = System.Configuration.ConfigurationManager.AppSettings["ConnectionString_mongoDB"];
        public string database_Default = System.Configuration.ConfigurationManager.AppSettings["Database_mongoDB_Ant"];

        public static DBHelper MongoHelper;

        static DBHelper()
        {
            MongoHelper = new DBHelper(ConfigurationManager.AppSettings["ConnectionString_mongoDB"], ConfigurationManager.AppSettings["ConnectionString_mongoDBRead"], ConfigurationManager.AppSettings["Database_mongoDB_Ant"]);
        }

        public DBHelper(string connectionStringForWrite, string connectionStringForRead, string database)
        {
            connectionString_Default = connectionStringForWrite;
            connectionString_DefaultRead = connectionStringForRead;
            database_Default = database;
        }

        public DBHelper(string database)
        {
            database_Default = database;
        }

        public DBHelper()
        {

        }

        ~DBHelper()
        {

        }

        /// <summary>
        /// 批量修改的方法:dgl
        /// </summary>
        [Obsolete("foreach更新，没有必须要不建议使用")]
        public List<SafeModeResult> UpdateAll<T>(string collectionName, IEnumerable<T> entitys)
        {
            List<SafeModeResult> results = new List<SafeModeResult>();
            MongoServer server = MongoServer.Create(this.connectionString_Default);
            //获取数据库或者创建数据库（不存在的话）。
            try
            {
                MongoDatabase database = server.GetDatabase(this.database_Default);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    foreach (var entity in entitys)
                    {
                        SafeModeResult result = myCollection.Save(entity);
                        results.Add(result);
                    }
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return results;
        }

        [Obsolete("网站建任务专用，myCollection.Count()慢")]
        public SafeModeResult DeleteAllUnitTask(string collectionName, IMongoQuery query)
        {
            MongoServer server = MongoServer.Create(this.connectionString_Default);
            //获取数据库或者创建数据库（不存在的话）。
            MongoDatabase database = server.GetDatabase(this.database_Default);
            SafeModeResult result;
            try
            {
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    if (null == query)
                    {
                        result = myCollection.RemoveAll();
                    }
                    else
                    {
                        result = myCollection.Remove(query);
                        myCollection.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }


        #region 新增
        public SafeModeResult InsertOne<T>(string collectionName, T entity)
        {
            return InsertOne<T>(this.connectionString_Default, this.database_Default, collectionName, entity);
        }
        public SafeModeResult InsertOne<T>(string connectionString, string databaseName, string collectionName, T entity)
        {
            SafeModeResult result = new SafeModeResult();
            if (null == entity)
            {
                return null;
            }
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.Insert(entity);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }

            return result;
        }


        public IEnumerable<SafeModeResult> InsertAll<T>(string collectionName, IEnumerable<T> entitys)
        {
            return InsertAll<T>(this.connectionString_Default, this.database_Default, collectionName, entitys);
        }
        public IEnumerable<SafeModeResult> InsertAll<T>(string connectionString, string databaseName, string collectionName, IEnumerable<T> entitys)
        {
            if (null == entitys)
            {
                return null;
            }
            MongoServer server = MongoServer.Create(connectionString);
            IEnumerable<SafeModeResult> result = null;
            try
            {
                //获取数据库或者创建数据库（不存在的话）。


                MongoDatabase database = server.GetDatabase(databaseName);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.InsertBatch(entitys, SafeMode.True);
                    //myCollection.Count();
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }

            return result;
        }
        #endregion

        #region 修改
        public SafeModeResult UpdateOne<T>(string collectionName, T entity)
        {
            return UpdateOne<T>(this.connectionString_Default, this.database_Default, collectionName, entity);
        }


        public SafeModeResult UpdateOne<T>(string collectionName, string id, IMongoUpdate update)
        {

            ObjectId mid;
            if (!ObjectId.TryParse(id, out mid))
            {
                return null;
            }

            return UpdateAll(collectionName, Query.EQ("_id", mid), update);
        }


        public SafeModeResult UpdateOne<T>(string connectionString, string databaseName, string collectionName, T entity)
        {
            SafeModeResult result;
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.Save(entity);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="update">更新设置。调用示例：Update.Set("Title", "yanc") 或者 Update.Set("Title", "yanc").Set("Author", "yanc2") 等等</param>
        /// <returns></returns>
        public SafeModeResult UpdateAll(string collectionName, IMongoQuery query, IMongoUpdate update)
        {
            return UpdateAll(this.connectionString_Default, this.database_Default, collectionName, query, update);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="update">更新设置。调用示例：Update.Set("Title", "yanc") 或者 Update.Set("Title", "yanc").Set("Author", "yanc2") 等等</param>
        /// <returns></returns>
        public SafeModeResult UpdateAll(string connectionString, string databaseName, string collectionName, IMongoQuery query, IMongoUpdate update)
        {
            SafeModeResult result = new SafeModeResult();
            if (null == query || null == update)
            {
                return null;
            }
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。


            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.Update(query, update, UpdateFlags.Multi);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }
        #endregion

        #region 删除
        public SafeModeResult Delete(string collectionName, string _id)
        {
            return Delete(this.connectionString_Default, this.database_Default, collectionName, _id);
        }
        public SafeModeResult Delete(string collectionName, IMongoQuery query)
        {
            SafeModeResult result;
            MongoServer server = MongoServer.Create(this.connectionString_Default);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(this.database_Default);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.Remove(query);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }
        public SafeModeResult Delete(string collectionName, ObjectId _id)
        {
            return Delete(this.connectionString_Default, this.database_Default, collectionName, _id);
        }
        public SafeModeResult Delete(string connectionString, string databaseName, string collectionName, ObjectId id)
        {
            SafeModeResult result;
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.Remove(Query.EQ("_id", id));
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }
        public SafeModeResult Delete(string connectionString, string databaseName, string collectionName, string _id)
        {
            SafeModeResult result;
            ObjectId id;
            if (!ObjectId.TryParse(_id, out id))
            {
                return null;
            }
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。




            MongoDatabase database = server.GetDatabase(databaseName);
            try
            {
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    result = myCollection.Remove(Query.EQ("_id", id));
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }

            return result;
        }
        public SafeModeResult DeleteAll(string collectionName)
        {
            return DeleteAll(this.connectionString_Default, this.database_Default, collectionName, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <returns></returns>
        public SafeModeResult DeleteAll(string collectionName, IMongoQuery query)
        {
            return DeleteAll(this.connectionString_Default, this.database_Default, collectionName, query);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <returns></returns>
        public SafeModeResult DeleteAll(string connectionString, string databaseName, string collectionName, IMongoQuery query)
        {
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。




            MongoDatabase database = server.GetDatabase(databaseName);
            SafeModeResult result;
            try
            {
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    if (null == query)
                    {
                        result = myCollection.RemoveAll();
                    }
                    else
                    {
                        result = myCollection.Remove(query);
                        //myCollection.Count();
                    }
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }
        #endregion

        #region 获取
        #region 获取单条信息
        public T GetOne<T>(string collectionName, string _id)
        {
            return GetOne<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, _id);
        }
        public T GetOne<T>(string connectionString, string databaseName, string collectionName, string _id)
        {
            T result = default(T);
            ObjectId id;
            if (!ObjectId.TryParse(_id, out id))
            {
                return default(T);
            }
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);
                using (server.RequestStart(database))//开始连接数据库。
                {

                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    MongoCursor<T> myCursor;
                    myCursor = myCollection.FindAs<T>(Query.EQ("_id", id));
                    myCursor.SetReadPreference(new ReadPreference(ReadPreferenceMode.SecondaryPreferred));
                    result = myCursor.FirstOrDefault();
                    //result = myCollection.FindOneAs<T>();

                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <returns></returns>
        public T GetOne<T>(string collectionName, IMongoQuery query)
        {
            return GetOne<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, query);
        }


        public T GetOne<T>(string connectionString, string databaseName, string collectionName, IMongoQuery query, SortByBuilder sb)
        {
            MongoServer server = MongoServer.Create(connectionString);
            T result = default(T);
            try
            {
                //获取数据库或者创建数据库（不存在的话）。



                MongoDatabase database = server.GetDatabase(databaseName);


                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    MongoCursor<T> myCursor = null;
                    if (null == query)
                    {
                        try
                        {
                            if (sb == null)
                            {
                                myCursor = myCollection.FindAs<T>(Query.NE("_id", ""));
                            }
                            else
                            {
                                myCursor = myCollection.FindAs<T>(Query.NE("_id", "")).SetSortOrder(sb);
                            }
                        }
                        catch (Exception ex)
                        {
                            server.Disconnect();
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            if (sb == null)
                            {
                                myCursor = myCollection.FindAs<T>(query);
                            }
                            else
                            {
                                myCursor = myCollection.FindAs<T>(query).SetSortOrder(sb);
                            }
                        }
                        catch (Exception ex)
                        {
                            server.Disconnect();
                            throw new Exception(ex.Message);
                        }
                    }


                    myCursor.SetReadPreference(new ReadPreference(ReadPreferenceMode.SecondaryPreferred));
                    result = myCursor.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <returns></returns>
        public T GetOne<T>(string connectionString, string databaseName, string collectionName, IMongoQuery query)
        {
            return GetOne<T>(connectionString, databaseName, collectionName, query, null);
        }


        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="query">查询条件</param>
        /// <param name="sb">排序</param>
        /// <returns>单个实体对象</returns>
        public T GetOne<T>(string collectionName, IMongoQuery query, SortByBuilder sb)
        {
            return GetOne<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, query, sb);
        }


        #endregion

        public BsonDocument GetOne(string collectionName, IMongoQuery query)
        {
            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            BsonDocument result = new BsonDocument();
            try
            {
                //获取数据库或者创建数据库（不存在的话）。


                MongoDatabase database = server.GetDatabase(this.database_Default);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    MongoCursor<BsonDocument> myCursor = null;
                    try
                    {
                        myCursor = myCollection.Find(query);
                    }
                    catch (Exception ex)
                    {
                        server.Disconnect();
                        throw new Exception(ex.Message);
                    }

                    myCursor.SetReadPreference(new ReadPreference(ReadPreferenceMode.SecondaryPreferred));
                    result = myCursor.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }

        #region 获取多个

        public List<BsonDocument> GetBsonDocumentByQuery(string collectionName, IMongoQuery query, int page, int pageSize, IMongoSortBy sortBy, params string[] fields)
        {
            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            MongoDatabase database = server.GetDatabase(this.database_Default);
            List<BsonDocument> result = new List<BsonDocument>();
            using (server.RequestStart(database))
            {
                MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                MongoCursor<BsonDocument> myCursor;
                if (null == query)
                {
                    myCursor = myCollection.FindAll();
                }
                else
                {
                    myCursor = myCollection.Find(query);
                }
                if (null != sortBy)
                {
                    myCursor.SetSortOrder(sortBy);
                }
                if (null != fields)
                {
                    myCursor.SetFields(fields);
                }
                result = myCursor.SetSkip((page - 1) * pageSize).SetLimit(pageSize).ToList();
            }
            return result;
        }

        public List<T> GetAll<T>(string collectionName)
        {
            return GetAll<T>(this.connectionString_DefaultRead, this.database_Default, collectionName);
        }

        /// <summary>
        /// 如果不清楚具体的数量，一般不要用这个函数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public List<T> GetAll<T>(string connectionString, string databaseName, string collectionName)
        {
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。
            List<T> result = new List<T>();
            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);

                    MongoCursor<T> myCursor;
                    myCursor = myCollection.FindAs<T>(null);

                    result = myCursor.ToList();

                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="pagerInfo"></param>
        /// <returns></returns>
        public List<T> GetAll<T>(string collectionName, IMongoQuery query, int page, int pageSize)
        {
            return GetAll<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, query, page, pageSize, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="pageSize"></param>
        /// <param name="sortBy">排序用的。调用示例：SortBy.Descending("Title") 或者 SortBy.Descending("Title").Ascending("Author")等等</param>
        /// <returns></returns>
        public List<T> GetAll<T>(string collectionName, IMongoQuery query, int page, int pageSize, IMongoSortBy sortBy)
        {
            return GetAll<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, query, page, pageSize, sortBy);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="pagerInfo"></param>
        /// <param name="fields">只返回所需要的字段的数据。调用示例："Title" 或者 new string[] { "Title", "Author" }等等</param>
        /// <returns></returns>
        public List<T> GetAll<T>(string collectionName, IMongoQuery query, int page, int pageSize, params string[] fields)
        {
            return GetAll<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, query, page, pageSize, null, fields);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="pagerInfo"></param>
        /// <param name="sortBy">排序用的。调用示例：SortBy.Descending("Title") 或者 SortBy.Descending("Title").Ascending("Author")等等</param>
        /// <param name="fields">只返回所需要的字段的数据。调用示例："Title" 或者 new string[] { "Title", "Author" }等等</param>
        /// <returns></returns>
        public List<T> GetAll<T>(string collectionName, IMongoQuery query, int page, int pageSize, IMongoSortBy sortBy, params string[] fields)
        {
            return GetAll<T>(this.connectionString_DefaultRead, this.database_Default, collectionName, query, page, pageSize, sortBy, fields);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query">条件查询。 调用示例：Query.Matches("Title", "感冒") 或者 Query.EQ("Title", "感冒") 或者Query.And(Query.Matches("Title", "感冒"),Query.EQ("Author", "yanc")) 等等</param>
        /// <param name="pagerInfo"></param>
        /// <param name="sortBy">排序用的。调用示例：SortBy.Descending("Title") 或者 SortBy.Descending("Title").Ascending("Author")等等</param>
        /// <param name="fields">只返回所需要的字段的数据。调用示例："Title" 或者 new string[] { "Title", "Author" }等等</param>
        /// <returns></returns>
        public List<T> GetAll<T>(string connectionString, string databaseName, string collectionName, IMongoQuery query, int page, int pageSize, IMongoSortBy sortBy, params string[] fields)
        {
            MongoServer server = MongoServer.Create(connectionString);
            List<T> result = new List<T>();
            try
            {
                //获取数据库或者创建数据库（不存在的话）。



                MongoDatabase database = server.GetDatabase(databaseName);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);

                    MongoCursor<T> myCursor;
                    if (null == query)
                    {
                        myCursor = myCollection.FindAllAs<T>();
                    }
                    else
                    {
                        myCursor = myCollection.FindAs<T>(query);
                    }
                    if (null != sortBy)
                    {
                        myCursor.SetSortOrder(sortBy);
                    }
                    if (null != fields && fields.Length > 0)
                    {
                        IMongoFields fieldss = Fields.Include(fields);

                        myCursor.SetFields(fieldss);
                    }

                    MongoCursor<T> ListMongoCursor = myCursor.SetSkip((page - 1) * pageSize).SetLimit(pageSize);

                    result = ListMongoCursor.ToList();

                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                //throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }

        /// <summary>
        /// 根据条件获取全部数据
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="conllectionName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> GetAllByQuery<T>(string collectionName, IMongoQuery query)
        {
            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            List<T> result = new List<T>();
            try
            {
                //获取数据库或者创建数据库（不存在的话）。



                MongoDatabase database = server.GetDatabase(this.database_Default);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);

                    MongoCursor<T> myCursor;
                    if (null == query)
                    {
                        myCursor = myCollection.FindAllAs<T>();
                    }
                    else
                    {
                        myCursor = myCollection.FindAs<T>(query);
                    }
                    result = myCursor.ToList();

                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }

        /// <summary>
        /// 根据条件获取指定条数数据
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="conllectionName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> GetAllByQuery<T>(int count, string collectionName, IMongoQuery query, IMongoSortBy sortBy = null)
        {
            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            List<T> result = new List<T>();
            try
            {
                //获取数据库或者创建数据库（不存在的话）。



                MongoDatabase database = server.GetDatabase(this.database_Default);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);

                    MongoCursor<T> myCursor;
                    if (null == query)
                    {
                        myCursor = myCollection.FindAllAs<T>();
                    }
                    else
                    {
                        myCursor = myCollection.FindAs<T>(query);
                    }
                    if (null != sortBy)
                    {
                        myCursor.SetSortOrder(sortBy);
                    }
                    foreach (T entity in myCursor.SetSkip(0).SetLimit(count))
                    {
                        result.Add(entity);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
        }

        public List<T> GetAllByQuery<T>(string collectionName, IMongoQuery query, IMongoSortBy sortBy)
        {
            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            List<T> result = new List<T>();
            try
            {
                //获取数据库或者创建数据库（不存在的话）。



                MongoDatabase database = server.GetDatabase(this.database_Default);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);

                    MongoCursor<T> myCursor;
                    if (null == query)
                    {
                        myCursor = myCollection.FindAllAs<T>();
                    }
                    else
                    {
                        myCursor = myCollection.FindAs<T>(query);
                    }
                    if (null != sortBy)
                    {
                        myCursor.SetSortOrder(sortBy);
                    }
                    result = myCursor.ToList();

                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return result;
        }

        #endregion

        #region Count

        public long CountAll(string collectionName, IMongoQuery query)
        {
            return CountAll(this.connectionString_DefaultRead, this.database_Default, collectionName, query);
        }

        public long CountAll(string connectionString, string databaseName, string collectionName, IMongoQuery query)
        {

            MongoServer server = MongoServer.Create(connectionString);
            long recordCount = 0;
            try
            {
                //获取数据库或者创建数据库（不存在的话）。   
                MongoDatabase database = server.GetDatabase(databaseName);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection myCollection = database.GetCollection(collectionName);
                    recordCount = myCollection.Count(query);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return recordCount;
        }

        /// <summary>
        /// 强制使用索引统计数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <param name="hintDoc"></param>
        /// <returns></returns>
        public long CountAllByHint<T>(string collectionName, IMongoQuery query, BsonDocument hintDoc)
        {
            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            long recordCount = 0;
            try
            {
                //获取数据库或者创建数据库（不存在的话）。   
                MongoDatabase database = server.GetDatabase(this.database_Default);

                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection myCollection = database.GetCollection(collectionName);

                    MongoCursor<T> myCursor;
                    if (null == query)
                    {
                        myCursor = myCollection.FindAllAs<T>();
                    }
                    else
                    {
                        myCursor = myCollection.FindAs<T>(query);
                    }

                    if (hintDoc == BsonNull.Value)
                        recordCount = myCursor.LongCount();
                    else
                        recordCount = myCursor.SetHint(hintDoc).LongCount();
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
            return recordCount;
        }
        #endregion
        #endregion

        #region Collection
        /// <summary>
        /// 创建新的集合
        /// </summary>
        /// <param name="newCollectionName">新的集合名称</param>
        public void CreateCollection(string newCollectionName)
        {
            MongoServer server = MongoServer.Create(this.connectionString_Default);
            BsonDocument result = new BsonDocument();

            //获取数据库或者创建数据库（不存在的话）。


            MongoDatabase database = server.GetDatabase(this.database_Default);
            try
            {
                using (server.RequestStart(database))//开始连接数据库。
                {
                    database.CreateCollection(newCollectionName);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
        }


        /// <summary>
        /// 移出集合
        /// </summary>
        /// <param name="dropCollectionName"></param>
        public void DropCollection(string dropCollectionName)
        {
            MongoServer server = MongoServer.Create(this.connectionString_Default);
            BsonDocument result = new BsonDocument();

            //获取数据库或者创建数据库（不存在的话）。


            MongoDatabase database = server.GetDatabase(this.database_Default);
            try
            {
                using (server.RequestStart(database))//开始连接数据库。
                {
                    database.DropCollection(dropCollectionName);
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
        }
        #endregion

        #region 索引
        public void CreateIndex(string collectionName, IMongoIndexKeys keys)
        {
            CreateIndex(connectionString_Default, database_Default, collectionName, keys);
        }
        public void CreateIndex(string connectionString, string databaseName, string collectionName, IMongoIndexKeys keys)
        {
            SafeModeResult result = new SafeModeResult();
            if (null == keys)
            {
                return;
            }
            MongoServer server = MongoServer.Create(connectionString);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(databaseName);
                using (server.RequestStart(database))//开始连接数据库。
                {
                    MongoCollection<BsonDocument> myCollection = database.GetCollection<BsonDocument>(collectionName);
                    if (!myCollection.IndexExists(keys))
                    {
                        myCollection.EnsureIndex(keys);
                    }
                }
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }
        }
        #endregion

        #region MapReduce
        public MapReduceResult MapReduce(string collectionName, IMongoQuery query, BsonJavaScript map, BsonJavaScript reduce, IMongoMapReduceOptions mapReduceOptions)
        {
            MapReduceResult result = new MapReduceResult();

            MongoServer server = MongoServer.Create(this.connectionString_DefaultRead);
            //获取数据库或者创建数据库（不存在的话）。



            try
            {
                MongoDatabase database = server.GetDatabase(this.database_Default);
                server.Connect();
                result = database[collectionName].MapReduce(query, map, reduce, mapReduceOptions);
            }
            catch (Exception ex)
            {
                server.Disconnect();
                throw new Exception(ex.Message);
            }
            finally
            {
                server.Disconnect();
            }

            return result;
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {

        }

        #endregion
    }
}