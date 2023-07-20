using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class GetEventSubscribeMessageRecordRequest : BasePaging
{
    /// <summary>
    /// 订阅应用id
    /// </summary>
    public string SubscribeClientId { get; set; }

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
