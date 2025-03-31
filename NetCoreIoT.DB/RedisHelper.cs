using NetCoreIoT.BasicsConfig;
using NetCoreIoT.Common;
using StackExchange.Redis;

namespace NetCoreIoT.DB
{
    /// <summary>
    /// Redis操作辅助类。
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// 静态构造函数用于初始化Redis连接。
        /// </summary>
        private static readonly ConnectionMultiplexer _redis;

        static RedisHelper()
        {
            var configuration = new ConfigurationManager();
            var config = configuration.GetConfigValue();
            var connectString = AESHelper.Decrypt(config.RedisConnectString, BasicsKeys.keys, BasicsKeys.iv);
            _redis = ConnectionMultiplexer.Connect(connectString);
        }

        /// <summary>
        /// 获取指定数据库索引的数据库实例。
        /// </summary>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>IDatabase 实例。</returns>
        private IDatabase GetDatabase(int dbIndex)
        {
            return _redis.GetDatabase(dbIndex);
        }

        /// <summary>
        /// 设置字符串类型的值到指定数据库索引的Redis中。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>如果设置成功返回true，否则返回false。</returns>
        /// <exception cref="ArgumentException">当key为空时抛出。</exception>
        public bool SetValue(string key, string value, int dbIndex)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            try
            {
                var db = GetDatabase(dbIndex);
                return db.StringSet(key, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetValue: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 异步设置字符串类型的值到指定数据库索引的Redis中。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="value">要存储的值。</param>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>一个Task，其结果为如果设置成功返回true，否则返回false。</returns>
        /// <exception cref="ArgumentException">当key为空时抛出。</exception>
        public async Task<bool> SetValueAsync(string key, string value, int dbIndex)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            try
            {
                var db = GetDatabase(dbIndex);
                return await db.StringSetAsync(key, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetValueAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 从指定数据库索引的Redis中获取字符串类型的值。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>如果找到则返回对应的值，否则返回null。</returns>
        /// <exception cref="ArgumentException">当key为空时抛出。</exception>
        public string GetValue(string key, int dbIndex)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            try
            {
                var db = GetDatabase(dbIndex);
                return db.StringGet(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetValue: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 异步从指定数据库索引的Redis中获取字符串类型的值。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>一个Task，其结果为如果找到则返回对应的值，否则返回null。</returns>
        /// <exception cref="ArgumentException">当key为空时抛出。</exception>
        public async Task<string> GetValueAsync(string key, int dbIndex)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            try
            {
                var db = GetDatabase(dbIndex);
                return await db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetValueAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 从指定数据库索引的Redis中删除给定键。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>如果删除成功返回true，否则返回false。</returns>
        /// <exception cref="ArgumentException">当key为空时抛出。</exception>
        public bool Remove(string key, int dbIndex)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            try
            {
                var db = GetDatabase(dbIndex);
                return db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Remove: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查指定数据库索引的Redis中是否存在给定键。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="dbIndex">数据库索引。</param>
        /// <returns>如果存在返回true，否则返回false。</returns>
        /// <exception cref="ArgumentException">当key为空时抛出。</exception>
        public bool HasKey(string key, int dbIndex)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");

            try
            {
                var db = GetDatabase(dbIndex);
                return db.KeyExists(key);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HasKey: {ex.Message}");
                return false;
            }
        }
    }
}