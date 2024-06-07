using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.OperateLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class OperateLogController : ApiControllerBase
    {
        private readonly IOperateLogService _operateLogService;

        /// <summary>
        /// 
        /// </summary>
        public OperateLogController(IOperateLogService operateLogService)
        {
            _operateLogService = operateLogService;
        }

        /// <summary>
        /// 查询操作日志信息
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Operate_Log>>> GetListAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO)
        {
            var result = await _operateLogService.GetListAsync(operateLogParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询操作日志分页
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Operate_Log>>> GetPaginationAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO)
        {
            var result = await _operateLogService.GetPaginationAsync(operateLogParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询操作日志信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Operate_Log>> GetOperateLogByIdAsync(long id)
        {
            var result = await _operateLogService.GetOperateLogByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除操作日志信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _operateLogService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO)
        {
            return await _operateLogService.ExportAsync(operateLogParamsDTO);
        }
    }
}
