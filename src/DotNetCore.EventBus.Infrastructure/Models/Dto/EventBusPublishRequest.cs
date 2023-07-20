using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class EventBusPublishRequest
{
    /// <summary>
    /// 发送事件应用id
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 事件id，
    /// 此参数可以用于做幂等处理，不传递会默认guid
    /// </summary>
    public string EventId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// 事件数据
    /// 建议格式为：json字符串格式
    /// </summary>
    public string EventData { get; set; }
}
