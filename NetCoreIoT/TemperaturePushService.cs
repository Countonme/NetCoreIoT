using Microsoft.AspNetCore.SignalR;
using NetCoreIoT.Hubs;

namespace NetCoreIoT
{
    public class TemperaturePushService
    {
        private readonly IHubContext<TemperatureHub> _hubContext;
        private Timer _timer;

        public TemperaturePushService(IHubContext<TemperatureHub> hubContext)
        {
            _hubContext = hubContext;
            // 启动定时器，每5秒触发一次回调
            _timer = new Timer(async _ =>
            {
                var randomTemp = new Random().NextDouble() * 40; // 0~40度随机温度
                await _hubContext.Clients.All.SendAsync("ReceiveTemperature", "TEST001", Math.Round(randomTemp, 2));
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public async Task StartTemperaturePushAsync(CancellationToken token)
        {
            var random = new Random();
            while (!token.IsCancellationRequested)
            {
                var temp = random.NextDouble() * 40;
                await _hubContext.Clients.All.SendAsync("ReceiveTemperature", "TEST001", Math.Round(temp, 2));
                await Task.Delay(5000, token);
            }
        }

    }
}
