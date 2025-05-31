using NetCoreIoT.MQTTProcessor.Services;

namespace NetCoreIoT.MQTTProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        MqttSubscriber mqtt = new MqttSubscriber();
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            mqtt.StartAsync();
            await mqtt.StartMessageProcessorAsync(stoppingToken);
        }
    }
}
