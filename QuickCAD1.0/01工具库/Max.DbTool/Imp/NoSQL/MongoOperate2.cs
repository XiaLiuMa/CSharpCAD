//using System;
//using System.Data;
//using System.Linq;
//using Max.BaseKit;
//using MongoDB.Bson;
//using MongoDB.Driver;
//using Max.BaseKit.Utils;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//namespace Max.DbTool.Imp
//{
//    /// <summary>
//    /// 提供Mongo数据库操作接口实现
//    /// </summary>
//    public class MongoOperate2 : ISqlOperate
//    {
//        public DbConfig Config { get; set; }

//        public bool Execute(string sqlstr)
//        {
//            throw new NotImplementedException();
//        }
//        public DataTable Query(string sqlstr)
//        {
//            throw new NotImplementedException();
//        }
//        public bool BatchCover(string tname, List<Dictionary<string, object>> dlst, string[] keys = null)
//        {
//            throw new NotImplementedException();
//        }
//        public bool BatchDelete(string tname, string[] fileds, List<Dictionary<string, object>> dlst)
//        {
//            throw new NotImplementedException();
//        }

//        private IMongoClient _client;
//        private IMongoDatabase _database;

//        public MongoOperate2()
//        {

//        }

//        public MongoOperate2(MongoClientSettings connection, string database)
//        {
//            #region 使用实例
//            //string _connection = "mongodb://fred:foobar@localhost/baz";
//            //string _database = "baz"; 
//            #endregion

//            _client = new MongoClient(connection);
//            _database = _client.GetDatabase(database);
//        }

//        public IMongoDatabase GetDatabase()
//        {
//            return _database;
//        }

//        public IMongoCollection<T> GetClient<T>(string table) where T : class, new()
//        {
//            return _database.GetCollection<T>(table);
//        }

//        #region 当前方案
//        private static object lckObj = new object();
//        /// <summary>
//        /// 根据时间删除
//        /// </summary>
//        /// <param name="table">表名</param>
//        /// <param name="bgyj">变更依据(用于根据时间删除的字段名称)</param>
//        /// <param name="datas">数据集</param>
//        /// <returns></returns>
//        public bool DeleteByTime(string table, string bgyj, List<Dictionary<string, object>> datas)
//        {
//            lock (lckObj)
//            {
//                try
//                {
//                    if (datas == null || datas.Count < 0) return true;
//                    List<string> tjsjs = new List<string>();
//                    datas.ForEach(p =>
//                    {
//                        datas.ForEach(p =>
//                        {
//                            string tjsj = p[bgyj].ToString();
//                            if (!tjsjs.Contains(tjsj))
//                            {
//                                tjsjs.Add(tjsj);
//                            }
//                        });
//                    });
//                    if (tjsjs.Count > 0)
//                    {
//                        var collection = _database.GetCollection<BsonDocument>(table);
//                        var modes = new List<WriteModel<BsonDocument>>();    //要删除的模型列表
//                        tjsjs.ForEach(p =>
//                        {
//                            var filter = Builders<BsonDocument>.Filter.Eq(bgyj, p);
//                            modes.Add(new DeleteOneModel<BsonDocument>(filter));
//                        });
//                        collection.BulkWriteAsync(modes).Wait();  //批量删除
//                        return true;
//                    }
//                }
//                catch (Exception ex)
//                {
//                    NLogger.Error($"根据时间删除{table}表数据异常:{ex.Message}");
//                }
//                return false;
//            }
//        }


//        /// <summary>
//        /// 根据主键批量删除数据
//        /// </summary>
//        /// <param name="table"></param>
//        /// <param name="dics"></param>
//        /// <returns></returns>
//        public bool DeleteManay(string table, List<Dictionary<string, object>> dics)
//        {
//            lock (lckObj)
//            {
//                try
//                {
//                    if (dics == null || dics.Count < 0) return true;
//                    var collection = _database.GetCollection<BsonDocument>(table);
//                    var modes = new List<WriteModel<BsonDocument>>();    //要删除的模型列表
//                    dics.ForEach(p =>
//                    {
//                        var filter = Builders<BsonDocument>.Filter.Eq("_id", p.Where(p1 => p1.Key.Equals("_id")).FirstOrDefault().Value);
//                        modes.Add(new DeleteOneModel<BsonDocument>(filter));
//                    });
//                    collection.BulkWriteAsync(modes).Wait();  //批量删除
//                    return true;
//                }
//                catch (Exception ex)
//                {
//                    NLogger.Error($"Mongo执行{table}表的DeleteManay异常:{ex.Message}");
//                }
//                return false;
//            }
//        }

//        /// <summary>
//        /// 批量更新数据(存在修改，不存在插入)
//        /// </summary>
//        /// <param name="table"></param>
//        /// <param name="dics"></param>
//        /// <returns></returns>
//        public bool UpdateManay(string table, List<Dictionary<string, object>> dics)
//        {
//            lock (lckObj)
//            {
//                try
//                {
//                    if (dics == null || dics.Count < 0) return true;
//                    var collection = _database.GetCollection<BsonDocument>(table);
//                    var modes = new List<WriteModel<BsonDocument>>();    //要删除的模型列表
//                    dics.ForEach(p =>
//                    {
//                        var filter = Builders<BsonDocument>.Filter.Eq("_id", p.Where(p1 => p1.Key.Equals("_id")).FirstOrDefault().Value);
//                        var updatedef = Builders<BsonDocument>.Update.Combine(NowBuildUpdateDefinition(BsonDocument.Parse(JsonUtil.ObjectToStr(p)), null));
//                        modes.Add(new UpdateOneModel<BsonDocument>(filter, updatedef) { IsUpsert = true });

//                        //var bsonDoc = p.ToBsonDocument();
//                        //WriteModel<BsonDocument> mode = new ReplaceOneModel<BsonDocument>(new BsonDocument("_id", p["_id"].ToString()), 
//                        //    bsonDoc) { IsUpsert = true };
//                        //modes.Add(mode);
//                    });
//                    collection.BulkWriteAsync(modes).Wait();  //批量修改

//                    #region 废弃
//                    //List<BsonDocument> docunemts = new List<BsonDocument>();
//                    //dics.ForEach(p =>
//                    //{
//                    //    var filter = Builders<BsonDocument>.Filter.Eq("_id", p.Where(p1 => p1.Key.Equals("_id")).FirstOrDefault().Value);
//                    //    modes.Add(new DeleteOneModel<BsonDocument>(filter));
//                    //    docunemts.Add(p.ToBsonDocument());
//                    //});

//                    //collection.BulkWriteAsync(modes).Wait();  //批量删除
//                    //collection.InsertManyAsync(docunemts).Wait();  //批量插入 
//                    #endregion
//                    return true;
//                }
//                catch (Exception ex)
//                {
//                    NLogger.Error($"Mongo执行{table}表的UpdateManay异常:{ex.Message}");
//                }
//                return false;
//            }
//        }

//        /// <summary>
//        /// 构建更新操作定义
//        /// </summary>
//        /// <param name="bc">bsondocument文档</param>
//        /// <returns></returns>
//        private List<UpdateDefinition<BsonDocument>> NowBuildUpdateDefinition(BsonDocument bc, string parent)
//        {
//            var updates = new List<UpdateDefinition<BsonDocument>>();
//            foreach (var element in bc.Elements)
//            {
//                var key = parent == null ? element.Name : $"{parent}.{element.Name}";
//                var subUpdates = new List<UpdateDefinition<BsonDocument>>();
//                //子元素是对象
//                if (element.Value.IsBsonDocument)
//                {
//                    updates.AddRange(NowBuildUpdateDefinition(element.Value.ToBsonDocument(), key));
//                }
//                //子元素是对象数组
//                else if (element.Value.IsBsonArray)
//                {
//                    var arrayDocs = element.Value.AsBsonArray;
//                    var i = 0;
//                    foreach (var doc in arrayDocs)
//                    {
//                        if (doc.IsBsonDocument)
//                        {
//                            updates.AddRange(NowBuildUpdateDefinition(doc.ToBsonDocument(), key + $".{i}"));
//                        }
//                        else
//                        {
//                            updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
//                            continue;
//                        }
//                        i++;
//                    }
//                }
//                //子元素是其他
//                else
//                {
//                    updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
//                }
//            }
//            return updates;
//        }
//        #endregion

//        #region 批量处理方案模板
//        //public void Insert(IEnumerable<TEntity> item)
//        //{
//        //    var list = new List<WriteModel<TEntity>>();
//        //    foreach (var iitem in item)
//        //    {
//        //        list.Add(new InsertOneModel<TEntity>(iitem));
//        //    }
//        //    _table.BulkWriteAsync(list).Wait();
//        //}

//        //public void Update(IEnumerable<TEntity> item)
//        //{

//        //    var list = new List<WriteModel<TEntity>>();

//        //    foreach (var iitem in item)
//        //    {
//        //        QueryDocument queryDocument = new QueryDocument("_id", new ObjectId(typeof(TEntity).GetProperty(EntityKey).GetValue(iitem).ToString()));
//        //        list.Add(new UpdateOneModel<TEntity>(queryDocument, Builders<TEntity>.Update.Combine(GeneratorMongoUpdate(iitem))));
//        //    }
//        //    _table.BulkWriteAsync(list).Wait();
//        //}

//        //public void Delete(IEnumerable<TEntity> item)
//        //{
//        //    var list = new List<WriteModel<TEntity>>();

//        //    foreach (var iitem in item)
//        //    {
//        //        QueryDocument queryDocument = new QueryDocument("_id", new ObjectId(typeof(TEntity).GetProperty(EntityKey).GetValue(iitem).ToString()));
//        //        list.Add(new DeleteOneModel<TEntity>(queryDocument));
//        //    }
//        //    _table.BulkWriteAsync(list).Wait();
//        //} 
//        #endregion

//        #region 纯Json操作
//        #region +Add 添加一条数据
//        /// <summary>
//        /// 添加一条数据
//        /// </summary>
//        /// <param name="table">集合名称</param>
//        /// <param name="json">json数据</param>
//        /// <param name="key">主键值，赋值给默认的_id</param>
//        /// <returns></returns>
//        public int Add(string table, string json, string key = null)
//        {
//            try
//            {
//                BsonDocument document = BsonDocument.Parse(json);
//                if (!string.IsNullOrEmpty(key))
//                {
//                    document.Add("_id", BsonValue.Create(key));
//                }
//                var client = _database.GetCollection<BsonDocument>(table);
//                client.InsertOne(document);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error(ex.ToString());
//                return 0;
//            }
//        }
//        #endregion

//        #region +AddAsync 异步添加一条数据
//        /// <summary>
//        /// 异步添加一条数据
//        /// </summary>
//        /// <param name="table">集合名称</param>
//        /// <param name="json">json数据</param>
//        /// <param name="key">主键值，赋值给默认的_id</param>
//        /// <returns></returns>
//        public async Task<int> AddAsync(string table, string json, string key = null)
//        {
//            try
//            {
//                BsonDocument document = BsonDocument.Parse(json);
//                if (!string.IsNullOrEmpty(key))
//                {
//                    document.Add("_id", BsonValue.Create(key));
//                }
//                var client = _database.GetCollection<BsonDocument>(table);
//                await client.InsertOneAsync(document);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error(ex.ToString());
//                return 0;
//            }
//        }
//        #endregion

//        #region +InsertMany 批量插入
//        /// <summary>
//        /// 批量插入
//        /// </summary>
//        /// <param name="table">集合名称</param>
//        /// <param name="jsons">json数据</param>
//        /// <returns></returns>
//        public int InsertMany(string table, List<string> jsons)
//        {
//            try
//            {
//                List<BsonDocument> bsons = new List<BsonDocument>();
//                jsons.ForEach(p => { bsons.Add(BsonDocument.Parse(p)); });
//                var client = _database.GetCollection<BsonDocument>(table);
//                client.InsertMany(bsons);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error(ex.ToString());
//                return 0;
//            }
//        }

//        /// <summary>
//        /// 批量插入
//        /// </summary>
//        /// <param name="table">集合名称</param>
//        /// <param name="dic">[主键,json数据]</param>
//        /// <returns></returns>
//        public int InsertMany(string table, Dictionary<string, string> dic)
//        {
//            try
//            {
//                List<BsonDocument> bsons = new List<BsonDocument>();
//                foreach (var item in dic)
//                {
//                    var bd = BsonDocument.Parse(item.Value);
//                    bd.Add("_id", BsonValue.Create(item.Key));
//                    bsons.Add(bd);
//                }
//                var client = _database.GetCollection<BsonDocument>(table);
//                client.InsertMany(bsons);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error(ex.ToString());
//                return 0;
//            }
//        }
//        #endregion

//        #region +InsertManyAsync 异步批量插入
//        /// <summary>
//        /// 异步批量插入
//        /// </summary>
//        /// <param name="table">集合名称</param>
//        /// <param name="jsons">json数据</param>
//        /// <returns></returns>
//        public async Task<int> InsertManyAsync(string table, List<string> jsons)
//        {
//            try
//            {
//                List<BsonDocument> bsons = new List<BsonDocument>();
//                jsons.ForEach(p => { bsons.Add(BsonDocument.Parse(p)); });
//                var client = _database.GetCollection<BsonDocument>(table);
//                await client.InsertManyAsync(bsons);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error($"{table}>>" + ex.ToString());
//                return 0;
//            }
//        }

//        /// <summary>
//        /// 异步批量插入
//        /// </summary>
//        /// <param name="table">集合名称</param>
//        /// <param name="dic">[主键,json数据]</param>
//        /// <returns></returns>
//        public async Task<int> InsertManyAsync(string table, Dictionary<string, string> dic)
//        {
//            try
//            {
//                List<BsonDocument> bsons = new List<BsonDocument>();
//                foreach (var item in dic)
//                {
//                    var bd = BsonDocument.Parse(item.Value);
//                    //bd.Add("_id", BsonValue.Create(item.Key));
//                    bsons.Add(bd);
//                }
//                var client = _database.GetCollection<BsonDocument>(table);
//                await client.InsertManyAsync(bsons);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error($"{table}>>" + ex.ToString());
//                return 0;
//            }
//        }
//        #endregion

//        #region +UpdateManayAsync 异步批量更新数据
//        /// <summary>
//        /// 异步批量更新数据(存在修改，不存在插入)
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="dic">要修改的字段</param>
//        /// <param name="filter">修改条件</param>
//        /// <returns></returns>
//        public async Task<int> UpdateManayAsync(string table, Dictionary<string, string> dic)
//        {
//            try
//            {
//                foreach (var item in dic)
//                {
//                    try
//                    {
//                        var client = _database.GetCollection<BsonDocument>(table);
//                        var filter = Builders<BsonDocument>.Filter.Eq("_id", item.Key);
//                        var bson = BsonDocument.Parse(item.Value);
//                        bson.Add("_id", BsonValue.Create(item.Key));
//                        var update = Builders<BsonDocument>.Update.Combine(BuildUpdateDefinition(bson, null));
//                        await client.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });    //更新
//                    }
//                    catch (Exception)   //给1次容错机会
//                    {
//                        var client = _database.GetCollection<BsonDocument>(table);
//                        var filter = Builders<BsonDocument>.Filter.Eq("_id", item.Key);
//                        var bson = BsonDocument.Parse(item.Value);
//                        bson.Add("_id", BsonValue.Create(item.Key));
//                        var update = Builders<BsonDocument>.Update.Combine(BuildUpdateDefinition(bson, null));
//                        await client.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });    //更新
//                    }
//                }
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                NLogger.Error($"{table}>>" + ex.ToString());
//                return 0;
//            }
//        }

//        /// <summary>
//        /// 构建更新操作定义
//        /// </summary>
//        /// <param name="bc">bsondocument文档</param>
//        /// <returns></returns>
//        private List<UpdateDefinition<BsonDocument>> BuildUpdateDefinition(BsonDocument bc, string parent)
//        {
//            var updates = new List<UpdateDefinition<BsonDocument>>();
//            foreach (var element in bc.Elements)
//            {
//                var key = parent == null ? element.Name : $"{parent}.{element.Name}";
//                var subUpdates = new List<UpdateDefinition<BsonDocument>>();
//                //子元素是对象
//                if (element.Value.IsBsonDocument)
//                {
//                    updates.AddRange(BuildUpdateDefinition(element.Value.ToBsonDocument(), key));
//                }
//                //子元素是对象数组
//                else if (element.Value.IsBsonArray)
//                {
//                    var arrayDocs = element.Value.AsBsonArray;
//                    var i = 0;
//                    foreach (var doc in arrayDocs)
//                    {
//                        if (doc.IsBsonDocument)
//                        {
//                            updates.AddRange(BuildUpdateDefinition(doc.ToBsonDocument(), key + $".{i}"));
//                        }
//                        else
//                        {
//                            updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
//                            continue;
//                        }
//                        i++;
//                    }
//                }
//                //子元素是其他
//                else
//                {
//                    updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
//                }
//            }
//            return updates;
//        }

//        #endregion
//        #endregion

//        #region 对象操作
//        #region +Add 添加一条数据
//        /// <summary>
//        /// 添加一条数据
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="t">添加的实体</param>
//        /// <returns></returns>
//        public int Add<T>(string table, T t) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                client.InsertOne(t);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//        }
//        #endregion

//        #region +AddAsync 异步添加一条数据
//        /// <summary>
//        /// 异步添加一条数据
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="t">添加的实体</param>
//        /// <returns></returns>
//        public async Task<int> AddAsync<T>(string table, T t) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                await client.InsertOneAsync(t);
//                return 1;
//            }
//            catch
//            {
//                return 0;
//            }
//        }
//        #endregion

//        #region +InsertMany 批量插入
//        /// <summary>
//        /// 批量插入
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="t">实体集合</param>
//        /// <returns></returns>
//        public int InsertMany<T>(string table, List<T> t) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                client.InsertMany(t);
//                return 1;
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//        }
//        #endregion

//        #region +InsertManyAsync 异步批量插入
//        /// <summary>
//        /// 异步批量插入
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="t">实体集合</param>
//        /// <returns></returns>
//        public async Task<int> InsertManyAsync<T>(string table, List<T> t) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                await client.InsertManyAsync(t);
//                return 1;
//            }
//            catch
//            {
//                return 0;
//            }
//        }
//        #endregion

//        #region +Update 修改一条数据
//        /// <summary>
//        /// 修改一条数据
//        /// </summary>
//        /// <param name="t">添加的实体</param>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="isObjectId">是否是主键</param>
//        /// <returns></returns>
//        public UpdateResult Update<T>(string table, T t, string id, bool isObjectId = true) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                //修改条件
//                FilterDefinition<T> filter;
//                if (isObjectId)
//                {
//                    filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
//                }
//                else
//                {
//                    filter = Builders<T>.Filter.Eq("_id", id);
//                }
//                //要修改的字段
//                var list = new List<UpdateDefinition<T>>();
//                foreach (var item in t.GetType().GetProperties())
//                {
//                    if (item.Name.ToLower() == "id") continue;
//                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
//                }
//                var updatefilter = Builders<T>.Update.Combine(list);
//                return client.UpdateOne(filter, updatefilter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region +UpdateAsync 异步修改一条数据
//        /// <summary>
//        /// 异步修改一条数据
//        /// </summary>
//        /// <param name="t">添加的实体</param>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="isObjectId">是否是主键</param>
//        /// <returns></returns>
//        public async Task<UpdateResult> UpdateAsync<T>(string table, T t, string id, bool isObjectId = true) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                //修改条件
//                FilterDefinition<T> filter;
//                if (isObjectId)
//                {
//                    filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
//                }
//                else
//                {
//                    filter = Builders<T>.Filter.Eq("_id", id);
//                }
//                //要修改的字段
//                var list = new List<UpdateDefinition<T>>();
//                foreach (var item in t.GetType().GetProperties())
//                {
//                    if (item.Name.ToLower() == "id") continue;
//                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
//                }
//                var updatefilter = Builders<T>.Update.Combine(list);
//                return await client.UpdateOneAsync(filter, updatefilter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region +UpdateManay 批量修改数据
//        /// <summary>
//        /// 批量修改数据
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="dic">要修改的字段</param>
//        /// <param name="filter">修改条件</param>
//        /// <returns></returns>
//        public UpdateResult UpdateManay<T>(string table, Dictionary<string, string> dic, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                T t = new T();
//                //要修改的字段
//                var list = new List<UpdateDefinition<T>>();
//                foreach (var item in t.GetType().GetProperties())
//                {
//                    if (!dic.ContainsKey(item.Name)) continue;
//                    var value = dic[item.Name];
//                    list.Add(Builders<T>.Update.Set(item.Name, value));
//                }
//                var updatefilter = Builders<T>.Update.Combine(list);
//                return client.UpdateMany(filter, updatefilter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region +UpdateManayAsync 异步批量修改数据
//        /// <summary>
//        /// 异步批量修改数据
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="dic">要修改的字段</param>
//        /// <param name="filter">修改条件</param>
//        /// <returns></returns>
//        public async Task<UpdateResult> UpdateManayAsync<T>(string table, Dictionary<string, string> dic, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                T t = new T();
//                //要修改的字段
//                var list = new List<UpdateDefinition<T>>();
//                foreach (var item in t.GetType().GetProperties())
//                {
//                    if (!dic.ContainsKey(item.Name)) continue;
//                    var value = dic[item.Name];
//                    list.Add(Builders<T>.Update.Set(item.Name, value));
//                }
//                var updatefilter = Builders<T>.Update.Combine(list);
//                return await client.UpdateManyAsync(filter, updatefilter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region Delete 删除一条数据
//        /// <summary>
//        /// 删除一条数据
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="id">objectId</param>
//        /// <param name="isObjectId">是否是主键</param>
//        /// <returns></returns>
//        public DeleteResult Delete<T>(string table, string id, bool isObjectId = true) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                FilterDefinition<T> filter;
//                if (isObjectId)
//                {
//                    filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
//                }
//                else
//                {
//                    filter = Builders<T>.Filter.Eq("_id", id);
//                }
//                return client.DeleteOne(filter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        #endregion

//        #region DeleteAsync 异步删除一条数据
//        /// <summary>
//        /// 异步删除一条数据
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="id">objectId</param>
//        /// <param name="isObjectId">是否是主键</param>
//        /// <returns></returns>
//        public async Task<DeleteResult> DeleteAsync<T>(string table, string id, bool isObjectId = true) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                //修改条件
//                FilterDefinition<T> filter;
//                if (isObjectId)
//                {
//                    filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
//                }
//                else
//                {
//                    filter = Builders<T>.Filter.Eq("_id", id);
//                }
//                return await client.DeleteOneAsync(filter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        #endregion

//        #region DeleteMany 删除多条数据
//        /// <summary>
//        /// 删除一条数据
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">删除的条件</param>
//        /// <returns></returns>
//        public DeleteResult DeleteMany<T>(string table, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                return client.DeleteMany(filter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        #endregion

//        #region DeleteManyAsync 异步删除多条数据
//        /// <summary>
//        /// 异步删除多条数据
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">删除的条件</param>
//        /// <returns></returns>
//        public async Task<DeleteResult> DeleteManyAsync<T>(string table, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                return await client.DeleteManyAsync(filter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }
//        #endregion

//        #region Count 根据条件获取总数
//        /// <summary>
//        /// 根据条件获取总数
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">条件</param>
//        /// <returns></returns>
//        public long Count<T>(string table, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                return client.Count(filter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region CountAsync 异步根据条件获取总数
//        /// <summary>
//        /// 异步根据条件获取总数
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">条件</param>
//        /// <returns></returns>
//        public async Task<long> CountAsync<T>(string table, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                return await client.CountAsync(filter);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region FindOne 根据id查询一条数据
//        /// <summary>
//        /// 根据id查询一条数据
//        /// </summary>
//        /// <param name="table">表名(集合名)</param>
//        /// <param name="id">objectid</param>
//        /// <param name="isObjectId">是否是主键</param>
//        /// <param name="field">要查询的字段，不写时查询全部</param>
//        /// <returns></returns>
//        public T FindOne<T>(string table, string id, bool isObjectId = true, string[] field = null) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                FilterDefinition<T> filter;
//                if (isObjectId)
//                {
//                    filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
//                }
//                else
//                {
//                    filter = Builders<T>.Filter.Eq("_id", id);
//                }
//                //不指定查询字段
//                if (field == null || field.Length == 0)
//                {
//                    return client.Find(filter).FirstOrDefault<T>();
//                }

//                //制定查询字段
//                var fieldList = new List<ProjectionDefinition<T>>();
//                for (int i = 0; i < field.Length; i++)
//                {
//                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
//                }
//                var projection = Builders<T>.Projection.Combine(fieldList);
//                fieldList?.Clear();
//                return client.Find(filter).Project<T>(projection).FirstOrDefault<T>();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region FindOneAsync 异步根据id查询一条数据
//        /// <summary>
//        /// 异步根据id查询一条数据
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="id">objectid</param>
//        /// <param name="isObjectId">是否是主键</param>
//        /// <returns></returns>
//        public async Task<T> FindOneAsync<T>(string table, string id, bool isObjectId = true, string[] field = null) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                FilterDefinition<T> filter;
//                if (isObjectId)
//                {
//                    filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
//                }
//                else
//                {
//                    filter = Builders<T>.Filter.Eq("_id", id);
//                }

//                //不指定查询字段
//                if (field == null || field.Length == 0)
//                {
//                    return await client.Find(filter).FirstOrDefaultAsync();
//                }

//                //制定查询字段
//                var fieldList = new List<ProjectionDefinition<T>>();
//                for (int i = 0; i < field.Length; i++)
//                {
//                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
//                }
//                var projection = Builders<T>.Projection.Combine(fieldList);
//                fieldList?.Clear();
//                return await client.Find(filter).Project<T>(projection).FirstOrDefaultAsync();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region FindList 查询集合
//        /// <summary>
//        /// 查询集合
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">查询条件</param>
//        /// <param name="field">要查询的字段,不写时查询全部</param>
//        /// <param name="sort">要排序的字段</param>
//        /// <returns></returns>
//        public List<T> FindList<T>(string table, FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                //不指定查询字段
//                if (field == null || field.Length == 0)
//                {
//                    if (sort == null) return client.Find(filter).ToList();
//                    //进行排序
//                    return client.Find(filter).Sort(sort).ToList();
//                }

//                //制定查询字段
//                var fieldList = new List<ProjectionDefinition<T>>();
//                for (int i = 0; i < field.Length; i++)
//                {
//                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
//                }
//                var projection = Builders<T>.Projection.Combine(fieldList);
//                fieldList?.Clear();
//                if (sort == null) return client.Find(filter).Project<T>(projection).ToList();
//                //排序查询
//                return client.Find(filter).Sort(sort).Project<T>(projection).ToList();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region FindListAsync 异步查询集合
//        /// <summary>
//        /// 异步查询集合
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">查询条件</param>
//        /// <param name="field">要查询的字段,不写时查询全部</param>
//        /// <param name="sort">要排序的字段</param>
//        /// <returns></returns>
//        public async Task<List<T>> FindListAsync<T>(string table, FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                //不指定查询字段
//                if (field == null || field.Length == 0)
//                {
//                    if (sort == null) return await client.Find(filter).ToListAsync();
//                    return await client.Find(filter).Sort(sort).ToListAsync();
//                }

//                //制定查询字段
//                var fieldList = new List<ProjectionDefinition<T>>();
//                for (int i = 0; i < field.Length; i++)
//                {
//                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
//                }
//                var projection = Builders<T>.Projection.Combine(fieldList);
//                fieldList?.Clear();
//                if (sort == null) return await client.Find(filter).Project<T>(projection).ToListAsync();
//                //排序查询
//                return await client.Find(filter).Sort(sort).Project<T>(projection).ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region FindListByPage 分页查询集合
//        /// <summary>
//        /// 分页查询集合
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">查询条件</param>
//        /// <param name="pageIndex">当前页</param>
//        /// <param name="pageSize">页容量</param>
//        /// <param name="count">总条数</param>
//        /// <param name="field">要查询的字段,不写时查询全部</param>
//        /// <param name="sort">要排序的字段</param>
//        /// <returns></returns>
//        public List<T> FindListByPage<T>(string table, FilterDefinition<T> filter, int pageIndex, int pageSize, out long count, string[] field = null, SortDefinition<T> sort = null) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                count = client.Count(filter);
//                //不指定查询字段
//                if (field == null || field.Length == 0)
//                {
//                    if (sort == null) return client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
//                    //进行排序
//                    return client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
//                }

//                //制定查询字段
//                var fieldList = new List<ProjectionDefinition<T>>();
//                for (int i = 0; i < field.Length; i++)
//                {
//                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
//                }
//                var projection = Builders<T>.Projection.Combine(fieldList);
//                fieldList?.Clear();

//                //不排序
//                if (sort == null) return client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

//                //排序查询
//                return client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        #region FindListByPageAsync 异步分页查询集合
//        /// <summary>
//        /// 异步分页查询集合
//        /// </summary>
//        /// <param name="table">字段名</param>
//        /// <param name="filter">查询条件</param>
//        /// <param name="pageIndex">当前页</param>
//        /// <param name="pageSize">页容量</param>
//        /// <param name="field">要查询的字段,不写时查询全部</param>
//        /// <param name="sort">要排序的字段</param>
//        /// <returns></returns>
//        public async Task<List<T>> FindListByPageAsync<T>(string table, FilterDefinition<T> filter, int pageIndex, int pageSize, string[] field = null, SortDefinition<T> sort = null) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                //不指定查询字段
//                if (field == null || field.Length == 0)
//                {
//                    if (sort == null) return await client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
//                    //进行排序
//                    return await client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
//                }

//                //制定查询字段
//                var fieldList = new List<ProjectionDefinition<T>>();
//                for (int i = 0; i < field.Length; i++)
//                {
//                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
//                }
//                var projection = Builders<T>.Projection.Combine(fieldList);
//                fieldList?.Clear();

//                //不排序
//                if (sort == null) return await client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

//                //排序查询
//                return await client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        public async Task<bool> AnyAsync<T>(string table, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                long count = await client.CountAsync(filter);
//                return count > 0;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        public bool Any<T>(string table, FilterDefinition<T> filter) where T : class, new()
//        {
//            try
//            {
//                var client = _database.GetCollection<T>(table);
//                long count = client.Count(filter);
//                return count > 0;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        #endregion

//        public void Dispose()
//        {
//            SqlOperateManger.I.Still(Config, this);
//        }
//    }
//}
