using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Common
{
    public class Bools
    {
        /// <summary>
        /// 返回结果状态
        /// </summary>
        public bool Flag { get; set; } = false;
        /// <summary>
        /// False and True  Desc
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// code
        /// </summary>
        public int code { get; set; } = 0;

        /// <summary>
        /// 获取jwt 验证时采用 emp返回操作人员信息
        /// </summary>
        public string emp { get; set; }
    }
}
