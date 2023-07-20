using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class EventBusRegisterRequest
{
    /// <summary>
    /// 主键id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }

    /// <summary>
    /// 事件参数
    /// </summary>
    public string EventParams { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public string Operator { get; set; }
}
