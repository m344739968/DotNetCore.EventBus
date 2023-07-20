using System.Net;
using DotNetCore.EventBus.Infrastructure.Http;
using DotNetCore.EventBus.Infrastructure.IdGenerate;
using DotNetCore.EventBus.Infrastructure.Json;
using DotNetCore.EventBus.Infrastructure.Models.Constant;
using DotNetCore.EventBus.Infrastructure.Models.Dto;
using DotNetCore.EventBus.Infrastructure.Models.Enums;
using DotNetCore.EventBus.Infrastructure.Models.EventBus;
using DotNetCore.EventBus.Infrastructure.Models.Options;
using Microsoft.Extensions.Options;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace DotNetCore.EventBus.Host.Processor;

/// <summary>
/// 默认处理器类
/// </summary>
public class DefaultProcessor : IProcessor
{
    private readonly ILogger<DefaultProcessor> _logger;
    private readonly IOptions<EventbusOptions> _kafkaOptions;
    private readonly IOptions<MySqlOptions> _mySqlOptions;
    private readonly FlurlHttpClient _flurlHttpClient;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="kafkaOptions"></param>
    /// <param name="baseItemQuery"></param>
    /// <param name="esClientFactory"></param>
    public DefaultProcessor(ILogger<DefaultProcessor> logger,
        IOptions<EventbusOptions> kafkaOptions,
        IOptions<MySqlOptions> mySqlOptions,
        FlurlHttpClient flurlHttpClient)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions;
        _mySqlOptions = mySqlOptions;
        _flurlHttpClient = flurlHttpClient;
    }

    /// <summary>
    /// 处理方法
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<bool> ProcessAsync(string topic, string value)
    {
        var eventId = string.Empty;
        try
        {
            _logger.LogDebug("【事件总线】，开始处理handle api，事件名称：{@topic}，入参：{@value}", topic, value);
            if (string.IsNullOrEmpty(value))
            {
                _logger.LogDebug("【事件总线】，事件名称：{@topic}，消息内容为空", topic);
                return false;
            }
            var obj = value.ToObj<EventBusPublishRequest>();
            // 订阅者列表事件执行
            // 1. 得到应用api调用token
            // 2. 根据token调用api执行回调
            eventId = obj.EventId;
            var eventName = topic.Replace($"{_kafkaOptions.Value.TopicPrefix}_", "");

            // 获取订阅者列表
            var subscribeEventList = await this.GetSubscribeEventList(eventName);
            var eventSubscribeMessageRecords = await this.GetEventSubscribeMessageRecord(eventId);

            _logger.LogInformation("【事件总线】，事件名称：{@topic}，订阅列表总数：{@count}", topic, subscribeEventList.Count);
            foreach (var subscribeEvent in subscribeEventList)
            {
                var isExistRecord = eventSubscribeMessageRecords?
                    .Where(x => x.EventId == eventId && x.SubscribeId == subscribeEvent.Id && x.Status == (int)EventStatusEnums.Successed)
                    .Any();
                if (isExistRecord == true)
                {
                    _logger.LogInformation("【事件总线】，事件名称：{@topic}，订阅clientId：{@clientId}，事件名称：{@eventName}，已经执行调用，请勿重复执行", topic, subscribeEvent.ClientId, eventName);
                    continue;
                }
                var clientId = subscribeEvent.ClientId;
                var clientSecret = subscribeEvent.ClientSecret;
                var tokeUrl = subscribeEvent.TokenUrl;
                var apiUrl = subscribeEvent.ApiUrl;
                var isValidToken = subscribeEvent.IsValidToken ?? false;
                var model = new EventSubscribeMessageRecord()
                {
                    Id = IdGenerateExtension.NewSequentialGuid(),
                    SubscribeId = subscribeEvent.Id,
                    SubscribeClientId = subscribeEvent.ClientId,
                    EventId = eventId,
                    EventName = eventName,
                    Status = (int)EventStatusEnums.Successed,
                    RequestUrl = apiUrl,
                    RequestContent = value,
                    CreatedTime = DateTime.Now,
                };
                _logger.LogInformation("【事件总线】，client：{@clientId}，调用执行api：{@apiUrl}", clientId, apiUrl);
                try
                {
                    var token = await this.GetToken(subscribeEvent);
                    var result = await _flurlHttpClient.PostJson(apiUrl, value, isValidToken, token);
                    model.ResponseStatus = result!.StatusCode;
                    model.ResponseContent = await result!.GetStringAsync();
                    if (result?.StatusCode == (int)HttpStatusCode.OK)
                    {
                        _logger.LogDebug("【事件总线】，订阅者，{@clientId}，执行回调【{@apiUrl}】成功", clientId, apiUrl);
                    }
                    else
                    {
                        model.Status = (int)EventStatusEnums.Failed;
                        _logger.LogInformation("【事件总线】，订阅者，{@clientId}，执行回调【{@apiUrl}】失败", clientId, apiUrl);
                    }
                }
                catch (FlurlHttpTimeoutException ex)
                {
                    // handle timeout
                    _logger.LogError(ex, "【事件总线】，订阅者，{@clientId}，执行回调【{@apiUrl}】超时异常", clientId, apiUrl);
                    model.ResponseStatus = ex.StatusCode;
                    model.Status = (int)EventStatusEnums.Failed;
                    model.Remark = "http超时异常，" + ex.StackTrace;
                }
                catch (FlurlHttpException ex)
                {
                    // handle error response
                    _logger.LogError(ex, "【事件总线】，订阅者，{@clientId}，执行回调【{@apiUrl}】http异常", clientId, apiUrl);
                    model.ResponseStatus = ex.StatusCode;
                    model.Status = (int)EventStatusEnums.Failed;
                    model.Remark = "http异常，" + ex.StackTrace;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "【事件总线】，订阅者，{@clientId}，执行回调【{@apiUrl}】异常", clientId, apiUrl);
                    model.ResponseStatus = 500;
                    model.Status = (int)EventStatusEnums.Failed;
                    model.Remark = "异常，" + ex.StackTrace;
                }
                finally
                {
                    await this.CreateEventSubscribeMessageRecord(model);
                }
            }

            // 修改发布消息状态
            eventSubscribeMessageRecords = await this.GetEventSubscribeMessageRecord(eventId);
            var recordSuccessCount = eventSubscribeMessageRecords.Count(x => x.Status == (int)EventStatusEnums.Successed);
            var isSuccess = subscribeEventList.Count <= recordSuccessCount; // 订阅者执行成功的次数大于等于总的订阅数量
            await this.UpdateEventPublishMessageRecord(eventId, isSuccess);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "【事件总线】，事件Id：{@eventId}，事件名称：{@topic}，处理发生异常", eventId, topic);
            return false;
        }
    }

    /// <summary>
    /// 获取token
    /// </summary>
    /// <param name="subscribe"></param>
    /// <returns></returns>
    public async Task<string> GetToken(SubscribeEventList subscribe)
    {
        var token = string.Empty;
        if (subscribe.IsValidToken == true)
        {
            TimeSpan? expiry = null;
            if (subscribe.TokenCacheDuration.HasValue)
            {
                expiry = TimeSpan.FromSeconds((double)subscribe.TokenCacheDuration!.Value);
            }
            if (subscribe.ClientType == (int)ClientTypeEnums.Internal)
            {
                // 暂未开放
                return String.Empty;
            }
            else
            {
                token = await _flurlHttpClient.GetToken(subscribe.TokenUrl, subscribe.ClientId, subscribe.ClientSecret, expiry);
            }
        }
        return token;
    }

    /// <summary>
    /// 通过事件名称获取订阅该事件的列表
    /// </summary>
    /// <param name="eventName"></param>
    /// <returns></returns>
    public async Task<List<SubscribeEventList>> GetSubscribeEventList(string eventName)
    {
        var cacheKey = $"{CacheKeyConstant.Prefix}:EventBus:{eventName}";
        var result = await RedisHelper.GetAsync<List<SubscribeEventList>>(cacheKey);
        if (result != null && result.Any())
        {
            return result;
        }
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        result = (await conn.QueryAsync<SubscribeEventList>("select * from event_subscribe_list")).ToList();
        await RedisHelper.SetAsync(cacheKey, result, TimeSpan.FromDays(1));
        return result;
    }

    /// <summary>
    /// 获取订阅者执行事件记录
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public async Task<List<EventSubscribeMessageRecord>> GetEventSubscribeMessageRecord(string eventId)
    {
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        var result = (await conn.QueryAsync<EventSubscribeMessageRecord>("select * from event_subscribe_message_record")).ToList();
        return result;
    }

    /// <summary>
    /// 新增订阅者执行事件记录
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public async Task<bool> CreateEventSubscribeMessageRecord(EventSubscribeMessageRecord model)
    {
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        var sql = "insert into event_subscribe_message_record(Name,Remark) values(@Name,@Remark)";
        var result = await conn.ExecuteAsync(sql, model);
        return result > 0;
    }

    /// <summary>
    /// 修改订阅者执行记录状态
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public async Task<bool> UpdateEventPublishMessageRecord(string eventId, bool isSuccess)
    {
        var status = isSuccess ? EventStatusEnums.Successed : EventStatusEnums.Failed;
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        var sql = "update event_publish_message_record set try_count=try_count+1, status=@status where event_id=@eventId";
        var result = await conn.ExecuteAsync(sql, new { status = (int)status, eventId = eventId });
        return result > 0;
    }

    /// <summary>
    /// 修改订阅者执行次数
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public async Task<bool> UpdateEventPublishMessageRecordTryCount(string eventId)
    {
        using var conn = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
        var sql = "update event_publish_message_record set try_count=try_count+1 where event_id=@eventId";
        var result = await conn.ExecuteAsync(sql, new { eventId = eventId });
        return result > 0;
    }
}
