namespace NetCoreIoT.DB.Mongod
{
    public class MongoDBHelperFactory
    {
        private readonly Dictionary<string, object> _mongoDbHelpers = new Dictionary<string, object>();

        public MongoDBHelper<T> GetMongoDBHelper<T>(string connectionString, string dbName, string collectionName)
        {
            var key = $"{connectionString}:{dbName}:{collectionName}";

            if (!_mongoDbHelpers.ContainsKey(key))
            {
                var mongoDbHelper = new MongoDBHelper<T>(connectionString, dbName, collectionName);
                _mongoDbHelpers[key] = mongoDbHelper;
            }

            return (MongoDBHelper<T>)_mongoDbHelpers[key];
        }
    }
}
