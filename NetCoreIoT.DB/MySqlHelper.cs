using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using NetCoreIoT.BasicsConfig;
using NetCoreIoT.Common;
using NetCoreIoT.Model;
using NetCoreIoT.Model.Mysql;
using static System.Reflection.Metadata.BlobBuilder;

namespace NetCoreIoT.DB
{
    public class MySqlHelper
    {
    
        private readonly string _MasterConnectionString;
        private readonly string _SlaveConnectionString;
        private readonly ConfigurationManager configuration = new ConfigurationManager();

        public MySqlHelper()
        {
            var config = configuration.GetConfigValue();
          //  _MasterConnectionString = AESHelper.Decrypt(config.BasicsMasterdbString, BasicsKeys.keys, BasicsKeys.iv) + "Pooling=true;Max Pool Size=500;";
           // _SlaveConnectionString = AESHelper.Decrypt(config.BasicsSlavedbString, BasicsKeys.keys, BasicsKeys.iv) + "Pooling=true;Max Pool Size=500;";

            _MasterConnectionString = config.BasicsMasterdbString;
            _SlaveConnectionString = config.BasicsMasterdbString;

            string str = string.Empty;
        }

        /// <summary>
        /// GetPagedData
        /// </summary>
        /// <param name="tableName">tableName</param>
        /// <param name="whereClause">条件</param>
        /// <param name="parameters">参数</param>
        /// <param name="limit"></param>
        /// <param name="orderBy"></param>
        /// <param name="selectFields"></param>
        /// <returns></returns>
        public (DataTable dtList, int TotalCount) ExecutePagedQuery(string tableName, string whereClause, MySqlParameter[] parameters, EntityLimit limit, string orderBy = "", string selectFields = "*")
        {
            string sql = $@"
                SELECT {selectFields}
                FROM {tableName}
                {whereClause}
                {orderBy}
                LIMIT @Offset, @Limit";

            var allParameters = new List<MySqlParameter>(parameters)
                {
                new MySqlParameter("@Offset",  (limit.page - 1) * limit.limit),
                new MySqlParameter("@Limit", limit.limit)
                };

            // 打印完整的SQL语句

            string completeSql = sql;
            foreach (var param in allParameters)
            {
                completeSql = completeSql.Replace(param.ParameterName, param.Value.ToString());
            }
            Console.WriteLine(completeSql);

            DataTable dataTable = ExecuteQuery(sql, allParameters.ToArray());
            int totalCount = GetTotalCount(tableName, whereClause, parameters);

            return (dataTable, totalCount);
        }

        /// <summary>
        /// GetPagedData
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <param name="limit"></param>
        /// <param name="orderBy"></param>
        /// <param name="selectFields"></param>
        /// <returns></returns>
        public async Task<(DataTable dtList, int TotalCount)> ExecutePagedQueryAsync(string tableName, string whereClause, MySqlParameter[] parameters, EntityLimit limit, string orderBy = "", string selectFields = "*")
        {
            string sql = $@"
                SELECT {selectFields}
                FROM {tableName}
                {whereClause}
                {orderBy}
                LIMIT @Offset, @Limit";

            var allParameters = new List<MySqlParameter>(parameters)
                {
                new MySqlParameter("@Offset",  (limit.page - 1) * limit.limit),
                new MySqlParameter("@Limit", limit.limit)
                };

            // 打印完整的SQL语句

            string completeSql = sql;
            foreach (var param in allParameters)
            {
                completeSql = completeSql.Replace(param.ParameterName, param.Value.ToString());
            }
            Console.WriteLine(completeSql);

            DataTable dataTable = await ExecuteQueryAsync(sql, allParameters.ToArray());
            int totalCount = await GetTotalCountAsync(tableName, whereClause, parameters);

            return (dataTable, totalCount);
        }

        public async Task<DataTable> ExecuteQueryAsync(string sqlString, Dictionary<string, object> parameters)
        {
           
            await using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                var dataTable = new DataTable();
                try
                {
                    await connection.OpenAsync();

                    await using (var command = new MySqlCommand(sqlString, connection))
                    {
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                            }
                        }

                        await using (var reader = await command.ExecuteReaderAsync())
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine("Error executing query: " + ex.Message);
                }
                return dataTable;
            }
        }

        public (DataTable dtList, int TotalCount) ExecutePagedQuery(string ResultSqlString, string TotalSqlString, MySqlParameter[] parameters)
        {
            var data = ExecuteQuery(ResultSqlString, parameters);
            var TotalData = GetTotalCount(TotalSqlString, parameters);

            return (data, TotalData);
        }

        private DataTable ExecuteQuery(string query, MySqlParameter[] parameters)
        {
            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddRange(parameters);
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable resultTable = new DataTable();
                        adapter.Fill(resultTable);
                        return resultTable;
                    }
                }
            }
        }

        private async Task<DataTable> ExecuteQueryAsync(string query, MySqlParameter[] parameters, CancellationToken ct = default)
        {
            var table = new DataTable();

            await using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                await connection.OpenAsync(ct).ConfigureAwait(false);

                await using (var cmd = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    await using (var reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false))
                    {
                        table.Load(reader);
                    }
                }
            }

            return table;
        }

        private async Task<int> GetTotalCountAsync(string tableName, string whereClause, MySqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            string countQuery = $@"
        SELECT COUNT(*)
        FROM {tableName}
        {whereClause}";

            await using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var cmd = new MySqlCommand(countQuery, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    object result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                    return result == DBNull.Value ? 0 : Convert.ToInt32(result);
                }
            }
        }

        private int GetTotalCount(string tableName, string whereClause, MySqlParameter[] parameters)
        {
            string countQuery = $@"
        SELECT COUNT(*)
        FROM {tableName}
        {whereClause}";

            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                using (var cmd = new MySqlCommand(countQuery, connection))
                {
                    cmd.Parameters.AddRange(parameters);
                    connection.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    connection.Close();
                    return count;
                }
            }
        }

        /// <summary>
        /// 查询总行数
        /// </summary>
        /// <param name="countQuery"></param>
        /// <returns></returns>
        private int GetTotalCount(string countQuery, MySqlParameter[] parameters)
        {
            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                using (var cmd = new MySqlCommand(countQuery, connection))
                {
                    cmd.Parameters.AddRange(parameters);
                    connection.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    connection.Close();
                    return count;
                }
            }
        }

        public T ExecuteScalar<T>(string sqlString, MySqlConnection connection = null)
        {
         
            using (var conn = connection ?? new MySqlConnection(_SlaveConnectionString))
            {
                if (connection == null) conn.Open();
                try
                {
                    var command = new MySqlCommand(sqlString, conn);
                    object result = command.ExecuteScalar();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Error executing scalar query: " + ex.Message);
                    return default;
                }
                finally
                {
                    if (connection == null) conn.Close();
                }
            }
        }

        /// <summary>
        /// 數據查詢
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string sqlString)
        {
            
            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                var dataTable = new DataTable();
                try
                {
                    connection.Open();
                    var command = new MySqlCommand(sqlString, connection);
                    var dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                  
                    Console.WriteLine("Error executing query: " + ex.Message);
                }
                return dataTable;
            }
        }

        /// <summary>
        /// 數據查詢（异步）
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteQueryAsync(string sqlString)
        {
            

            await using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                var dataTable = new DataTable();
                try
                {
                    // 异步打开数据库连接
                    await connection.OpenAsync();

                    var command = new MySqlCommand(sqlString, connection);

                    // 使用 MySqlDataReader 异步读取数据
                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader); // 将读取的数据加载到 DataTable 中
                    }
                }
                catch (Exception ex)
                {
                  
                    Console.WriteLine("Error executing query: " + ex.Message);
                }
                return dataTable;
            }
        }

        /// <summary>
        /// 數據插入更新（异步）
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public async Task<Bools> sqlInsertUpdateDeleteAsync(string sqlString)
        {
            Bools bools = new Bools();
            await using (var connection = new MySqlConnection(_MasterConnectionString))
            {
                try
                {
                    // 异步打开数据库连接
                    await connection.OpenAsync();

                    var command = new MySqlCommand(sqlString, connection);

                    // 异步执行非查询命令
                    int count = await command.ExecuteNonQueryAsync();

                    if (count > 0)
                    {
                        bools.msg = "操作成功";
                        bools.Flag = true;
                    }
                    else
                    {
                        bools.Flag = false;
                        bools.msg = "操作失败";

                    }
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine("Error executing non-query: " + ex.Message);
                    bools.Flag = false;
                    bools.msg = $"操作失败 {ex.Message}";
                }
            }
            return bools;
        }

        public async Task<Bools> sqlInsertUpdateDeleteAsync(string sqlString, Dictionary<string, object>? parameters = null)
        {
            Bools bools = new Bools();
            await using (var connection = new MySqlConnection(_MasterConnectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    using var command = new MySqlCommand(sqlString, connection);

                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                        }
                    }

                    int count = await command.ExecuteNonQueryAsync();

                    if (count > 0)
                    {
                        bools.msg = "操作成功";
                        bools.Flag = true;
                    }
                    else
                    {
                        bools.Flag = false;
                        bools.msg = "操作失败";
                     
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error executing non-query: " + ex.Message);
                    bools.Flag = false;
                    bools.msg = $"操作失败 {ex.Message}";
                }
            }
            return bools;
        }

        /// <summary>
        /// 數據插入更新
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public Bools sqlInsertUpdateDelete(string sqlString)
        {
            Bools bools = new Bools();
          
            using (var connection = new MySqlConnection(_MasterConnectionString))
            {
                try
                {
                    connection.Open();
                    var command = new MySqlCommand(sqlString, connection);
                    int count = command.ExecuteNonQuery();
                    if (count > 0)
                    {
                        bools.msg = "操作成功";
                        bools.Flag = true;
                    }
                    else
                    {
                        bools.Flag = false;
                        bools.msg = "操作失败";
                     
                    }
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine("Error executing non-query: " + ex.Message);
                    bools.Flag = false;
                    bools.msg = $"操作失败 {ex.Message}";
                }
            }
            return bools;
        }

        /// <summary>
        /// Executes a paged query on the specified table, returning a DataTable of page data and the total row count in parallel.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="SqlString">The SQL query string, including any WHERE clauses.</param>
        /// <param name="pageNumber">The current page number for pagination.</param>
        /// <param name="pageSize">The number of rows per page.</param>
        /// <param name="parameters">Optional query parameters.</param>
        /// <returns>A tuple containing the DataTable of page data and an integer for the total row count.</returns>
        public async Task<UniversalResData> ExecutePagedQueryParallel(string tableName, string SqlString, int pageNumber, int pageSize, MySqlParameter[] parameters)
        {
            
            var res = new UniversalResData();
            // 构建行数查询的 SQL 语句
            string countQuery = $"SELECT COUNT(0) FROM {tableName}";
            if (parameters.Length > 0 && SqlString.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            {
                // 提取 WHERE 子句并添加到 countQuery
                string whereClause = SqlString.Substring(SqlString.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase));
                countQuery += $" {whereClause}";
            }

            // 启动获取分页数据和行数的并行任务
            var getDataTask = Task.Run(() => FetchPageData(SqlString, pageNumber, pageSize, parameters));
            var getCountTask = Task.Run(() => FetchTotalRowCount(countQuery, parameters));

            // 等待两个任务完成
            await Task.WhenAll(getDataTask, getCountTask);
            if (!string.IsNullOrEmpty(getDataTask.Result.error_message))
            {
                res.error_message = getDataTask.Result.error_message;
                return res;
            }
            res.data = getDataTask.Result.data;
            if (!string.IsNullOrEmpty(getCountTask.Result.error_message))
            {
                res.error_message += getCountTask.Result.error_message;
                return res;
            }
            res.TotalCount = getCountTask.Result.TotalCount;
            return res;
        }

        /// <summary>
        /// Retrieves paginated data based on the provided SQL query.
        /// </summary>
        /// <param name="SqlString">SQL query string with WHERE clause if any.</param>
        /// <param name="pageNumber">Current page number for pagination.</param>
        /// <param name="pageSize">Number of rows per page.</param>
        /// <param name="parameters">Optional query parameters.</param>
        /// <returns>A DataTable containing the paginated data.</returns>
        private UniversalResData FetchPageData(string SqlString, int pageNumber, int pageSize, MySqlParameter[] parameters)
        {
            var res = new UniversalResData();
            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                var dataTable = new DataTable();
                try
                {
                    connection.Open();

                    int offset = (pageNumber - 1) * pageSize;
                    SqlString += $" LIMIT @offset, @pageSize";

                    var command = new MySqlCommand(SqlString, connection);
                    command.Parameters.AddRange(parameters);
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@pageSize", pageSize);

                    var dataAdapter = new MySqlDataAdapter(command);
                    dataAdapter.Fill(dataTable);
                    res.data = dataTable;
                }
                catch (Exception ex)
                {
                   
                    res.error_flag = true;
                    res.error_message = "Error fetching paged data: " + ex.Message;
                }
                return res;
            }
        }

        /// <summary>
        /// Retrieves the total row count for the provided count query.
        /// </summary>
        /// <param name="countQuery">The SQL count query to execute.</param>
        /// <param name="parameters">Optional query parameters.</param>
        /// <returns>The total row count as an integer.</returns>
        private UniversalResData FetchTotalRowCount(string countQuery, MySqlParameter[] parameters)
        {
            var res = new UniversalResData();
            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                try
                {
                    connection.Open();
                    var countCommand = new MySqlCommand(countQuery, connection);
                    countCommand.Parameters.AddRange(parameters);
                    res.TotalCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    return res;
                }
                catch (Exception ex)
                {
                   
                    res.error_flag = true;
                    res.error_message = ("Error fetching total row count: " + ex.Message);
                    return res;
                }
            }
        }

        /// <summary>
        /// Execute a scalar query and return the result
        /// </summary>
        /// <typeparam name="T">Type of the result</typeparam>
        /// <param name="SqlString">SQL query</param>
        /// <returns>Result of the scalar query</returns>
        public T ExecuteScalar<T>(string SqlString)
        {
          
            using (var connection = new MySqlConnection(_SlaveConnectionString))
            {
                try
                {
                    connection.Open();
                    var command = new MySqlCommand(SqlString, connection);
                    object result = command.ExecuteScalar();
                    return (T)Convert.ChangeType(result, typeof(T));
                }
                catch (Exception ex)
                {
                   
                    Console.WriteLine("Error executing scalar query: " + ex.Message);
                    return default(T);
                }
            }
        }
    }
}
