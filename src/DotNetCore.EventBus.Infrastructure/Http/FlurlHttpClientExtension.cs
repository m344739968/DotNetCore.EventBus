using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.EventBus.Infrastructure.Http;

    public static class FlurlHttpClientExtension
    {
        public static IServiceCollection AddFlurlHttpClient(this IServiceCollection services, IConfiguration configuration = null)
        {
            // 单例模式
            return services.AddSingleton<FlurlHttpClient>();
        }
    }
