using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.Machine
{
    public class HumidityData
    {
        // 采集时间
        public DateTime Timestamp { get; set; }

        // 传感器唯一ID
        public string SensorId { get; set; }

        // 湿度值，百分比
        public double HumidityPercent { get; set; }

        // 状态，比如正常、异常等
        public string Status { get; set; }
    }
}
