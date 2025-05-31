using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.Machine
{
    public class TemperatureData
    {
        // 采集时间
        public DateTime Timestamp { get; set; }

        // 传感器唯一ID
        public string SensorId { get; set; }

        // 温度值，单位摄氏度
        public double TemperatureCelsius { get; set; }

        // 状态，比如正常、异常
        public string Status { get; set; }
    }
}
