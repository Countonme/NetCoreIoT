using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.MQTT
{
    public class DeviceHeartbeat
    {
        /// <summary>
        /// 设备唯一ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 心跳发送时间（UTC时间）
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 设备状态，如在线、离线、故障等
        /// </summary>
        public string Status { get; set; }
    }
}
