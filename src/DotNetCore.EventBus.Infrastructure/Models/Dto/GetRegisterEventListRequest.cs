using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.EventBus.Infrastructure.Models.Dto;
public class GetRegisterEventListRequest : BasePaging
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public string EventName { get; set; }
}

///<summary>
/// 注册事件列表
///</summary>
public class QueryRegisterEventListResponse
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
    /// Desc:事件参数
    /// Default:
    /// Nullable:False
    /// </summary>
    public string EventParams { get; set; }

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

    /// <summary>
    /// Desc:扩展字段
    /// Default:
    /// Nullable:True
    /// </summary>
    public string Extend { get; set; }
}
