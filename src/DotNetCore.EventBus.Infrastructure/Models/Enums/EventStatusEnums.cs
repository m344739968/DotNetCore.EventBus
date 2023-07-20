using System.ComponentModel;

namespace DotNetCore.EventBus.Infrastructure.Models.Enums;

/// <summary>
/// 事件状态
/// </summary>
public enum EventStatusEnums
{
    [Description("未处理")]
    Unprocessed = 0,
    [Description("处理成功")]
    Successed = 1,
    [Description("处理失败")]
    Failed = 2,
}

/// <summary>
/// 应用类型
/// </summary>
public enum ClientTypeEnums
{
    [Description("内部")]
    Internal = 0,
    [Description("外部")]
    External = 1,
}