using System.Linq.Expressions;
using MongoDB.Driver;
using NetCoreIoT.BasicsConfig;
using NetCoreIoT.Common;
using NetCoreIoT.Model.MongoDB;

namespace NetCoreIoT.DB.Mongod
{
    public class MongBase<T>
    {
        // 实例化 MongoDBHelper 类，指定文档类型
        private MongoDBHelperFactory factory = new MongoDBHelperFactory();

        private LogManager _log = new LogManager();
        private readonly ConfigurationManager configuration = new ConfigurationManager();
        private readonly string _MasterConnectionString;

        public MongBase()
        {
            var config = configuration.GetConfigValue();
            _MasterConnectionString = AESHelper.Decrypt(config.BasicsMasterHistoryConnectString, BasicsKeys.keys, BasicsKeys.iv);
            // _SlaveConnectionString = AESHelper.Decrypt(config.BasicsSlavedbString, BasicsKeys.keys, BasicsKeys.iv);
            string str = string.Empty;
            _MasterConnectionString = _MasterConnectionString;
        }

        /// <summary>
        /// 插入集合
        /// </summary>
        /// <param name="connect"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public bool Insert(MongoConnect connect, T document)
        {
            try
            {
                var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
                mongoDBHelper.Insert(document);
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("MongodbInsertError:" + ex.Message);
            }
            return false;
        }

        /// 异步插入
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> InsertManyDynamicAsync(MongoConnect connect, IEnumerable<dynamic> documents)
        {
            try
            {
                var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
                await mongoDBHelper.InsertManyDynamicAsync(documents);
                return true;
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("MongodbInsertError:" + ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 插入集合
        /// </summary>
        /// <param name="connect"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(MongoConnect connect, T document)
        {
            try
            {
                var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
                return await mongoDBHelper.InsertAsync(document);
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("MongodbInsertError:" + ex.Message);
            }
            return false;
        }

        /// <summary>
        ///查询
        /// </summary>
        /// <param name="connect"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<T> Find(MongoConnect connect, Expression<Func<T, bool>> filter)
        {
            var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
            return mongoDBHelper.Find(filter).ToList();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filter"></param>
        public bool DeleteMany(MongoConnect connect, Expression<Func<T, bool>> filter)
        {
            var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
            return mongoDBHelper.DeleteMany(filter);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="filter"></param>
        public bool DeleteMany(MongoConnect connect, FilterDefinition<T> filter)
        {
            var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
            return mongoDBHelper.DeleteMany(filter);
        }

        /// <summary>
        ///查询
        /// </summary>
        /// <param name="connect"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<T> Find(MongoConnect connect, FilterDefinition<T> filter)
        {
            var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
            return mongoDBHelper.Find(filter).ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        public List<T> Find(MongoConnect connect, FilterDefinition<T> filter, SortDefinition<T> sort)
        {
            // 创建排序条件，按 'InsertTime' 字段降序排序
            //var sort = Builders<Bracelet>.Sort.Descending(doc => doc.InsertTime);
            var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
            return mongoDBHelper.Find(filter, sort);
        }

        /// <summary>
        /// FindLimeOne
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public T FindLimeOne(MongoConnect connect, FilterDefinition<T> filter, SortDefinition<T> sort)
        {
            // 创建排序条件，按 'InsertTime' 字段降序排序
            //var sort = Builders<Bracelet>.Sort.Descending(doc => doc.InsertTime);
            var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
            return mongoDBHelper.FindLimeOne(filter, sort);
        }

        //public List<T> Find(EntityMongoConnect connect, FilterDefinition<T> filter,SortDefinition<BsonDocument> sort)
        //{
        //    var mongoDBHelper = factory.GetMongoDBHelper<T>(_MasterConnectionString, connect.dbName, connect.collectionName);
        //    return mongoDBHelper.Find(filter).Sort(sort);
        //}
    }
}
