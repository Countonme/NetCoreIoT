using NetCoreIoT.Model.ConfigData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.BasicsConfig
{
    public class ConfigurationManager
    {
        private string ConfigFilePath { get; }

        public ConfigurationManager()
        {
            // 根据操作系统类型确定配置文件路径
            if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                ConfigFilePath = "/etc/MySystem/NetCoreIoT.conf"; // Linux 或 MacOSX
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // Windows
                // string windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                ConfigFilePath = Path.Combine(@"C:\MySystem", "NetCoreIoT.conf");
            }
            else
            {
                throw new NotSupportedException("Unsupported operating system.");
            }

            // 检查配置文件是否存在，如果不存在则创建
            if (!File.Exists(ConfigFilePath))
            {
                try
                {
                    // 创建目录
                    string directory = Path.GetDirectoryName(ConfigFilePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    var configString = new EntityConfigData();
                    // 创建配置文件
                    File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(configString));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// GetConfigValue
        /// </summary>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public EntityConfigData GetConfigValue()
        {
            // 读取配置文件中指定键的值
            if (!File.Exists(ConfigFilePath))
            {
                throw new FileNotFoundException("Config file not found.", ConfigFilePath);
            }
            var jsonString = File.ReadAllText(ConfigFilePath);
            // 解析 JSON 字符串为 AppConfig 实例
            EntityConfigData config = JsonConvert.DeserializeObject<EntityConfigData>(jsonString);
            return config;
        }
    }
}