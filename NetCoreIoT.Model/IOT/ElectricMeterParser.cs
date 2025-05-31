using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.Model.Machine;
using Newtonsoft.Json;

namespace NetCoreIoT.Model.IOT
{
    public class ElectricMeterParser:ISensorDataParser
    {
        public object Parse(string payload)
        {
            // 解析电表数据
            return JsonConvert.DeserializeObject<ElectricMeterData>(payload);
        }
    }
}
