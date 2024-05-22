using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Task;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface ITaskService
    {
        /// <summary>
        /// 查询任务信息
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_Task>> GetListAsync(TaskParamsDTO taskParamsDTO);

        /// <summary>
        /// 查询任务信息分页
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Task>> GetPaginationAsync([FromQuery] TaskParamsDTO taskParamsDTO);

        /// <summary>
        /// 根据id查询任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Task> GetTaskByIdAsync(long id);

        /// <summary>
        /// 根据ids结束任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> FinishTask(string ids, long currentUserId);

        /// <summary>
        /// 根据ids删除任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] TaskParamsDTO taskParamsDTO);

        /// <summary>
        /// 根据订单编码查询任务信息
        /// </summary>
        /// <param name="orderNos"></param>
        /// <returns></returns>
        public Task<List<WMS_Task>> GetTaskByCodesAsync(string orderNos);

    }
}
