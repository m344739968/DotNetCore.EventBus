using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DotNetCore.EventBus.Infrastructure.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetCore.EventBus.Infrastructure.Kafka;
/// <summary>
/// kafka消费客户端
/// </summary>
public class KafkaConsumerClient : IDisposable
{
    private static readonly SemaphoreSlim ConnectionLock = new(initialCount: 1, maxCount: 1);

    private readonly ILogger<KafkaConsumerClient> _logger;
    private readonly IOptions<EventbusOptions> _kafkaOptions;
    private IConsumer<Ignore, string>? _consumerClient;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <param name="db"></param>
    public KafkaConsumerClient(ILogger<KafkaConsumerClient> logger, IOptions<EventbusOptions> options)
    {
        _logger = logger;
        _kafkaOptions = options;
    }

    /// <summary>
    /// 获取所有topic
    /// </summary>
    /// <returns></returns>
    public List<string> FetchTopics()
    {
        var result = new List<string>();
        //var eventNames = _db.Queryable<RegisterEventList>().Select(x => x.EventName).ToList();
        //foreach (var item in eventNames)
        //{
        //    result.Add($"{_kafkaOptions.Value.TopicPrefix}_{item}");
        //}
        result.Add($"^{_kafkaOptions.Value.TopicPrefix}_.*"); // 正则表达式订阅topic
        return result;
    }

    /// <summary>
    /// 创建topic
    /// </summary>
    /// <param name="topicNames"></param>
    /// <returns></returns>
    public async Task<bool> CreateTopics(List<string> topics)
    {
        try
        {
            var config = new AdminClientConfig() { BootstrapServers = _kafkaOptions.Value.BootstrapServers };

            using var adminClient = new AdminClientBuilder(config).Build();

            await adminClient.CreateTopicsAsync(topics.Select(x => new TopicSpecification
            {
                Name = x
            }));
        }
        catch (CreateTopicsException ex) when (ex.Message.Contains("already exists"))
        {
            _logger.LogError(ex, "【事件总线】，kafka topic已经存在，不用重复创建");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "【事件总线】，kafka创建topic异常");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 删除topic
    /// </summary>
    /// <param name="topicNames"></param>
    /// <returns></returns>
    public async Task<bool> DeleteTopics(List<string> topics)
    {
        try
        {
            var config = new AdminClientConfig() { BootstrapServers = _kafkaOptions.Value.BootstrapServers };

            using var adminClient = new AdminClientBuilder(config).Build();

            await adminClient.DeleteTopicsAsync(topics);
        }
        catch (DeleteTopicsException ex) when (ex.Message.Contains("already exists"))
        {
            _logger.LogError(ex, "【事件总线】，kafka topic已经存在，不用重复创建");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "【事件总线】，kafka删除topic异常");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="topics"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> Subscribe(IEnumerable<string> topics)
    {
        try
        {
            if (topics == null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            await Connect();

            _consumerClient!.Subscribe(topics);

            var subscription = _consumerClient.Subscription;
            _logger.LogDebug("【事件总线】，首次加载订阅的topic列表：" + string.Join(",", subscription));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "【事件总线】，kafka订阅消息失败");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    /// <returns></returns>
    public async Task<ConsumeResult<Ignore, string>> Consume()
    {
        var consumerResult = _consumerClient!.Consume(CancellationToken.None); // timeout
        await Task.CompletedTask;
        return consumerResult;
    }

    /// <summary>
    /// 提交消息
    /// </summary>
    /// <returns></returns>
    public async Task<List<TopicPartitionOffset>> Commit()
    {
        await Task.CompletedTask;
        var result = _consumerClient!.Commit();
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public async Task Reject(object? sender)
    {
        await Task.CompletedTask;
        _consumerClient!.Assign(_consumerClient.Assignment);
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <returns></returns>
    public async Task Connect()
    {
        if (_consumerClient != null)
        {
            return;
        }

        ConnectionLock.Wait();

        try
        {
            if (_consumerClient == null)
            {
                var config = new ConsumerConfig(new Dictionary<string, string>());
                config.BootstrapServers ??= _kafkaOptions.Value.BootstrapServers;
                config.GroupId ??= _kafkaOptions.Value.GroupId;
                config.AutoOffsetReset ??= AutoOffsetReset.Earliest;
                config.AllowAutoCreateTopics ??= true;
                config.EnableAutoCommit ??= false;
                config.LogConnectionClose ??= false;

                _consumerClient = await BuildConsumer(config);
            }
        }
        finally
        {
            ConnectionLock.Release();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    protected async Task<IConsumer<Ignore, string>> BuildConsumer(ConsumerConfig config)
    {
        await Task.CompletedTask;
        return new ConsumerBuilder<Ignore, string>(config)
            .SetErrorHandler((IConsumer<Ignore, string> consumer, Error e) => {
                _logger.LogError($"An error occurred during connect kafka --> {e.Reason}");
            })
            .Build();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        _consumerClient?.Dispose();
    }
}