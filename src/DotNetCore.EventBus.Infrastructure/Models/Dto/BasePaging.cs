using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DotNetCore.EventBus.Infrastructure.Models.Dto;

/// <summary>
/// 分页对象
/// </summary>
public class BasePaging
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页显示条数
    /// </summary>
    public int PageSize { get; set; } = 10;
}