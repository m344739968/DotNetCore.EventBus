using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class EventSubscribeMessageRecordResponse
{
    /// <summary>
    /// Desc:主键编号
    /// Default:
    /// Nullable:False
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Desc: 事件消息id
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
    /// Desc: 事件订阅者主键id
    /// Default:
    /// Nullable:False
    /// </summary>
    public string SubscribeId { get; set; }

    /// <summary>
    /// 订阅者应用id
    /// </summary>
    public string SubscribeClientId { get; set; }

    /// <summary>
    /// Desc: 消息状态
    /// Default:
    /// Nullable:False
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    public string RequestUrl { get; set; }

    /// <summary>
    /// 请求内容
    /// </summary>
    public string RequestContent { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public int? ResponseStatus { get; set; }

    /// <summary>
    /// 响应内容
    /// </summary>
    public string ResponseContent { get; set; }

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