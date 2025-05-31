
using Microsoft.AspNetCore.SignalR;

namespace NetCoreIoT.Hubs
{
    public class EquipmentHub:Hub
    {
        // 可以拓展自定义方法，前端可以调用
        public async Task SendMessage(string deviceId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", deviceId, message);
        }
    }
}
