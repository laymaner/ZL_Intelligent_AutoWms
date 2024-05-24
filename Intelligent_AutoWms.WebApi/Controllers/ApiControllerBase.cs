using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Model.BaseModel;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// 成功状态返回结果
        /// </summary>
        /// <param name="result">返回的数据</param>
        /// <returns></returns>
        protected ApiResult<T> SuccessResult<T>(T result)
        {
            return ApiResult<T>.SuccessResult(result);
        }

        /// <summary>
        /// 失败状态返回结果
        /// </summary>
        /// <param name="msg">失败信息</param>
        /// <returns></returns>
        protected ApiResult<T> FailResult<T>(string? msg = null)
        {
            return ApiResult<T>.FailResult(msg);
        }

        /// <summary>
        /// 异常状态返回结果
        /// </summary>
        /// <param name="msg">异常信息</param>
        /// <returns></returns>
        protected ApiResult<T> ErrorResult<T>(string? msg = null)
        {
            return ApiResult<T>.ErrorResult(msg);
        }

        /// <summary>
        /// 自定义状态返回结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="status"></param>
        /// <param name="isSuccess"></param>
        /// <param name="result"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ApiResult<T> Result<T>(ResultStatusEnum status, bool isSuccess, T result, string? msg = null)
        {
            return ApiResult<T>.Result(status, isSuccess, result, msg);
        }
    }
}
