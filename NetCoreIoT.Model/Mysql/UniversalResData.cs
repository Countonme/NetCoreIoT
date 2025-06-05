using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.Mysql
{
    public class UniversalResData
    {

        /// <summary>
        /// 總數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 錯誤信息
        /// </summary>
        public string error_message { get; set; }

        /// <summary>
        /// 數據
        /// </summary>
        public DataTable data { get; set; }
        /// <summary>
        /// 錯誤信息標識
        /// </summary>
        public bool error_flag { get; set; }
    }
}
