using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using zbwebcore.API.Model;

namespace zbwebcore.API.Config
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    public class MongodbHelper : IDisposable
    {
        private readonly IMongoDatabase _db = null;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="server">服务器链接字符串，格式如下：mongodb://root:k8008.com@192.168.1.125/uploadfile</param>
        /// <param name="dbName"></param>
        public MongodbHelper(string server, string dbName)
        {
            var client = new MongoClient(server);
            _db = client.GetDatabase(dbName);
        }
        public MongodbHelper(IDataBaseSetting settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _db = client.GetDatabase(settings.DatabaseName);
        }


        /// <summary>
        /// 生成Id
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private int GetId(string tableName)
        {
            lock (tableName)
            {
                var collection = _db.GetCollection<counters>("counters");
                //获取
                counters model = collection.Find(m => m.TableName == tableName).FirstOrDefault() ?? new counters();
                model.ID += 1;
                model.TableName = tableName;
                if (model.ID == 1)
                {
                    collection.InsertOne(model);
                }
                else
                {
                    FilterDefinition<counters> filter = Builders<counters>.Filter.Eq(f => f.TableName, tableName);
                    var update = Builders<counters>.Update.Set(m => m.ID, model.ID);
                    var updateOptions = new UpdateOptions { IsUpsert = true };
                    collection.UpdateOne<counters>(m => m.TableName == tableName, update, updateOptions);
                }
                return model.ID;
            }
        }

        /// <summary>
        /// 添加数据，不自动生成自增的数字ID
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="t">实体对象</param>
        /// <returns>返回添加后的ID</returns>
        public virtual bool AddNoAutoId<T>(T t) where T : class
        {
            string tableName = GettableName<T>();
            _db.GetCollection<T>(tableName).InsertOne(t);
            return true;
        }
        /// <summary>
        ///  批量增加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <returns></returns>

        public virtual bool AddManyNoAutoId<T>(IEnumerable<T> tlist) where T : class
        {
            string tableName = GettableName<T>();
            _db.GetCollection<T>(tableName).InsertMany(tlist);
            return true;
        }

        /// <summary>
        /// 异步添加，不自动生成自增的数字ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddNoAutoIdAsync<T>(T t) where T : class
        {
            string tableName = GettableName<T>();
            await _db.GetCollection<T>(tableName).InsertOneAsync(t);
            return true;
        }
        /// <summary>
        /// 异步批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddManyNoAutoIdAsync<T>(IEnumerable<T> tlist) where T : class
        {
            string tableName = GettableName<T>();
            await _db.GetCollection<T>(tableName).InsertManyAsync(tlist);
            return true;
        }
        /// <summary>
        /// 添加实体，默认带自增Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Add<T>(T t) where T : class
        {
            string tableName = GettableName<T>();
            var props = t.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                if (p.Name == "ID")
                {
                    int id = this.GetId(tableName);
                    p.SetValue(t, id, null);
                }
            }
            return this.AddNoAutoId(t);
        }

        /// <summary>
        /// 添加实体，默认带自增Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <returns></returns>
        public virtual bool AddMany<T>(IEnumerable<T> tlist) where T : class
        {
            foreach (T t in tlist)
            {
                string tableName = GettableName<T>();
                var props = t.GetType().GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.Name == "ID")
                    {
                        int id = this.GetId(tableName);
                        p.SetValue(t, id, null);
                    }
                }
            }
            return AddManyNoAutoId(tlist);
        }

        /// <summary>
        /// 异步添加，带自增ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddAsync<T>(T t) where T : class
        {
            string tableName = GettableName<T>();
            var props = t.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                if (p.Name == "ID")
                {
                    int id = this.GetId(tableName);
                    p.SetValue(t, id, null);
                }
            }
            return await this.AddNoAutoIdAsync(t);
        }

        /// <summary>
        /// 异步添加，带自增ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tlist"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddManyAsync<T>(IEnumerable<T> tlist) where T : class
        {
            foreach (T t in tlist)
            {
                string tableName = GettableName<T>();
                var props = t.GetType().GetProperties();
                foreach (PropertyInfo p in props)
                {
                    if (p.Name == "ID")
                    {
                        int id = this.GetId(tableName);
                        p.SetValue(t, id, null);
                    }
                }
            }
            return await this.AddManyNoAutoIdAsync(tlist);
        }


        /// <summary>
        /// 修改整个文档
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Update<T>(T t) where T : class
        {
            var doc = t.ToBsonDocument();
            var updateOptions = new UpdateOptions { IsUpsert = true };
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(f => f["_id"], doc["_id"]);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Combine(BuildUpdateDefinition(doc, null));
            string tableName = GettableName<T>();
            _db.GetCollection<BsonDocument>(tableName).UpdateOne(filter, update, updateOptions);

            return true;
        }

        /// <summary>
        /// 异步修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync<T>(T t) where T : class
        {
            var doc = t.ToBsonDocument();
            var updateOptions = new UpdateOptions { IsUpsert = true };
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(f => f["_id"], doc["_id"]);
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Combine(BuildUpdateDefinition(doc, null));
            string tableName = GettableName<T>();
            await _db.GetCollection<BsonDocument>(tableName).UpdateOneAsync(filter, update, updateOptions);
            return true;
        }

        public virtual bool Remove<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).DeleteOne<T>(predicate).DeletedCount > 0;
        }

        public virtual async Task<bool> RemoveAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            var result = await _db.GetCollection<T>(tableName).DeleteOneAsync<T>(predicate);
            return result.DeletedCount > 0;
        }

        public virtual bool RemoveMany<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).DeleteMany<T>(predicate).DeletedCount > 0;
        }

        public virtual async Task<bool> RemoveManyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            var result = await _db.GetCollection<T>(tableName).DeleteManyAsync<T>(predicate);
            return result.DeletedCount > 0;
        }


        public virtual T FirstOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).AsQueryable().FirstOrDefault(predicate);
        }

        public virtual async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return await _db.GetCollection<T>(tableName).AsQueryable().FirstOrDefaultAsync(predicate);
        }

        public virtual int Count<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).AsQueryable().Count(predicate);
        }

        public virtual async Task<long> LongCountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return await _db.GetCollection<T>(tableName).AsQueryable().LongCountAsync(predicate);
        }

        public virtual long LongCount<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).AsQueryable().LongCount(predicate);
        }

        public virtual async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return await _db.GetCollection<T>(tableName).AsQueryable().CountAsync(predicate);
        }
        public virtual bool Any<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).AsQueryable().Any(predicate);
        }
        public virtual async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            string tableName = GettableName<T>();
            return await _db.GetCollection<T>(tableName).AsQueryable().AnyAsync(predicate);
        }
        public virtual IMongoQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class
        {

            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).AsQueryable().Where(predicate);
        }

        public virtual IMongoQueryable<T> All<T>() where T : class
        {
            string tableName = GettableName<T>();
            return _db.GetCollection<T>(tableName).AsQueryable();

        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns></returns>
        private string GettableName<T>() where T : class
        {
            string tableName = Activator.CreateInstance(typeof(T)).GetType().Name;
            return tableName;
        }

        /// <summary>
        /// 构建更新操作定义 
        /// </summary>
        /// <param name="bc">BsonDocument</param>
        /// <param name="parent">父级</param>
        /// <returns></returns>
        private List<UpdateDefinition<BsonDocument>> BuildUpdateDefinition(BsonDocument bc, string parent)
        {
            var updates = new List<UpdateDefinition<BsonDocument>>();
            foreach (var element in bc.Elements)
            {
                var key = parent == null ? element.Name : $"{parent}.{element.Name}";
                var subUpdates = new List<UpdateDefinition<BsonDocument>>();
                //子元素是对象
                if (element.Value.IsBsonDocument)
                {
                    updates.AddRange(BuildUpdateDefinition(element.Value.ToBsonDocument(), key));
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
                            updates.AddRange(BuildUpdateDefinition(doc.ToBsonDocument(), key + $".{i}"));
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

        public void Dispose()
        {
            //通知垃圾回收器不再调用终结器
            GC.SuppressFinalize(this);
        }

    }



    partial class counters
    {
        public ObjectId _id { get; set; }

        public string TableName { get; set; }

        public int ID { get; set; } = 0;
    }
}
