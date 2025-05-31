
using System.Text;
using System.Threading.Channels;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Formatter;
using MQTTnet.Implementations;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using NetCoreIoT.Model.IOT;
namespace NetCoreIoT.MQTTProcessor.Services
{
    public class MqttSubscriber
    {
        private IMqttClient _mqttClient;
        private string subscribeTopic = "IOT/#";
        private Channel<(string topic, string payload)> _channel = Channel.CreateUnbounded<(string, string)>();
        private readonly Dictionary<string, ISensorDataParser> _parsers = new Dictionary<string, ISensorDataParser>()
        {

             { "pressure", new PressureParser() },
    { "humidity", new HumidityParser() },
    { "temperature", new TemperatureParser() },
    { "electricmeter", new ElectricMeterParser() },
            // 继续添加对应 Topic 和解析器
        };
        private (string category, string deviceId, string metric) ParseTopic(string topic)
        {
            var parts = topic.Split('/');
            if (parts.Length != 3)
                throw new ArgumentException("Topic 格式不正确，应为三段，如 IOT/TEST001/electricmeter");

            return (parts[0], parts[1], parts[2]);
        }

        public async Task StartMessageProcessorAsync(CancellationToken token)
        {
            await foreach (var (topic, payload) in _channel.Reader.ReadAllAsync(token))
            {
                var (category, equipment_id, metric) = ParseTopic(topic);

                if (metric == null) {
                    metric = topic;
                }
               
                if (_parsers.TryGetValue(metric, out var parser))
                {
                    var entity = parser.Parse(payload);
                    // TODO: 异步存库，或其他业务处理
                    await IOTDataSavingServices.SaveEntityAsync(equipment_id, entity);
                }
                else
                {
                    Console.WriteLine($"未找到对应的解析器，Topic: {topic}");
                }
            }
        }

        public MqttSubscriber()
        {
            // 初始化 MQTT 客户端
            // 创建客户端
            _mqttClient = new MqttClient(new MqttClientAdapterFactory(), new MqttNetEventLogger());

        }
        public async Task StartAsync()
        {

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("8.140.235.110", 1883)
                .WithCredentials("IOTuser", "IOTuser13579..")
                .WithClientId("MQTT_Server")
                .WithCleanSession()
                .Build();

          
            _mqttClient.ApplicationMessageReceivedAsync += _mqttClient_ApplicationMessageReceivedAsync;
            // MQTT 连接成功时触发
            _mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync;

            //MQTT 断开连接时触发
            _mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync;

            await _mqttClient.ConnectAsync(options);
        }

        /// <summary>
        /// MQTT 断开连接时触发
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine("MQTT 已断开");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 服务器连接时触发
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("已连接 MQTT 服务器");
            _mqttClient.SubscribeAsync(subscribeTopic, MqttQualityOfServiceLevel.AtLeastOnce); //topic_02

            return Task.CompletedTask;
        }

        /// <summary>
        /// 接收到消息时触发
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private Task _mqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            var payload = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            var topic = arg.ApplicationMessage.Topic;

            // 写入 Channel，快速返回
            _channel.Writer.TryWrite((topic, payload));

            return Task.CompletedTask;
        }

    }
}