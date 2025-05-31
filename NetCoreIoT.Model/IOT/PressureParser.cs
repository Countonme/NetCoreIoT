using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.Model.Machine;
using Newtonsoft.Json;

namespace NetCoreIoT.Model.IOT
{
    public class PressureParser:ISensorDataParser
    {
        public object Parse(string payload)
        {
            // 解析气压数据
            return JsonConvert.DeserializeObject<PressureData>(payload);
        }
    }
}
