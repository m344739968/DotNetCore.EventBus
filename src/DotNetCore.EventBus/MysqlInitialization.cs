using DotNetCore.EventBus.Infrastructure.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using Dapper;
using DotNetCore.EventBus.Infrastructure.Kafka;

namespace DotNetCore.EventBus
{
    /// <summary>
    /// mysql init 
    /// </summary>
    public class MysqlInitialization
    {
        private readonly ILogger<MysqlInitialization> _logger;
        private readonly IOptions<MySqlOptions> _mySqlOptions;
        private readonly IOptions<EventbusOptions> _eventbusOptions;
        private readonly KafkaConsumerClient _kafkaConsumerClient;

        /// <summary>
        /// mysql init
        /// </summary>
        /// <param name="mySqlOptions"></param>
        public MysqlInitialization(ILogger<MysqlInitialization> logger, 
            IOptions<MySqlOptions> mySqlOptions,
            IOptions<EventbusOptions> eventbusOptions, 
            KafkaConsumerClient kafkaConsumerClient) 
        {
            _logger = logger;
            _mySqlOptions = mySqlOptions;
            _eventbusOptions = eventbusOptions;
            _kafkaConsumerClient = kafkaConsumerClient;
        }

        public async Task InitializeAsync()
        {
            var sql = CreateDbTablesScript();
            var connection = new MySqlConnection(_mySqlOptions.Value.ConnectionString);
            var result = await connection.ExecuteAsync(sql);
            _logger.LogDebug("初始化数据表完成，初始化结果：" + result);


            // 初始化topic
            var topicName = $"{_eventbusOptions.Value.TopicPrefix}_test";
            var res = await _kafkaConsumerClient.CreateTopics(new List<string>() { topicName });
            _logger.LogDebug($"初始化消息topic：【{topicName}】，结果：" + res);
        }

        protected string CreateDbTablesScript()
        {
            var batchSql =
                $@"
CREATE TABLE IF NOT EXISTS `{_eventbusOptions.Value.TopicPrefix}_register_list` (
  `Id` varchar(50) NOT NULL COMMENT '主键id',
  `EventName` varchar(50) DEFAULT NULL COMMENT '事件名称（唯一）',
  `EventParams` varchar(200) DEFAULT NULL COMMENT '事件参数',
  `CreatedBy` varchar(20) DEFAULT NULL COMMENT '创建人id',
  `CreatedName` varchar(20) DEFAULT NULL COMMENT '创建人名称',
  `CreatedTime` datetime DEFAULT NULL COMMENT '创建时间',
  `ModifiedBy` varchar(20) DEFAULT NULL COMMENT '修改人id',
  `ModifiedName` varchar(20) DEFAULT NULL COMMENT '修改人名称',
  `ModifiedTime` datetime DEFAULT NULL COMMENT '修改时间',
  `Remark` varchar(50) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`),
  UNIQUE KEY `idx_event_name` (`EventName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='注册事件列表';

CREATE TABLE IF NOT EXISTS `{_eventbusOptions.Value.TopicPrefix}_subscribe_list` (
  `Id` varchar(50) NOT NULL COMMENT '主键id',
  `EventName` varchar(50) DEFAULT NULL COMMENT '事件名称',
  `ClientType` int DEFAULT NULL COMMENT '应用类型，0内部应用，1外部应用',
  `ClientId` varchar(50) DEFAULT NULL COMMENT '订阅应用id',
  `ClientSecret` varchar(50) DEFAULT NULL COMMENT '订阅应用密钥',
  `TokenUrl` varchar(100) DEFAULT NULL COMMENT '订阅者回调api获取token地址',
  `ApiUrl` varchar(100) DEFAULT NULL COMMENT '订阅者回调api地址',
  `TokenCacheDuration` decimal(18,4) DEFAULT NULL COMMENT 'token缓存时间单位：秒',
  `IsValidToken` bit(1) DEFAULT NULL COMMENT '是否验证token',
  `CreatedBy` varchar(20) DEFAULT NULL COMMENT '创建人id',
  `CreatedName` varchar(20) DEFAULT NULL COMMENT '创建人名称',
  `CreatedTime` datetime DEFAULT NULL COMMENT '创建时间',
  `ModifiedBy` varchar(20) DEFAULT NULL COMMENT '修改人id',
  `ModifiedName` varchar(20) DEFAULT NULL COMMENT '修改人名称',
  `ModifiedTime` datetime DEFAULT NULL COMMENT '修改时间',
  `Remark` varchar(50) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `idx_event_name` (`ClientId`,`EventName`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='事件订阅列表';

CREATE TABLE IF NOT EXISTS `{_eventbusOptions.Value.TopicPrefix}_publish_message_record` (
  `Id` varchar(50) NOT NULL COMMENT '主键id',
  `EventId` varchar(50) DEFAULT NULL COMMENT '事件id',
  `EventName` varchar(50) DEFAULT NULL COMMENT '事件名称',
  `EventData` text COMMENT '事件业务数据',
  `Status` int DEFAULT '0' COMMENT '消息状态，0未处理，1已处理',
  `ClientId` varchar(50)  DEFAULT NULL COMMENT '应用id',
  `BusinessId` varchar(50)  DEFAULT NULL COMMENT '业务标识',
  `CreatedBy` varchar(20) DEFAULT NULL COMMENT '创建人id',
  `CreatedName` varchar(20) DEFAULT NULL COMMENT '创建人名称',
  `CreatedTime` datetime DEFAULT NULL COMMENT '创建时间',
  `Remark` varchar(50) DEFAULT NULL COMMENT '备注信息',
  `TryCount` int DEFAULT NULL COMMENT '重试次数，默认3',
  PRIMARY KEY (`id`),
  UNIQUE KEY `idx_event_id` (`EventId`) USING BTREE,
  KEY `idx_event_name` (`EventName`,`CreatedTime`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='事件发布消息记录表';

CREATE TABLE IF NOT EXISTS `{_eventbusOptions.Value.TopicPrefix}_subscribe_message_record` (
  `Id` varchar(50) NOT NULL COMMENT '主键id',
  `EventId` varchar(50)  DEFAULT NULL COMMENT '事件消息id',
  `EventName` varchar(50) DEFAULT NULL COMMENT '事件消息名称',
  `SubscribeId` varchar(50)  DEFAULT NULL COMMENT '事件订阅者id',
  `SubscribeClientId` varchar(50)  DEFAULT NULL COMMENT '事件订阅者应用id',
  `Status` int DEFAULT '0' COMMENT '消息状态，0未处理，1已处理',
  `RequestUrl` varchar(500) DEFAULT NULL COMMENT '请求地址',
  `RequestContent` text  COMMENT '请求报文',
  `ResponseStatus` int DEFAULT NULL COMMENT '响应状态码',
  `ResponseContent` varchar(500)  DEFAULT NULL COMMENT '响应报文',
  `BusinessId` varchar(50) DEFAULT NULL COMMENT '业务标识id',
  `CreatedBy` varchar(20) DEFAULT NULL COMMENT '创建人id',
  `CreatedName` varchar(20) DEFAULT NULL COMMENT '创建人名称',
  `CreatedTime` datetime DEFAULT NULL COMMENT '创建时间',
  `Remark` text  COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `idx_event_id` (`EventId`,`EventName`,`CreatedTime`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='事件消息订阅者执行记录表';

";
            return batchSql;
        }
    }
}
