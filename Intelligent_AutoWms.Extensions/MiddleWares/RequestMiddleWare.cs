using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Intelligent_AutoWms.Extensions.MiddleWares
{
    public class RequestMiddleWare
    {
        private readonly RequestDelegate _next;  // 用来处理上下文请求  
        private readonly ILogger<RequestMiddleWare> _logger;
        private readonly Intelligent_AutoWms_DbContext _db;

        public RequestMiddleWare(RequestDelegate next, ILogger<RequestMiddleWare> logger,Intelligent_AutoWms_DbContext db)
        {
            _next = next;
            _logger = logger;
            _db = db;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            #region
            /* // 过滤，只有接口
             if (httpContext.Request.Path.Value.Contains("api"))
             {

                 WMS_Operate_Log operate_Log = new WMS_Operate_Log();

                 httpContext.Request.EnableBuffering();

                 // 请求数据处理
                 await RequestDataHandle(httpContext, operate_Log);

                 await _next(httpContext);

                 // 响应数据处理
                 ResponseDataHandle(httpContext.Response, operate_Log);
             }
             else
             {
                 await _next(httpContext);
             }*/
            #endregion

            WMS_Operate_Log operate_Log = new WMS_Operate_Log();

            try
            {
                // 过滤，只有接口
                if (httpContext.Request.Path.Value.Contains("api"))
                {
                    httpContext.Request.EnableBuffering();

                    // 请求数据处理
                    await RequestDataHandle(httpContext, operate_Log);

                    await _next(httpContext);               
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex,operate_Log); // 捕获异常了 在HandleExceptionAsync中处理
            }
            finally 
            {
                if (httpContext.Request.Path.Value.Contains("api"))
                {
                    // 响应数据处理
                    ResponseDataHandle(httpContext.Response, operate_Log);
                }
            }
        }


        private async Task HandleExceptionAsync(HttpContext context, Exception exception, WMS_Operate_Log operate_Log)
        {
            context.Response.ContentType = "application/json";  // 返回json 类型
            var response = context.Response;
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var result = new ErrorResponse
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
            operate_Log.Operate_Status = result.StatusCode;
            operate_Log.Error_Msg = result.Message;
            _logger.LogError(exception.Message);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(result,settings));
        }

        /// <summary>
        /// 请求数据处理
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private async Task RequestDataHandle(HttpContext httpContext, WMS_Operate_Log operate_Log)
        {
            try 
            {
                if (httpContext.Request.Method == "POST" && httpContext.Request.ContentLength.Value > 0)
                {
                    var body = httpContext.Request.Body;
                    var buffer = new byte[httpContext.Request.ContentLength.Value];
                    await body.ReadAsync(buffer, 0, buffer.Length);
                    operate_Log.Operate_Params = Encoding.UTF8.GetString(buffer);
                    //重置字节流读取下标
                    body.Position = 0;
                }
                else
                {
                    operate_Log.Operate_Params = httpContext.Request.QueryString.Value;
                }
                var user = httpContext.User;
                if (httpContext.GetRouteValue("action").ToString().Equals("Login"))
                {
                    UserLoginDTO userDTO = JsonConvert.DeserializeObject<UserLoginDTO>(operate_Log.Operate_Params);
                    userDTO.Password = MD5EncryptionUtil.Encrypt(userDTO.Password);
                    operate_Log.Operate_Params = JsonConvert.SerializeObject(userDTO);
                }
                if (user != null && user.Identities.Any(identity => identity.IsAuthenticated))
                {
                    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var item = await _db.Users.Where(m => m.Id == long.Parse(userId) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (user != null)
                        {
                            operate_Log.User_Code = item.Code;
                            operate_Log.User_Name = item.Name;
                            operate_Log.Creator = item.Id;
                        }
                    }
                }


                operate_Log.Operate_Type = httpContext.Request.Method;
                operate_Log.Ip_Address = httpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                operate_Log.Operate_Url = httpContext.Request.Path;
                operate_Log.Title = httpContext.GetRouteValue("controller").ToString();
                operate_Log.Method_Name = httpContext.GetRouteValue("action").ToString();
                operate_Log.Operate_Url = httpContext.Request.Path;
            }
            catch (Exception ex) 
            {
                _logger.LogDebug(ex.Message);
            }          
        }

        /// <summary>
        /// 响应数据处理
        /// </summary>
        /// <param name="response"></param>
        [Transation]
        private void  ResponseDataHandle(HttpResponse response, WMS_Operate_Log operate_Log)
        {
            try
            {
                if (string.IsNullOrEmpty(operate_Log.Error_Msg))
                {
                    operate_Log.Operate_Status = response.StatusCode;
                }
                operate_Log.Create_Time = DateTime.Now;
                operate_Log.Status = (int)DataStatusEnum.Normal;
                _db.Operate_Logs.Add(operate_Log);
                _db.SaveChanges();
            }
            catch (Exception ex) 
            {
                _logger.LogDebug(ex.Message);
            }
        }
    }

    public class ErrorResponse
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
