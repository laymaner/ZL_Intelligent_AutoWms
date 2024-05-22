using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.OperateLog;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IOperateLogService
    {
        /// <summary>
        /// 查询操作日志信息
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_Operate_Log>> GetListAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO);

        /// <summary>
        /// 查询操作日志分页
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Operate_Log>> GetPaginationAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO);

        /// <summary>
        /// 根据id查询操作日志信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Operate_Log> GetOperateLogByIdAsync(long id);

        /// <summary>
        /// 根据ids删除操作日志信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO);
    }
}
