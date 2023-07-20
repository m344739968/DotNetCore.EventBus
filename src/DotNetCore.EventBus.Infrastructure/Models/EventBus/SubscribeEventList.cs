using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCore.EventBus.Infrastructure.Models.EventBus
{
    ///<summary>
    /// 订阅事件列表
    ///</summary>
    [Table("event_subscribe_list")]
    public class SubscribeEventList
    {
        /// <summary>
        /// Desc:主键编号
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("id")]
        public string Id { get; set; }

        /// <summary>
        /// Desc:事件名称
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("event_name")]
        public string EventName { get; set; }

        /// <summary>
        /// Desc: 应用类别，0内部应用，1外部应用
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("client_type")]
        public int? ClientType { get; set; } = 1;

        /// <summary>
        /// Desc:事件订阅应用id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// Desc:事件订阅应用密钥
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("client_secret")]
        public string ClientSecret { get; set; }

        /// <summary>
        /// Desc:事件订阅应用获取token地址
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("token_url")]
        public string TokenUrl { get; set; }

        /// <summary>
        /// Desc:事件订阅应用api地址
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("api_url")]
        public string ApiUrl { get; set; }

        /// <summary>
        /// Desc: token缓存时间单位：秒
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("token_cache_duration")]
        public decimal? TokenCacheDuration { get; set; }

        /// <summary>
        /// Desc: 是否验证token
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("is_valid_token")]
        public bool? IsValidToken { get; set; }

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

        /// <summary>
        /// Desc:修改人id
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("modified_by")]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Desc:修改人名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("modified_name")]
        public string ModifiedName { get; set; }

        /// <summary>
        /// Desc:修改时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("modified_time")]
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// Desc:扩展字段
        /// Default:
        /// Nullable:True
        /// </summary>           
        [Column("extend")]
        public string Extend { get; set; }
    }
}
