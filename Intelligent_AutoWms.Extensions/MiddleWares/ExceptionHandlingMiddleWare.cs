using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Intelligent_AutoWms.Extensions.MiddleWares
{
    public class ExceptionHandlingMiddleWare
    {
        private readonly RequestDelegate _next;  // 用来处理上下文请求  
        private readonly ILogger<ExceptionHandlingMiddleWare> _logger;
        public ExceptionHandlingMiddleWare(RequestDelegate next, ILogger<ExceptionHandlingMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); //要么在中间件中处理，要么被传递到下一个中间件中去
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex); // 捕获异常了 在HandleExceptionAsync中处理
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";  // 返回json 类型
            var response = context.Response;

            var result = new ErrorResponseInfo
            {
                Success = false,
                Message = exception.Message,
            };
            switch (exception)
            {
                case ApplicationException ex:
                    if (ex.Message.Contains("Invalid token"))
                    {
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        result.StatusCode = response.StatusCode;
                        result.Message = ex.Message;
                        break;
                    }
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    result.StatusCode = response.StatusCode;
                    result.Message = ex.Message;
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    result.StatusCode = response.StatusCode;
                    result.Message = ex.Message;
                    break;
                default:
                    result.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
            }
            _logger.LogError(exception.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        }

    }

    public class ErrorResponseInfo
    {
        /// <summary>
        /// 状态编码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// 信息描述
        /// </summary>
        public string? Message { get; set; }


    }
}
