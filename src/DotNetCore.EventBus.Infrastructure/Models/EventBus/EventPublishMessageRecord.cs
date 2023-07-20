using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCore.EventBus.Infrastructure.Models.EventBus
{
    ///<summary>
    /// 事件发布消息记录
    ///</summary>
    [Table("event_publish_message_record")]
    public class EventPublishMessageRecord
    {
        /// <summary>
        /// 主键编号
        /// </summary>           
        [Column("Id")]
        public string Id { get; set; }

        /// <summary>
        /// 发布事件消息应用id
        /// </summary>           
        [Column("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Desc: 事件id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("event_id")]
        public string EventId { get; set; }

        /// <summary>
        /// Desc:事件名称
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("event_name")]
        public string EventName { get; set; }

        /// <summary>
        /// Desc: 事件消息业务json数据
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("event_data")]
        public string EventData { get; set; }

        /// <summary>
        /// Desc: 消息状态
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 重试次数
        /// 默认3次
        /// </summary>
        [Column("try_count")]
        public int? TryCount { get; set; } = 0;

        /// <summary>
        /// Desc: 备注
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:创建人id
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("created_by")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Desc:创建人名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("created_name")]
        public string CreatedName { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("created_time")]
        public DateTime? CreatedTime { get; set; }
    }
}
