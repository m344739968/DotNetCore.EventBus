using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class EventBusSubscribeRequest
{
    /// <summary>
    /// 主键id（修改必传）
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 事件名称
    /// </summary>           
    public string EventName { get; set; }

    /// <summary>
    /// 应用类型 0内部应用，1外部应用
    /// </summary>
    public int? ClientType { get; set; }

    /// <summary>
    /// 事件订阅应用id
    /// </summary>           
    public string ClientId { get; set; }

    /// <summary>
    /// 事件订阅应用密钥
    /// </summary>           
    public string ClientSecret { get; set; }

    /// <summary>
    /// 事件订阅应用获取token地址
    /// </summary>
    public string TokenUrl { get; set; }

    /// <summary>
    /// 事件订阅应用api地址
    /// </summary>           
    public string ApiUrl { get; set; }

    /// <summary>
    /// token缓存时间单位：秒
    /// 默认：86400s
    /// </summary>  
    public decimal? TokenCacheDuration { get; set; } = 86400M;

    /// <summary>
    /// 是否验证token
    /// 默认：true
    /// </summary>  
    public bool? IsValidToken { get; set; } = true;

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public string Operator { get; set; }

}
