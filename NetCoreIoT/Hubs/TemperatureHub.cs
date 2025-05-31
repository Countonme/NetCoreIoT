

using Microsoft.AspNetCore.SignalR;

namespace NetCoreIoT.Hubs
{
    public class TemperatureHub:Hub
    {
        public async Task SendTemperature(string deviceId, double value)
        {
            await Clients.All.SendAsync("ReceiveTemperature", deviceId, value);
        }
    }
}
