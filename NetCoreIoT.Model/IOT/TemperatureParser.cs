using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.Model.Machine;
using Newtonsoft.Json;

namespace NetCoreIoT.Model.IOT
{
    public class TemperatureParser:ISensorDataParser
    {
        public object Parse(string payload)
        {
            // 解析温度数据
            return JsonConvert.DeserializeObject<TemperatureData>(payload);
        }
    }
}
