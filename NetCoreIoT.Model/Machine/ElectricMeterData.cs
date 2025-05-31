using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.Machine
{
    public class ElectricMeterData
    {
        // 采集时间
        public DateTime Timestamp { get; set; }

        // 设备ID
        public string DeviceId { get; set; }

        // 用电量，单位千瓦时(kWh)
        public double EnergyConsumption { get; set; }

        // 电压，单位伏特(V)
        public double Voltage { get; set; }

        // 电流，单位安培(A)
        public double Current { get; set; }

        // 功率因数
        public double PowerFactor { get; set; }

        // 状态，比如正常、异常
        public string Status { get; set; }
    }
}
