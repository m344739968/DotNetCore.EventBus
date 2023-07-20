using DotNetCore.EventBus.Host.Processor;
using DotNetCore.EventBus.Infrastructure.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotNetCore.EventBus.Infrastructure.Kafka;

namespace DotNetCore.EventBus.Host;

/// <summary>
/// 事件总线服务
/// </summary>
public class MonitorService : BackgroundService
{
    private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);
    private readonly ILogger<MonitorService> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly IOptions<EventbusOptions> _kafkaOptions;
    private readonly IProcessorFacotory _processorFacotory;
    private readonly KafkaConsumerClient _kafkaConsumerClient;
    private const string _serviceDesc = "【事件总线服务】";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="kafkaOptions"></param>
    /// <param name="processorFacotory"></param>
    public MonitorService(ILogger<MonitorService> logger,
        IOptions<AppSettings> options,
        IOptions<EventbusOptions> kafkaOptions,
        IProcessorFacotory processorFacotory,
        KafkaConsumerClient kafkaConsumerClient)
    {
        _logger = logger;
        _options = options;
        _kafkaOptions = kafkaOptions;
        _processorFacotory = processorFacotory;
        _kafkaConsumerClient = kafkaConsumerClient;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{_serviceDesc}，3s后开始执行");
        await Task.Delay(1000 * 3); // 5s后开始
        await Task.Factory.StartNew(async () =>
        {
            await Start(stoppingToken);
        }, stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }
    /// <summary>
    /// 停止
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// 开始执行监听
    /// </summary>
    /// <returns></returns>
    private async Task Start(CancellationToken stoppingToken)
    {
        try
        {
            var topics = _kafkaConsumerClient.FetchTopics();

            // 订阅
            await _kafkaConsumerClient!.Subscribe(topics);

            // 监听
            await this.Listening(_pollingDelay, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{_serviceDesc}，kafka监听异常");
        }
    }

    /// <summary>
    /// 监听消息
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    public async Task Listening(TimeSpan timeout, CancellationToken cancellationToken)
    {
        await _kafkaConsumerClient.Connect();

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("【事件总线】，开始监听消息");
                var consumerResult = await _kafkaConsumerClient!.Consume(); // timeout
                if (consumerResult == null)
                {
                    // _logger.LogDebug("【事件总线】，未拉取到kafka消息");
                    continue;
                }

                if (consumerResult.IsPartitionEOF || consumerResult.Message.Value == null)
                {
                    _logger.LogDebug("【事件总线】，拉取到的kafka消息内容为空");
                    continue;
                }

                // 处理消息
                var process = await _processorFacotory.CreateDefaultAsync();
                if (process == null)
                {
                    _logger.LogDebug("【事件总线】，未找到消息处理代理类");
                    continue;
                }
                var message = consumerResult.Message.Value; // Encoding.UTF8.GetString(consumerResult.Message.Value);
                var processResult = await process!.ProcessAsync(consumerResult.Topic, message);
                _logger.LogDebug("【事件总线】，处理结果：{@processResult}", processResult);

                await _kafkaConsumerClient.Commit();
                // _logger.LogDebug("【事件总线】，提交偏移量，Committed offset: {@committedOffsets}", committedOffsets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "【事件总线】，处理异常");
                // break;
            }
        }
        _logger.LogDebug("【事件总线】，监听消息结束");
        // ReSharper disable once FunctionNeverReturns
    }

}