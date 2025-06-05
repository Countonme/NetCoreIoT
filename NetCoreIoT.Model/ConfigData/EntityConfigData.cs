using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.ConfigData
{
    public class EntityConfigData
    {
        /// <summary>
        /// 基础数据库主节点
        /// </summary>
        public string BasicsMasterHistoryConnectString { get; set; } = string.Empty;


        /// <summary>
        /// Redis 的连接信息
        /// </summary>
        public string RedisConnectString { get; set; } = string.Empty;

        /// <summary>
        /// Mysql数据库连接字符串
        /// </summary>
        public string BasicsMasterdbString { get; set; } = string.Empty;
    }
}
