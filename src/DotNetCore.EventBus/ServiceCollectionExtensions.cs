using DotNetCore.EventBus.Host;
using DotNetCore.EventBus.Infrastructure.Models.Options;
using DotNetCore.EventBus.Infrastructure.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册 EventBus
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration, Action<EventbusOptions> setupAction)
        {
            services.Configure<MySqlOptions>(configuration.GetSection("Mysql"));
            services.Configure<EventbusOptions>(configuration.GetSection("EventBus"));
            var redisConn = configuration["Redis:ConnectionString"];
            services.AddCsRedis(redisConn);

            services.AddSingleton<MysqlInitialization>();
            // 监听消息
            services.AddMonitorService();
            return services;
        }
    }
}
