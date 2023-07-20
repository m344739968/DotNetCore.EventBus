using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class EventPublishMessageRecordResponse
{
    /// <summary>
    /// Desc:主键编号
    /// Default:
    /// Nullable:False
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Desc: 发布事件消息应用id
    /// Default:
    /// Nullable:False
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Desc: 事件id
    /// Default:
    /// Nullable:False
    /// </summary>
    public string EventId { get; set; }

    /// <summary>
    /// Desc:事件名称
    /// Default:
    /// Nullable:False
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// Desc: 事件消息业务json数据
    /// Default:
    /// Nullable:False
    /// </summary>   
    public string EventData { get; set; }

    /// <summary>
    /// Desc: 消息状态 0未处理，1处理成功，2处理失败
    /// Default:
    /// Nullable:False
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 重试次数
    /// 默认3次
    /// </summary>
    public int? TryCount { get; set; } = 0;

    /// <summary>
    /// Desc: 备注
    /// Default:
    /// Nullable:False
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// Desc:创建人id
    /// Default:
    /// Nullable:True
    /// </summary>    
    public string CreatedBy { get; set; }

    /// <summary>
    /// Desc:创建人名称
    /// Default:
    /// Nullable:True
    /// </summary>
    public string CreatedName { get; set; }

    /// <summary>
    /// Desc:创建时间
    /// Default:
    /// Nullable:True
    /// </summary>
    public DateTime? CreatedTime { get; set; }
}