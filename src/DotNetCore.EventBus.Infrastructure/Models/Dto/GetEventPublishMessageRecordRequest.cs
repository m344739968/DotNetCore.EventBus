using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class GetEventPublishMessageRecordRequest : BasePaging
{
    /// <summary>
    /// 应用id
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 事件id
    /// </summary>
    public string EventId { get; set; }

    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public int? Status { get; set; }
}
