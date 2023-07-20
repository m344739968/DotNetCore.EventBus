using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCore.EventBus.Infrastructure.Models.EventBus
{
    ///<summary>
    /// 事件消息订阅者执行记录表
    ///</summary>
    [Table("event_subscribe_message_record")]
    public class EventSubscribeMessageRecord
    {
        /// <summary>
        /// Desc:主键编号
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// Desc: 事件消息id
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
        /// Desc: 事件订阅者id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("subscribe_id")]
        public string SubscribeId { get; set; }

        /// <summary>
        /// 订阅者应用id
        /// </summary>
        [Column("subscribe_client_id")]
        public string SubscribeClientId { get; set; }

        /// <summary>
        /// Desc: 消息状态
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 请求url
        /// </summary>
        [Column("request_url")]
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求内容
        /// </summary>
        [Column("request_content")]
        public string RequestContent { get; set; }

        /// <summary>
        /// 响应状态
        /// </summary>
        [Column("response_status")]
        public int? ResponseStatus { get; set; }

        /// <summary>
        /// 响应内容
        /// </summary>
        [Column("response_content")]
        public string ResponseContent { get; set; }

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
