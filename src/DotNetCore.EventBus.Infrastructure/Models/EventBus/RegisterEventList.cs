using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCore.EventBus.Infrastructure.Models.EventBus
{
    ///<summary>
    /// 注册事件列表
    ///</summary>
    [Table("event_register_list")]
    public class RegisterEventList
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
        /// Desc:事件参数
        /// Default:
        /// Nullable:False
        /// </summary>           
        [Column("event_params")]
        public string EventParams { get; set; }

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
