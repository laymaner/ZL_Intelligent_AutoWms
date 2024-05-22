using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 任务
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TaskController : ApiControllerBase
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskService"></param>
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// 查询任务信息
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Task>>> GetListAsync(TaskParamsDTO taskParamsDTO)
        {
            var result = await _taskService.GetListAsync(taskParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询任务信息分页
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Task>>> GetPaginationAsync([FromQuery] TaskParamsDTO taskParamsDTO)
        {
            var result = await _taskService.GetPaginationAsync(taskParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id查询任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Task>> GetTaskByIdAsync(long id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids结束任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> FinishTask(string ids)
        {
            var result = await _taskService.FinishTask(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids删除任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var result = await _taskService.DelAsync(ids, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] TaskParamsDTO taskParamsDTO)
        {
            return await _taskService.ExportAsync(taskParamsDTO);
        }

        /// <summary>
        /// 根据订单编码查询任务信息
        /// </summary>
        /// <param name="orderNos"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Task>>> GetTaskByCodesAsync(string orderNos)
        {
            var result = await _taskService.GetTaskByCodesAsync(orderNos);
            return SuccessResult(result);
        }

    }
}
