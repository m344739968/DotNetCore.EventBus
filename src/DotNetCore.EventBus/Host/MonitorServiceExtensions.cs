using DotNetCore.EventBus.Host.Processor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCore.EventBus.Host;

    public static class MonitorServiceExtensions
    {
    /// <summary>
    /// ×¢Èë¼àÌý·þÎñ
    /// </summary>
    /// <param name="service"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
        public static IServiceCollection AddMonitorService(this IServiceCollection service)
        {
            service.AddHostedService<MonitorService>();
            service.AddHostedService<MonitorCompensateService>();
            service.AddSingleton<IProcessorFacotory, DefaultProcessorFacotory>();
            service.AddSingleton<IProcessor, DefaultProcessor>();
            return service;
        }
    }
