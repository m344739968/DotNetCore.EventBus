using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DotNetCore.EventBus.Infrastructure.Filter;

/// <summary>
/// 全局异常拦截器
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter, IFilterMetadata
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger) {
        _logger = logger;
    }

    /// <summary>
    /// 捕获异常
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        if (context.Exception.GetType() == typeof(BusOperationException))
        {
            ExceptionBase exceptionBase = context.Exception as ExceptionBase;
            context.Result = new JsonResult(new
            {
                code = exceptionBase?.Code ?? "fail",
                msg = exceptionBase?.Message,
            });
            context.HttpContext.Response.StatusCode = 500;
            _logger.LogError(exceptionBase?.InnerException, "业务异常：" + exceptionBase?.Message);
        }
        else
        {
            var exceptionBase = context.Exception;
            context.Result = new JsonResult(new
            {
                code = "fail",
                msg = exceptionBase?.Message,
            });
            context.HttpContext.Response.StatusCode = 500;
            _logger.LogError(exceptionBase?.InnerException, "发生异常：" + exceptionBase?.Message);
        }
    }
}

public class BusOperationException : ExceptionBase
{
    public BusOperationException()
    {
    }

    public BusOperationException(string Message) : base(Message)
    {
        
    }

    public BusOperationException(string Code, string Message)
        : base(Code, Message)
    {
        base.Code = Code;
    }

    public BusOperationException(string Code, string Message, Exception InnerException)
        : base(Code, Message, InnerException)
    {
        base.Code = Code;
    }
}

public class ExceptionBase : Exception
{

    public string Code
    {
        get;
        set;
    }

    public ExceptionBase()
    {
    }

    public ExceptionBase(string Message): base(Message)
    {
        this.Code = "fail";
    }

    public ExceptionBase(string Code, string Message)
        : base(Message)
    {
        this.Code = Code;
    }

    public ExceptionBase(string Code, string Message, Exception InnerException)
        : base(Message, InnerException)
    {
        this.Code = Code;
    }
}

public static class ExceptionExt {
    /// <summary>
    /// 抛出异常
    /// </summary>
    /// <param name="ex"></param>
    /// <exception cref="BusOperationException"></exception>
    /// <exception cref="Exception"></exception>
    public static void ExceptionThrow(this Exception ex) {
        if (ex.GetType() == typeof(BusOperationException))
        {
            ExceptionBase exceptionBase = ex as ExceptionBase;
            throw new BusOperationException(exceptionBase.Code, exceptionBase.Message);
        }
        else
        {
            throw new Exception(ex.Message, ex);
        }
    }
}
