
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
            // ��� SignalR ����
            builder.Services.AddSignalR();
            builder.Services.AddRazorPages(); // �� ��������䣬���� RazorPages �޷���������
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

            app.UseStaticFiles(); // ���� wwwroot �µľ�̬�ļ�
            app.MapControllers();
            app.MapRazorPages(); // �����Ҫ Razor Pages ֧��
            app.MapHub<EquipmentHub>("/deviceHub");
            app.MapHub<TemperatureHub>("/temperatureHub"); // SignalR �˵�

            // ��������
            var pushService = app.Services.GetRequiredService<TemperaturePushService>();
            var cts = new CancellationTokenSource();
            // ���ȴ����첽������������
            _ = pushService.StartTemperaturePushAsync(cts.Token);
            app.Run();

        }


    }
}
