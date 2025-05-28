using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace NetCoreIoT.DB.Mongod
{
    public class MongoDBHelper<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDBHelper(string connectionString, string dbName, string collectionName)
        {
            var client = new MongoClient(connectionString);

            var database = client.GetDatabase(dbName);
            _collection = database.GetCollection<T>(collectionName);
            EnsureCollectionExists(database, collectionName);
        }

        private void EnsureCollectionExists(IMongoDatabase database, string collectionName)
        {
            // 检查集合是否已经存在
            var filter = new BsonDocument("name", collectionName);
            var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });
            if (!collections.Any())
            {
                // 集合不存在，创建它
                database.CreateCollection(collectionName);
            }
        }

        public bool Insert(T document)
        {
            try
            {
                if (document is dynamic)
                {
                    var jsonString = JsonConvert.SerializeObject(document);
                    BsonDocument bsonDocument = BsonDocument.Parse(jsonString);
                    // 使用 BsonSerializer.Deserialize<T> 进行反序列化
                    T documentS = BsonSerializer.Deserialize<T>(bsonDocument);
                    _collection.InsertOne(documentS);
                    return true;
                }
                else
                {
                    _collection.InsertOne(document);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // 处理错误
                Console.WriteLine("Insert error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(T document)
        {
            try
            {
                if (document is dynamic)
                {
                    var jsonString = JsonConvert.SerializeObject(document);
                    BsonDocument bsonDocument = BsonDocument.Parse(jsonString);
                    // 使用 BsonSerializer.Deserialize<T> 进行反序列化
                    T documentS = BsonSerializer.Deserialize<T>(bsonDocument);
                    await _collection.InsertOneAsync(documentS);
                    return true;
                }
                else
                {
                    await _collection.InsertOneAsync(document);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // 处理错误
                Console.WriteLine("Insert Mongodb error: " + ex.Message);
                return false;
            }
        }

        /// 异步插入
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> InsertManyDynamicAsync(IEnumerable<dynamic> documents)
        {
            try
            {
                var bsonDocs = new List<BsonDocument>();
                foreach (var doc in documents)
                {
                    var json = JsonConvert.SerializeObject(doc);
                    var bson = BsonDocument.Parse(json);
                    bsonDocs.Add(bson);
                }

                var collection = _collection.Database.GetCollection<BsonDocument>(_collection.CollectionNamespace.CollectionName);
                await collection.InsertManyAsync(bsonDocs);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Insert dynamic error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="filter">动态筛选条件</param>
        /// <returns></returns>
        public List<T> Find(FilterDefinition<T> filter)
        {
            var projection = Builders<T>.Projection.Exclude("_id"); // 排除 _id 字段

            return _collection.Find(filter).Project<T>(projection).ToList();
        }

        /// <summary>
        /// Async 查询
        /// </summary>
        /// <param name="filter">动态筛选条件</param>
        /// <returns></returns>
        public async Task<List<T>> FindAsync(FilterDefinition<T> filter)
        {
            var projection = Builders<T>.Projection.Exclude("_id"); // 排除 _id 字段

            return await _collection.FindAsync(filter).Result.ToListAsync();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        public List<T> Find(FilterDefinition<T> filter, SortDefinition<T> sort)
        {
            // 创建排序条件，按 'InsertTime' 字段降序排序
            //var sort = Builders<Bracelet>.Sort.Descending(doc => doc.InsertTime);
            var projection = Builders<T>.Projection.Exclude("_id"); // 排除 _id 字段

            return _collection.Find(filter).Project<T>(projection).Sort(sort).ToList();
        }

        /// <summary>
        /// FindLimeOne
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public T FindLimeOne(FilterDefinition<T> filter, SortDefinition<T> sort)
        {
            // 创建排序条件，按 'InsertTime' 字段降序排序
            //var sort = Builders<Bracelet>.Sort.Descending(doc => doc.InsertTime);
            var projection = Builders<T>.Projection.Exclude("_id"); // 排除 _id 字段

            return _collection.Find(filter).Project<T>(projection).Sort(sort).Limit(1).FirstOrDefault();
        }

        /// <summary>
        ///查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<T> Find(Expression<Func<T, bool>> filter)
        {
            var projection = Builders<T>.Projection.Exclude("_id"); // 排除 _id 字段

            return _collection.Find(filter).Project<T>(projection).ToList();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filter"></param>
        public bool DeleteMany(FilterDefinition<T> filter)
        {
            try
            {
                _collection.DeleteMany(filter);
                return true;
            }
            catch (Exception ex)
            {
                // FileLog.WriteErrorLog("MongExceptionDeleteMany", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filter"></param>
        public bool DeleteMany(Expression<Func<T, bool>> filter)
        {
            try
            {
                _collection.DeleteMany(filter);
                return true;
            }
            catch (Exception ex)
            {
                //  FileLog.WriteErrorLog("MongExceptionDeleteMany", ex.Message);
                return false;
            }
        }
    }
}
