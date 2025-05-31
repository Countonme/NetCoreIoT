using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.Machine
{
    public class PressureData
    {
        // 采集时间，建议用 DateTime 类型
        public DateTime Timestamp { get; set; }

        // 传感器唯一ID
        public string SensorId { get; set; }

        // 气压值，单位帕斯卡（Pa）
        public double PressureValue { get; set; }

        // 状态，比如正常、异常等
        public string Status { get; set; }
    }
}
