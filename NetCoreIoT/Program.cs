
using Microsoft.AspNetCore.SignalR;
using NetCoreIoT.Dal.User;
using NetCoreIoT.Hubs;
using NetCoreIoT.Services.User;

namespace NetCoreIoT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 添加 SignalR 服务
            builder.Services.AddSignalR();
            builder.Services.AddRazorPages(); // ← 必须有这句，否则 RazorPages 无法正常工作
            builder.Services.AddSingleton<IUserServices, UserServices>();
            builder.Services.AddSingleton<IDalUserMapper, DalUserMapper>();
        
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<TemperaturePushService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles(); // 启用 wwwroot 下的静态文件
            app.MapControllers();
            app.MapRazorPages(); // 如果需要 Razor Pages 支持
            app.MapHub<EquipmentHub>("/deviceHub");
            app.MapHub<TemperatureHub>("/temperatureHub"); // SignalR 端点

            // 解析依赖
            var pushService = app.Services.GetRequiredService<TemperaturePushService>();
            var cts = new CancellationTokenSource();
            // 不等待，异步启动推送任务
            _ = pushService.StartTemperaturePushAsync(cts.Token);
            app.Run();

        }


    }
}
