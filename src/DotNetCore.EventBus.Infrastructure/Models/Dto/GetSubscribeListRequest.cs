using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class GetSubscribeListRequest : BasePaging
{
    /// <summary>
    /// 应用id
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }
}

public class QuerySubscribeEventListResponse
{
    /// <summary>
    /// Desc:主键编号
    /// Default:
    /// Nullable:False
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Desc:事件名称
    /// Default:
    /// Nullable:False
    /// </summary> 
    public string EventName { get; set; }

    /// <summary>
    /// Desc:事件订阅应用id
    /// Default:
    /// Nullable:False
    /// </summary> 
    public string ClientId { get; set; }

    /// <summary>
    /// Desc:事件订阅应用密钥
    /// Default:
    /// Nullable:False
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    /// Desc: 应用类别，0内部应用，1外部应用
    /// Default:
    /// Nullable:False
    /// </summary>   
    public int? ClientType { get; set; }

    /// <summary>
    /// Desc:事件订阅应用获取token地址
    /// Default:
    /// Nullable:False
    /// </summary>
    public string TokenUrl { get; set; }

    /// <summary>
    /// Desc:事件订阅应用api地址
    /// Default:
    /// Nullable:False
    /// </summary>
    public string ApiUrl { get; set; }

    /// <summary>
    /// Desc: token缓存时间单位：秒
    /// Default:
    /// Nullable:False
    /// </summary>    
    public decimal? TokenCacheDuration { get; set; }

    /// <summary>
    /// Desc: 是否验证token
    /// Default:
    /// Nullable:False
    /// </summary>
    public bool? IsValidToken { get; set; }

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

    /// <summary>
    /// Desc:修改人id
    /// Default:
    /// Nullable:True
    /// </summary>
    public string ModifiedBy { get; set; }

    /// <summary>
    /// Desc:修改人名称
    /// Default:
    /// Nullable:True
    /// </summary>
    public string ModifiedName { get; set; }

    /// <summary>
    /// Desc:修改时间
    /// Default:
    /// Nullable:True
    /// </summary>
    public DateTime? ModifiedTime { get; set; }

    /// <summary>
    /// Desc:备注
    /// Default:
    /// Nullable:True
    /// </summary>
    public string Remark { get; set; }
}
