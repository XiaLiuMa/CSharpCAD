﻿using System;
using System.Data;
using System.Linq;
using Max.BaseKit;
using MongoDB.Bson;
using MongoDB.Driver;
using Max.BaseKit.Utils;
using System.Collections.Generic;
using Max.DbTool.Mod;

namespace Max.DbTool.Imp.NoSQL
{
    /// <summary>
    /// 提供Mongo数据库操作接口实现
    /// </summary>
    public class MongoOperate : AbsDbOperate
    {
        public override DbConfig Config { get; set; }
        public MongoOperate(DbConfig config) { Config = config; }

        public bool Execute(string sqlstr)
        {
            throw new NotImplementedException();
        }
        public DataTable Query(string sqlstr)
        {
            throw new NotImplementedException();
        }
        public bool BatchCover(string tname, List<Dictionary<string, object>> dlst, string[] keys = null)
        {
            throw new NotImplementedException();
        }
        public bool BatchDelete(string tname, string[] fileds, List<Dictionary<string, object>> dlst)
        {
            throw new NotImplementedException();
        }

        private IMongoClient _client;
        private IMongoDatabase _database;

        public MongoOperate()
        {

        }

        public MongoOperate(MongoClientSettings connection, string database)
        {
            #region 使用实例
            //string _connection = "mongodb://fred:foobar@localhost/baz";
            //string _database = "baz"; 
            #endregion

            _client = new MongoClient(connection);
            _database = _client.GetDatabase(database);
        }

        public IMongoDatabase GetDatabase()
        {
            return _database;
        }

        public IMongoCollection<T> GetClient<T>(string table) where T : class, new()
        {
            return _database.GetCollection<T>(table);
        }

        #region 当前方案
        private static object lckObj = new object();
        /// <summary>
        /// 根据时间删除
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="bgyj">变更依据(用于根据时间删除的字段名称)</param>
        /// <param name="datas">数据集</param>
        /// <returns></returns>
        public bool DeleteByTime(string table, string bgyj, List<Dictionary<string, object>> datas)
        {
            lock (lckObj)
            {
                try
                {
                    if (datas == null || datas.Count < 0) return true;
                    List<string> tjsjs = new List<string>();
                    datas.ForEach(p =>
                    {
                        datas.ForEach(p =>
                        {
                            string tjsj = p[bgyj].ToString();
                            if (!tjsjs.Contains(tjsj))
                            {
                                tjsjs.Add(tjsj);
                            }
                        });
                    });
                    if (tjsjs.Count > 0)
                    {
                        var collection = _database.GetCollection<BsonDocument>(table);
                        var modes = new List<WriteModel<BsonDocument>>();    //要删除的模型列表
                        tjsjs.ForEach(p =>
                        {
                            var filter = Builders<BsonDocument>.Filter.Eq(bgyj, p);
                            modes.Add(new DeleteOneModel<BsonDocument>(filter));
                        });
                        collection.BulkWriteAsync(modes).Wait();  //批量删除
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    NLogger.Error($"根据时间删除{table}表数据异常:{ex.Message}");
                }
                return false;
            }
        }


        /// <summary>
        /// 根据主键批量删除数据
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        public bool DeleteManay(string table, List<Dictionary<string, object>> dics)
        {
            lock (lckObj)
            {
                try
                {
                    if (dics == null || dics.Count < 0) return true;
                    var collection = _database.GetCollection<BsonDocument>(table);
                    var modes = new List<WriteModel<BsonDocument>>();    //要删除的模型列表
                    dics.ForEach(p =>
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", p.Where(p1 => p1.Key.Equals("_id")).FirstOrDefault().Value);
                        modes.Add(new DeleteOneModel<BsonDocument>(filter));
                    });
                    collection.BulkWriteAsync(modes).Wait();  //批量删除
                    return true;
                }
                catch (Exception ex)
                {
                    NLogger.Error($"Mongo执行{table}表的DeleteManay异常:{ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 批量更新数据(存在修改，不存在插入)
        /// </summary>
        /// <param name="table"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        public bool UpdateManay(string table, List<Dictionary<string, object>> dics)
        {
            lock (lckObj)
            {
                try
                {
                    if (dics == null || dics.Count < 0) return true;
                    var collection = _database.GetCollection<BsonDocument>(table);
                    var modes = new List<WriteModel<BsonDocument>>();    //要删除的模型列表
                    dics.ForEach(p =>
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", p.Where(p1 => p1.Key.Equals("_id")).FirstOrDefault().Value);
                        var updatedef = Builders<BsonDocument>.Update.Combine(NowBuildUpdateDefinition(BsonDocument.Parse(JsonUtil.ObjectToStr(p)), null));
                        modes.Add(new UpdateOneModel<BsonDocument>(filter, updatedef) { IsUpsert = true });

                        //var bsonDoc = p.ToBsonDocument();
                        //WriteModel<BsonDocument> mode = new ReplaceOneModel<BsonDocument>(new BsonDocument("_id", p["_id"].ToString()), 
                        //    bsonDoc) { IsUpsert = true };
                        //modes.Add(mode);
                    });
                    collection.BulkWriteAsync(modes).Wait();  //批量修改

                    #region 废弃
                    //List<BsonDocument> docunemts = new List<BsonDocument>();
                    //dics.ForEach(p =>
                    //{
                    //    var filter = Builders<BsonDocument>.Filter.Eq("_id", p.Where(p1 => p1.Key.Equals("_id")).FirstOrDefault().Value);
                    //    modes.Add(new DeleteOneModel<BsonDocument>(filter));
                    //    docunemts.Add(p.ToBsonDocument());
                    //});

                    //collection.BulkWriteAsync(modes).Wait();  //批量删除
                    //collection.InsertManyAsync(docunemts).Wait();  //批量插入 
                    #endregion
                    return true;
                }
                catch (Exception ex)
                {
                    NLogger.Error($"Mongo执行{table}表的UpdateManay异常:{ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 构建更新操作定义
        /// </summary>
        /// <param name="bc">bsondocument文档</param>
        /// <returns></returns>
        private List<UpdateDefinition<BsonDocument>> NowBuildUpdateDefinition(BsonDocument bc, string parent)
        {
            var updates = new List<UpdateDefinition<BsonDocument>>();
            foreach (var element in bc.Elements)
            {
                var key = parent == null ? element.Name : $"{parent}.{element.Name}";
                var subUpdates = new List<UpdateDefinition<BsonDocument>>();
                //子元素是对象
                if (element.Value.IsBsonDocument)
                {
                    updates.AddRange(NowBuildUpdateDefinition(element.Value.ToBsonDocument(), key));
                }
                //子元素是对象数组
                else if (element.Value.IsBsonArray)
                {
                    var arrayDocs = element.Value.AsBsonArray;
                    var i = 0;
                    foreach (var doc in arrayDocs)
                    {
                        if (doc.IsBsonDocument)
                        {
                            updates.AddRange(NowBuildUpdateDefinition(doc.ToBsonDocument(), key + $".{i}"));
                        }
                        else
                        {
                            updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
                            continue;
                        }
                        i++;
                    }
                }
                //子元素是其他
                else
                {
                    updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
                }
            }
            return updates;
        }

        public override bool TryConnect()
        {
            throw new NotImplementedException();
        }

        public override List<string> QueryTableNames()
        {
            throw new NotImplementedException();
        }

        public override bool ExecuteSql(string sqlstr)
        {
            throw new NotImplementedException();
        }

        public override List<Dictionary<string, object>> QuerySql(string sqlstr)
        {
            throw new NotImplementedException();
        }

        public override bool Insert<T>(T model)
        {
            throw new NotImplementedException();
        }

        public override bool Insert(string tname, IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public override bool BatchInsert<T>(List<T> models)
        {
            throw new NotImplementedException();
        }

        public override bool BatchInsert(string tname, List<IDictionary<string, object>> datas)
        {
            throw new NotImplementedException();
        }

        public override bool Delete<T>(T model)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(string tname, List<string> keys, IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public override bool BatchDelete<T>(List<T> models)
        {
            throw new NotImplementedException();
        }

        public override bool BatchDelete(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteByCondition<T>(Func<T, bool> filter)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteByCondition(string table, params ConditionModel[] conditions)
        {
            throw new NotImplementedException();
        }

        public override bool Clear<T>()
        {
            throw new NotImplementedException();
        }

        public override bool Clear(string tname)
        {
            throw new NotImplementedException();
        }

        public override bool Update<T>(T model)
        {
            throw new NotImplementedException();
        }

        public override bool Update(string tname, List<string> keys, IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public override bool BatchUpdate<T>(List<T> models)
        {
            throw new NotImplementedException();
        }

        public override bool BatchUpdate(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            throw new NotImplementedException();
        }

        public override bool Cover<T>(T model)
        {
            throw new NotImplementedException();
        }

        public override bool Cover(string tname, List<string> keys, IDictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public override bool BatchCover<T>(List<T> models)
        {
            throw new NotImplementedException();
        }

        public override bool BatchCover(string tname, List<string> keys, List<IDictionary<string, object>> datas)
        {
            throw new NotImplementedException();
        }

        public override T QueryFirst<T>(params object[] values)
        {
            throw new NotImplementedException();
        }

        public override List<T> QueryByCondition<T>(Func<T, bool> filter = null)
        {
            throw new NotImplementedException();
        }

        public override List<Dictionary<string, object>> QueryByCondition(string table, params ConditionModel[] conditions)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            DbOperateManger.I.ReturnObject(Config, this);
        }
        #endregion
    }
}
