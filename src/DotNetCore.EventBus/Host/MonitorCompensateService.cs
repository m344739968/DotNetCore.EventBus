using DotNetCore.EventBus.Infrastructure.Models.Options;
using Microsoft.Extensions.Options;
using DotNetCore.EventBus.Infrastructure.Models.Dto;
using DotNetCore.EventBus.Infrastructure.Models.EventBus;
using DotNetCore.EventBus.Infrastructure.Models.Enums;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotNetCore.EventBus.Infrastructure.Kafka;
using MySql.Data.MySqlClient;
using Dapper;

namespace DotNetCore.EventBus.Host;

/// <summary>
/// 事件总线补偿服务
/// </summary>
public class MonitorCompensateService : BackgroundService
{
    private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);
    private readonly ILogger<MonitorCompensateService> _logger;
    private readonly IOptions<AppSettings> _options;
    private readonly IOptions<EventbusOptions> _kafkaOptions;
    private readonly IOptions<MySqlOptions> _mySqlOptions;
    private readonly KafkaConsumerClient _kafkaConsumerClient;
    private readonly KafkaProduceClient _kafkaProduceClient;
    private const string _serviceDesc = "【事件总线补偿服务】";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="kafkaOptions"></param>
    /// <param name="processorFacotory"></param>
    public MonitorCompensateService(ILogger<MonitorCompensateService> logger,
        IOptions<AppSettings> options,
        IOptions<EventbusOptions> kafkaOptions,
        IOptions<MySqlOptions> mySqlOptions,
        KafkaConsumerClient kafkaConsumerClient,
        KafkaProduceClient kafkaProduceClient)
    {
        _logger = logger;
        _options = options;
        _kafkaOptions = kafkaOptions;
        _mySqlOptions = mySqlOptions;
        _kafkaConsumerClient = kafkaConsumerClient;
        _kafkaProduceClient = kafkaProduceClient;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{_serviceDesc}，10s后开始执行");
        await Task.Delay(1000 * 10); // 10s后开始
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
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pageIndex = 1;
                var pageSize = 100;
                var total = await this.GetEventPublishMessageRecordTotal();
                var data = await this.GetEventPublishMessageRecordList(pageIndex, pageSize);
                var totalPage = (total + pageSize - 1) / pageSize;
                for (var i = 1; i <= totalPage; i++)
                {
                    if (i > 1)
                    {
                        data = await this.GetEventPublishMessageRecordList(i, pageSize);
                    }
                    if (data == null || !data.Any())
                    {
                        continue;
                    }
                    foreach (var item in data)
                    {
                        //try
                        //{
                        var topicName = $"{_kafkaOptions.Value.TopicPrefix}_{item.EventName}";
                        var message = new EventBusPublishRequest()
                        {
                            ClientId = item.ClientId,
                            EventId = item.EventId,
                            EventName = item.EventName,
                            EventData = item.EventData,
                        };
                        // 重新推入到消息队列中
                        await _kafkaProduceClient.Produce(topicName, message);
                        //}
                        //catch (Exception ex) {
                        //    _logger.LogError(ex, $"{_serviceDesc}，推送消息，发生异常了");
                        //    await _noticeSendServcie.Send(JobCommandType.Exception, $"{_serviceDesc}，推送消息，发生异常，{ex.ToString()}");
                        //}
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_serviceDesc}，发生异常了");
                // break;
            }
            await Task.Delay(1000 * _kafkaOptions.Value.RetryInterval); // 60s*10 休眠
        }
    }

    public async Task<long> GetEventPublishMessageRecordTotal()
    {
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        var result = await conn.QuerySingleOrDefaultAsync<long>(
            $"select count(1) from {_kafkaOptions.Value.TopicPrefix}_publish_message_record where status=@status and tryCount=@tryCount;",
            new { status = (int)(int)EventStatusEnums.Failed, tryCount = _kafkaOptions.Value.RetryCount });
        return result;
    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="total"></param>
    /// <returns></returns>
    private async Task<List<EventPublishMessageRecord>> GetEventPublishMessageRecordList(int pageIndex, int pageSize)
    {
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        var result = await conn.QueryAsync<EventPublishMessageRecord>(
            $"select * from {_kafkaOptions.Value.TopicPrefix}_publish_message_record where status=@status and tryCount=@tryCount limit @start,@limit",
            new
            {
                status = (int)(int)EventStatusEnums.Failed,
                tryCount = _kafkaOptions.Value.RetryCount,
                start = (pageIndex - 1) * pageSize,
                limit = pageSize
            });
        return result.ToList();
    }
}