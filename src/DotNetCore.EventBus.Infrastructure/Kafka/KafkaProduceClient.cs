using Confluent.Kafka;
using DotNetCore.EventBus.Infrastructure.Json;
using DotNetCore.EventBus.Infrastructure.Models.Dto;
using DotNetCore.EventBus.Infrastructure.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetCore.EventBus.Infrastructure.Kafka;
/// <summary>
/// kafka生产者客户端
/// </summary>
public class KafkaProduceClient
{
    private readonly ILogger<KafkaProduceClient> _logger;
    private readonly IOptions<EventbusOptions> _kafkaOptions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    public KafkaProduceClient(ILogger<KafkaProduceClient> logger, IOptions<EventbusOptions> kafkaOptions)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions;
    }

    /// <summary>
    /// 生产消息
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<bool> Produce(string topic, EventBusPublishRequest request)
    {
        try
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaOptions.Value.BootstrapServers,
                QueueBufferingMaxMessages = 10,
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 3000
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            // byte[] value = Encoding.UTF8.GetBytes(request.ToJson());
            var value = request.ToJson();
            var dr = await producer.ProduceAsync(topic, new Message<Null, string> { Value = value });
            _logger.LogDebug($"【事件总线】，生产消息，Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogDebug($"【事件总线】，Delivery failed: {e.Error.Reason}");
            return false;
        }
        return true;
    }
}