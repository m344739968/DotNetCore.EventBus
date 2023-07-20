using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.EventBus.Infrastructure.Redis
{
    public static class RedisExtension
    {
        /// <summary>
        /// 注册 RedLock
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCsRedis(this IServiceCollection services, string redisEndpoint)
        {
            var csredis = new CSRedisClient(redisEndpoint);
            RedisHelper.Initialization(csredis);
            services.AddSingleton(csredis);
            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
            return services;
        }
    }


}
