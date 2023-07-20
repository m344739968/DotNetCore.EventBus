//using DotNetCore.EventBus.Infrastructure.Filter;
//using DotNetCore.EventBus.Infrastructure.IdGenerate;
//using DotNetCore.EventBus.Infrastructure.Models.Dto;
//using DotNetCore.EventBus.Infrastructure.Models.EventBus;
//using DotNetCore.EventBus.Infrastructure.Models.Options;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;

//namespace Samples.DotNetCore.EventBus.Controllers
//{
//    /// <summary>
//    /// 事件服务
//    /// </summary>
//    public class EventBusController : Controller
//    {
//        private readonly ILogger<EventBusController> _logger;
//        private readonly IOptions<KafkaOptions> _kafkaOptions;
//        private readonly KafkaConsumerClient _kafkaConsumerClient;
//        private readonly KafkaProduceClient _kafkaProduceClient;
//        private readonly SqlSugarScopeProvider _db;
//        private readonly CsRedisClient _csRedis;

//        public EventBusController(ILogger<EventBusController> logger,
//            IOptions<KafkaOptions> kafkaOptions,
//            KafkaConsumerClient kafkaConsumerClient,
//            KafkaProduceClient kafkaProduceClient,
//            ISqlSugarClient db,
//            CsRedisClient csRedis)
//        {
//            _logger = logger;
//            _kafkaOptions = kafkaOptions;
//            _kafkaConsumerClient = kafkaConsumerClient;
//            _kafkaProduceClient = kafkaProduceClient;
//            _db = db.AsTenant().GetDb();
//            _csRedis = csRedis;
//        }

//        /// <summary>
//        /// 注册事件
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpPost("RegisterEventAsync")]
//        public async Task<IActionResult> RegisterEventAsync([FromBody] EventBusRegisterRequest request)
//        {
//            _logger.LogInformation("【事件总线】，注册事件，{@request}", request);
//            if (string.IsNullOrEmpty(request.EventName))
//            {
//                throw new BusOperationException("事件名称不能为空");
//            }
//            var isExists = await _db.Queryable<RegisterEventList>()
//                .Where(x => x.EventName == request.EventName)
//                .AnyAsync();
//            if (isExists)
//            {
//                throw new BusOperationException("请勿重复注册名称相同的事件");
//            }
//            // 订阅事件
//            var model = new RegisterEventList()
//            {
//                Id = IdGenerateExtension.NewSequentialGuid(),
//                EventName = request.EventName,
//                EventParams = request.EventParams,
//                Remark = request.Remark,
//                CreatedBy = request.Operator,
//                CreatedName = request.Operator,
//                CreatedTime = DateTime.Now,
//            };
//            var res = await _db.Insertable<RegisterEventList>(model).ExecuteCommandAsync();
//            if (res < 0)
//            {
//                throw new BusOperationException("注册事件失败");
//            }
//            var topicName = $"{_kafkaOptions.Value.TopicPrefix}_{request.EventName}";
//            var kafkaRes = await _kafkaConsumerClient.CreateTopics(new List<string>() { topicName });
//            if (!kafkaRes)
//            {
//                throw new BusOperationException("注册事件topic失败");
//            }
//            return View();
//        }

//        /// <summary>
//        /// 删除注册事件
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpPost("DeleteEventAsync")]
//        public async Task<ResultMessage<bool>> DeleteEventAsync([FromBody] EventBusRegisterRequest request)
//        {
//            _logger.LogInformation("【事件总线】，删除注册事件，{@request}", request);
//            var result = new ResultMessage<bool>();
//            var res = await _db.Deleteable<RegisterEventList>().Where(x => x.Id == request.Id).ExecuteCommandAsync();
//            var topicName = $"{_kafkaOptions.Value.TopicPrefix}_{request.EventName}";
//            await _kafkaConsumerClient.DeleteTopics(new List<string>() { topicName });
//            result.SetSuccess(true);
//            return result;
//        }

//        /// <summary>
//        /// 获取注册事件列表
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpGet("GetRegisterEventListAsync")]
//        public async Task<ResultMessage<Page<QueryRegisterEventListResponse>>> GetRegisterEventListAsync([FromQuery] GetRegisterEventListRequest request)
//        {
//            _logger.LogInformation("【事件总线】，获取注册事件列表，{@request}", request);
//            RefAsync<int> total = new();
//            var result = new ResultMessage<Page<QueryRegisterEventListResponse>>();
//            var data = await _db.Queryable<RegisterEventList>()
//                .WhereIF(!string.IsNullOrEmpty(request.EventName), x => x.EventName == request.EventName)
//                .Select(x => new QueryRegisterEventListResponse()
//                {
//                    Id = x.Id,
//                    EventName = x.EventName,
//                    EventParams = x.EventParams,
//                    Remark = x.Remark,
//                    CreatedBy = x.CreatedBy,
//                    CreatedName = x.CreatedName,
//                    CreatedTime = x.CreatedTime,
//                    ModifiedBy = x.ModifiedBy,
//                    ModifiedName = x.ModifiedName,
//                    ModifiedTime = x.ModifiedTime,
//                })
//                .OrderByDescending(x => x.CreatedTime)
//                .ToPageListAsync(request.PageIndex, request.PageSize, total);
//            var list = new Page<QueryRegisterEventListResponse>(request.PageIndex, request.PageSize, data, total);
//            result.SetSuccess(list);
//            return result;
//        }

//        /// <summary>
//        /// 订阅事件消息
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpPost("SubscribeAsync")]
//        public async Task<ResultMessage<bool>> SubscribeAsync([FromBody] EventBusSubscribeRequest request)
//        {
//            _logger.LogInformation("【事件总线】，订阅事件，{@request}", request);
//            var result = new ResultMessage<bool>();
//            if (string.IsNullOrEmpty(request.EventName))
//            {
//                throw new BusOperationException("事件名称不能为空");
//            }
//            if (string.IsNullOrEmpty(request.ClientId))
//            {
//                throw new BusOperationException("订阅者clienetId不能为空");
//            }
//            var isExists = await _db.Queryable<SubscribeEventList>()
//                .Where(x => x.ClientId == request.ClientId && x.EventName == request.EventName)
//                .WhereIF(!string.IsNullOrEmpty(request.Id), x => x.Id != request.Id)
//                .AnyAsync();
//            if (isExists)
//            {
//                throw new BusOperationException("请勿重复订阅");
//            }
//            if (string.IsNullOrEmpty(request.Id))
//            {
//                // 新增，订阅事件信息
//                var model = new SubscribeEventList()
//                {
//                    Id = IdGenerateExtension.NewSequentialGuid(),
//                    EventName = request.EventName,
//                    ClientType = request.ClientType,
//                    ClientId = request.ClientId,
//                    ClientSecret = request.ClientSecret,
//                    ApiUrl = request.ApiUrl.Trim(),
//                    TokenUrl = request.TokenUrl.Trim(),
//                    IsValidToken = request.IsValidToken,
//                    TokenCacheDuration = request.TokenCacheDuration,
//                    Remark = request.Remark,
//                    CreatedBy = request.Operator,
//                    CreatedName = request.Operator,
//                    CreatedTime = DateTime.Now,
//                };
//                var res = await _db.Insertable<SubscribeEventList>(model).ExecuteCommandAsync();
//                if (res < 0)
//                {
//                    throw new BusOperationException("新增订阅失败");
//                }
//            }
//            else
//            {
//                var res = await _db.Updateable<SubscribeEventList>()
//                     .SetColumns(x => x.EventName == request.EventName)
//                     .SetColumns(x => x.ClientId == request.ClientId)
//                     .SetColumns(x => x.ClientSecret == request.ClientSecret)
//                     .SetColumns(x => x.ClientType == request.ClientType)
//                     .SetColumns(x => x.ApiUrl == request.ApiUrl.Trim())
//                     .SetColumns(x => x.TokenUrl == request.TokenUrl.Trim())
//                     .SetColumns(x => x.IsValidToken == request.IsValidToken)
//                     .SetColumns(x => x.TokenCacheDuration == request.TokenCacheDuration)
//                     .SetColumns(x => x.Remark == request.Remark)
//                     .SetColumns(x => x.ModifiedBy == request.Operator)
//                     .SetColumns(x => x.ModifiedName == request.Operator)
//                     .SetColumns(x => x.ModifiedTime == DateTime.Now)
//                     .Where(x => x.Id == request.Id)
//                     .ExecuteCommandAsync();
//                if (res < 0)
//                {
//                    throw new BusOperationException("修改订阅失败");
//                }
//            }
//            var cacheKey = $"{CacheKeyConstant.Prefix}:EventBus:SubscribeEventList_{request.EventName}";
//            await _csRedis.DelAsync(cacheKey);
//            result.SetSuccess(true);
//            return result;
//        }

//        /// <summary>
//        /// 取消订阅消息
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpPost("CancelSubscribeAsync")]
//        public async Task<ResultMessage<bool>> CancelSubscribeAsync([FromBody] EventBusSubscribeRequest request)
//        {
//            _logger.LogInformation("【事件总线】，取消订阅事件，{@request}", request);
//            var result = new ResultMessage<bool>();
//            var res = await _db.Deleteable<SubscribeEventList>()
//                .Where(x => x.Id == request.Id)
//                .ExecuteCommandAsync();
//            if (res < 0)
//            {
//                throw new BusOperationException("取消订阅失败");
//            }
//            result.SetSuccess(true);
//            return result;
//        }

//        /// <summary>
//        /// 获取订阅事件列表
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpGet("GetSubscribeListAsync")]
//        public async Task<ResultMessage<Page<QuerySubscribeEventListResponse>>> GetSubscribeListAsync([FromQuery] GetSubscribeListRequest request)
//        {
//            _logger.LogInformation("【事件总线】，获取订阅事件列表，{@request}", request);
//            RefAsync<int> total = new();
//            var result = new ResultMessage<Page<QuerySubscribeEventListResponse>>();
//            var data = await _db.Queryable<SubscribeEventList>()
//                .WhereIF(!string.IsNullOrEmpty(request.EventName), x => x.EventName == request.EventName)
//                 .WhereIF(!string.IsNullOrEmpty(request.ClientId), x => x.ClientId == request.ClientId)
//                .Select(x => new QuerySubscribeEventListResponse()
//                {
//                    Id = x.Id,
//                    EventName = x.EventName,
//                    ClientId = x.ClientId,
//                    ClientSecret = x.ClientSecret,
//                    ClientType = x.ClientType,
//                    TokenUrl = x.TokenUrl,
//                    ApiUrl = x.ApiUrl,
//                    IsValidToken = x.IsValidToken,
//                    TokenCacheDuration = x.TokenCacheDuration,
//                    Remark = x.Remark,
//                    CreatedBy = x.CreatedBy,
//                    CreatedName = x.CreatedName,
//                    CreatedTime = x.CreatedTime,
//                    ModifiedBy = x.ModifiedBy,
//                    ModifiedName = x.ModifiedName,
//                    ModifiedTime = x.ModifiedTime,
//                })
//                .OrderByDescending(x => x.CreatedTime)
//                .ToPageListAsync(request.PageIndex, request.PageSize, total);
//            var list = new Page<QuerySubscribeEventListResponse>(request.PageIndex, request.PageSize, data, total);
//            result.SetSuccess(list);
//            return result;
//        }

//        /// <summary>
//        /// 发布事件消息
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        [HttpPost("PublishAsync")]
//        public async Task<ResultMessage<bool>> PublishAsync([FromBody] EventBusPublishRequest request)
//        {
//            _logger.LogInformation("【事件总线】，发送事件消息，{@request}", request);
//            var result = new ResultMessage<bool>();
//            if (string.IsNullOrEmpty(request.ClientId))
//            {
//                throw new BusOperationException("应用id不能为空");
//            }
//            if (string.IsNullOrEmpty(request.EventName))
//            {
//                throw new BusOperationException("发布消息名称不能为空");
//            }
//            if (string.IsNullOrEmpty(request.EventData))
//            {
//                throw new BusOperationException("发布消息内容不能为空");
//            }
//            var isExists = await _db.Queryable<RegisterEventList>()
//               .Where(x => x.EventName == request.EventName)
//               .AnyAsync();
//            if (!isExists)
//            {
//                throw new BusOperationException("未注册的事件名称，发布消息失败");
//            }
//            var record = new EventPublishMessageRecord()
//            {
//                Id = IdGenerateExtension.NewSequentialGuid(),
//                ClientId = request.ClientId,
//                EventId = request.EventId,
//                EventName = request.EventName,
//                EventData = request.EventData,
//                Status = (int)EventStatusEnums.Unprocessed,
//                CreatedTime = DateTime.Now,
//            };
//            var epmrRes = await _db.Insertable<EventPublishMessageRecord>(record).ExecuteCommandAsync();
//            if (epmrRes <= 0)
//            {
//                throw new BusOperationException("发布消息失败，插入数据库失败");
//            }
//            var topicName = $"{_kafkaOptions.Value.TopicPrefix}_{request.EventName}";
//            var kafkaResult = await _kafkaProduceClient.Produce(topicName, request);
//            if (!kafkaResult)
//            {
//                throw new BusOperationException("发布消息失败");
//            }
//            result.SetSuccess(true);
//            return result;
//        }

//        /// <summary>
//        /// 获取消息列表
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("GetEventPublishMessageRecordAsync")]
//        public async Task<ResultMessage<Page<EventPublishMessageRecordResponse>>> GetEventPublishMessageRecordAsync([FromQuery] GetEventPublishMessageRecordRequest request)
//        {
//            _logger.LogInformation("【事件总线】，获取消息列表，{@request}", request);
//            RefAsync<int> total = new();
//            var result = new ResultMessage<Page<EventPublishMessageRecordResponse>>();
//            var data = await _db.Queryable<EventPublishMessageRecord>()
//                .WhereIF(!string.IsNullOrEmpty(request.EventId), x => x.EventId == request.EventId)
//                .WhereIF(!string.IsNullOrEmpty(request.EventName), x => x.EventName == request.EventName)
//                .WhereIF(!string.IsNullOrEmpty(request.ClientId), x => x.ClientId == request.ClientId)
//                .WhereIF(request.Status.HasValue, x => x.Status == request.Status)
//                .Select(x => new EventPublishMessageRecordResponse()
//                {
//                    Id = x.Id,
//                    EventName = x.EventName,
//                    ClientId = x.ClientId,
//                    // EventData = x.EventData,
//                    EventId = x.EventId,
//                    Status = x.Status,
//                    TryCount = x.TryCount,
//                    // Remark = x.Remark,
//                    CreatedBy = x.CreatedBy,
//                    CreatedName = x.CreatedName,
//                    CreatedTime = x.CreatedTime,
//                })
//                .OrderByDescending(x => x.CreatedTime)
//                .ToPageListAsync(request.PageIndex, request.PageSize, total);
//            var list = new Page<EventPublishMessageRecordResponse>(request.PageIndex, request.PageSize, data, total);
//            result.SetSuccess(list);
//            return result;
//        }

//        /// <summary>
//        /// 获取消息详情
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("GetEventPublishMessageRecordDetailAsync")]
//        public async Task<ResultMessage<EventPublishMessageRecordResponse>> GetEventPublishMessageRecordDetailAsync([FromQuery] string id)
//        {
//            var result = new ResultMessage<EventPublishMessageRecordResponse>();
//            var data = await _db.Queryable<EventPublishMessageRecord>()
//                .Where(x => x.Id == id)
//                .Select(x => new EventPublishMessageRecordResponse()
//                {
//                    Id = x.Id,
//                    EventName = x.EventName,
//                    ClientId = x.ClientId,
//                    EventData = x.EventData,
//                    EventId = x.EventId,
//                    Status = x.Status,
//                    TryCount = x.TryCount,
//                    Remark = x.Remark,
//                    CreatedBy = x.CreatedBy,
//                    CreatedName = x.CreatedName,
//                    CreatedTime = x.CreatedTime,
//                }).FirstAsync();
//            result.SetSuccess(data);
//            return result;
//        }

//        /// <summary>
//        /// 获取订阅者执行记录列表
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("GetEventSubscribeMessageRecordAsync")]
//        public async Task<ResultMessage<Page<EventSubscribeMessageRecordResponse>>> GetEventSubscribeMessageRecordAsync([FromQuery] GetEventSubscribeMessageRecordRequest request)
//        {
//            _logger.LogInformation("【事件总线】，获取订阅者执行记录列表，{@request}", request);
//            RefAsync<int> total = new();
//            var result = new ResultMessage<Page<EventSubscribeMessageRecordResponse>>();
//            var data = await _db.Queryable<EventSubscribeMessageRecord>()
//                .WhereIF(!string.IsNullOrEmpty(request.EventId), x => x.EventId == request.EventId)
//                .WhereIF(!string.IsNullOrEmpty(request.EventName), x => x.EventName == request.EventName)
//                .WhereIF(!string.IsNullOrEmpty(request.SubscribeClientId), x => x.SubscribeClientId == request.SubscribeClientId)
//                .WhereIF(request.Status.HasValue, x => x.Status == request.Status)
//                .Select(x => new EventSubscribeMessageRecordResponse()
//                {
//                    Id = x.Id,
//                    SubscribeId = x.SubscribeId,
//                    SubscribeClientId = x.SubscribeClientId,
//                    EventId = x.EventId,
//                    EventName = x.EventName,
//                    Status = x.Status,
//                    // Remark = x.Remark,
//                    RequestUrl = x.RequestUrl,
//                    // RequestContent = x.RequestContent,
//                    // ResponseContent = x.ResponseContent,
//                    ResponseStatus = x.ResponseStatus,
//                    CreatedBy = x.CreatedBy,
//                    CreatedName = x.CreatedName,
//                    CreatedTime = x.CreatedTime,
//                })
//                .OrderByDescending(x => x.CreatedTime)
//                .ToPageListAsync(request.PageIndex, request.PageSize, total);
//            var list = new Page<EventSubscribeMessageRecordResponse>(request.PageIndex, request.PageSize, data, total);
//            result.SetSuccess(list);
//            return result;
//        }

//        /// <summary>
//        /// 获取订阅者执行记录详情
//        /// </summary>
//        /// <returns></returns>
//        [HttpGet("GetEventSubscribeMessageRecordDetailAsync")]
//        public async Task<ResultMessage<EventSubscribeMessageRecordResponse>> GetEventSubscribeMessageRecordDetailAsync([FromQuery] string id)
//        {
//            _logger.LogInformation("【事件总线】，获取订阅者执行记录详情，{@id}", id);
//            var result = new ResultMessage<EventSubscribeMessageRecordResponse>();
//            var data = await _db.Queryable<EventSubscribeMessageRecord>()
//                .Where(x => x.Id == id)
//                .Select(x => new EventSubscribeMessageRecordResponse()
//                {
//                    Id = x.Id,
//                    SubscribeId = x.SubscribeId,
//                    SubscribeClientId = x.SubscribeClientId,
//                    EventId = x.EventId,
//                    EventName = x.EventName,
//                    Status = x.Status,
//                    Remark = x.Remark,
//                    RequestUrl = x.RequestUrl,
//                    RequestContent = x.RequestContent,
//                    ResponseContent = x.ResponseContent,
//                    ResponseStatus = x.ResponseStatus,
//                    CreatedBy = x.CreatedBy,
//                    CreatedName = x.CreatedName,
//                    CreatedTime = x.CreatedTime,
//                }).FirstAsync();
//            result.SetSuccess(data);
//            return result;
//        }
//    }
//}
