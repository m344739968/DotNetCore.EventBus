namespace DotNetCore.EventBus.Host.Processor;

    /// <summary>
    /// 处理器接口
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// 处理方法
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> ProcessAsync(string topic, string value);
    }

    /// <summary>
    /// 处理器工厂接口
    /// </summary>
    public interface IProcessorFacotory
    {
        /// <summary>
        /// 创建默认处理器实例
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<IProcessor> CreateDefaultAsync();
    }

    /// <summary>
    /// 默认处理器工厂类
    /// </summary>
    public class DefaultProcessorFacotory : IProcessorFacotory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IProcessor> _processors;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="db"></param>
        /// <param name="processors"></param>
        public DefaultProcessorFacotory(IServiceProvider serviceProvider, IEnumerable<IProcessor> processors)
        {
            _serviceProvider = serviceProvider;
            _processors = processors;
        }

        /// <summary>
        /// 创建默认处理器实例
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<IProcessor> CreateDefaultAsync()
        {
            await Task.CompletedTask;
            var result = _processors?.FirstOrDefault();
            return result;
        }
    }
