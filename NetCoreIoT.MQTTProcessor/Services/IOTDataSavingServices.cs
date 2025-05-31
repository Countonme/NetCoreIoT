using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.DB.Mongod;
using NetCoreIoT.Model.Machine;
using NetCoreIoT.Model.MongoDB;

namespace NetCoreIoT.MQTTProcessor.Services
{
    public class IOTDataSavingServices
    {
        // 假设你有个统一的接口或方法来存储
        public static async Task SaveEntityAsync(string equipment_id,object entity)
        {
            if (entity == null) return;

            // 这里根据实体类型做不同存储
            switch (entity)
            {
                case PressureData p:
                    await SavePressureDataAsync(equipment_id, p);
                    break;
                case HumidityData h:
                    await SaveHumidityDataAsync(equipment_id, h);
                    break;
                case TemperatureData t:
                    await SaveTemperatureDataAsync(equipment_id, t);
                    break;
                case ElectricMeterData e:
                    await SaveElectricMeterDataAsync(equipment_id, e);
                    break;
                default:
                    Console.WriteLine("未支持的实体类型");
                    break;
            }
        }

        // 各个存储方法示例
        private static async Task SavePressureDataAsync(string equipment_id,PressureData data)
        {
            MongoConnect mongos = new MongoConnect()
            {
                dbName = $"IOT_MQTT_Broker_PressureData",
                collectionName = equipment_id.ToUpper()
            };
            MongBase<PressureData> mong = new MongBase<PressureData>();

            await mong.InsertAsync(mongos, data); ;
            Console.WriteLine($"保存PressureData，时间:{data.Timestamp}");
        }

        private static async Task SaveHumidityDataAsync(string equipment_id,HumidityData data)
        {
            MongoConnect mongos = new MongoConnect()
            {
                dbName = $"IOT_MQTT_Broker_HumidityData",
                collectionName = equipment_id.ToUpper()
            };
            MongBase<HumidityData> mong = new MongBase<HumidityData>();

            await mong.InsertAsync(mongos, data); ;
            Console.WriteLine($"保存HumidityData，时间:{data.Timestamp}");
           // return Task.CompletedTask;
        }

        private static async Task SaveTemperatureDataAsync(string equipment_id,TemperatureData data)
        {
            MongoConnect mongos = new MongoConnect()
            {
                dbName = $"IOT_MQTT_Broker_TemperatureData",
                collectionName = equipment_id.ToUpper()
            };
            MongBase<TemperatureData> mong = new MongBase<TemperatureData>();

            await mong.InsertAsync(mongos, data); ;
            Console.WriteLine($"保存TemperatureData，时间:{data.Timestamp}");
          //  return Task.CompletedTask;
        }

        private static async Task SaveElectricMeterDataAsync(string equipment_id,ElectricMeterData data)
        {
            MongoConnect mongos = new MongoConnect()
            {
                dbName = $"IOT_MQTT_Broker_ElectricMeterData",
                collectionName = equipment_id.ToUpper()
            };
            MongBase<ElectricMeterData> mong = new MongBase<ElectricMeterData>();
           
            await mong.InsertAsync(mongos, data); 
            Console.WriteLine($"保存ElectricMeterData，时间:{data.Timestamp}");
        }

    }
}
