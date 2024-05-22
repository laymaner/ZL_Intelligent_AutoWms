using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;

namespace Intelligent_AutoWms.Model.BaseModel
{
    /// <summary>
    /// 封装api返回值数据传输对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T>
    {
        /// <summary>
        /// 状态结果
        /// </summary>
        public ResultStatusEnum StatusCode { get; set; } = ResultStatusEnum.Success;

        /// <summary>
        /// api调用是否成功
        /// </summary>
        public bool Success { get; set; } = false;

        private string? _msg;

        /// <summary>
        /// 消息描述
        /// </summary>
        public string? Message
        {
            get
            {
                return !string.IsNullOrEmpty(_msg) ? _msg : EnumUtil.GetEnumDescription(StatusCode);
            }
            set
            {
                _msg = value;
            }
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 成功状态返回结果
        /// </summary>
        /// <param name="result">返回的数据</param>
        /// <returns></returns>
        public static ApiResult<T> SuccessResult(T data)
        {
            return new ApiResult<T> { StatusCode = ResultStatusEnum.Success, Success = true, Data = data };
        }

        /// <summary>
        /// 失败状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">失败信息</param>
        /// <returns></returns>
        public static ApiResult<T> FailResult(string? msg = null)
        {
            return new ApiResult<T> { StatusCode = ResultStatusEnum.Fail, Success = false, Message = msg };
        }

        /// <summary>
        /// 请求资源未找到,失败状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">失败信息</param>
        /// <returns></returns>
        public static ApiResult<T> NotFoundResult(string? msg = null)
        {
            return new ApiResult<T> { StatusCode = ResultStatusEnum.NotFound, Success = false, Message = msg };
        }

        /// <summary>
        /// 异常状态返回结果
        /// </summary>
        /// <param name="code">状态码</param>
        /// <param name="msg">异常信息</param>
        /// <returns></returns>
        public static ApiResult<T> ErrorResult(string? msg = null)
        {
            return new ApiResult<T> { StatusCode = ResultStatusEnum.Error, Success = false, Message = msg };
        }

        /// <summary>
        /// 自定义状态返回结果
        /// </summary>
        /// <param name="status"></param>  
        /// <param name="result"></param>
        /// <returns></returns>
        public static ApiResult<T> Result(ResultStatusEnum status, bool isSuccess, T data, string? msg = null)
        {
            return new ApiResult<T> { StatusCode = status, Success = isSuccess, Data = data, Message = msg };
        }

        /// <summary>
        /// 隐式将T转化为ResponseResult<T>
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ApiResult<T>(T value)
        {
            return new ApiResult<T> { Data = value };
        }


    }
}
