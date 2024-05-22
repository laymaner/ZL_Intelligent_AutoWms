using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.OperateLog;
using Intelligent_AutoWms.Model.RequestDTO.OperateLog;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class OperateLogService : IOperateLogService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<OperateLogService> _log;

        public OperateLogService(Intelligent_AutoWms_DbContext db, ILogger<OperateLogService> log)
        {
            _db = db;
            _log = log;
        }

        /// <summary>
        /// 根据ids删除操作日志
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> DelAsync(string ids, long currentUserId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var result = ids.Split(',').ToList();
                    List<long> idList = result.Select(s => long.Parse(s)).ToList();
                    foreach (var id in idList)
                    {
                        if (id <= 0)
                        {
                            throw new Exception("The OperateLog id parameter is empty");
                        }
                        var operate_Log = await _db.Operate_Logs.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (operate_Log == null)
                        {
                            throw new Exception($"No information found for OperateLog,id is {id}");
                        }

                        operate_Log.Status = (int)DataStatusEnum.Delete;
                        operate_Log.Update_Time = DateTime.Now;
                        operate_Log.Updator = currentUserId;
                    }
                    return await _db.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("The ids parameter is empty");
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO)
        {
            try
            {
                List<OperateLogExportTemplate> list = new List<OperateLogExportTemplate>();
                var items = _db.Operate_Logs.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.User_Name))
                {
                    items = items.Where(m => m.User_Name.StartsWith(operateLogParamsDTO.User_Name));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.User_Code))
                {
                    items = items.Where(m => m.User_Code.StartsWith(operateLogParamsDTO.User_Code));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Method_Name))
                {
                    items = items.Where(m => m.Method_Name.StartsWith(operateLogParamsDTO.Method_Name));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Title))
                {
                    items = items.Where(m => m.Title.StartsWith(operateLogParamsDTO.Title));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Ip_Address))
                {
                    items = items.Where(m => m.Ip_Address.StartsWith(operateLogParamsDTO.Ip_Address));
                }
                if (operateLogParamsDTO.Operate_Status != null && operateLogParamsDTO.Operate_Status > 0)
                {
                    items = items.Where(m => m.Operate_Status == operateLogParamsDTO.Operate_Status);
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Operate_Type))
                {
                    items = items.Where(m => m.Operate_Type.Equals(operateLogParamsDTO.Operate_Type));
                }
                if (operateLogParamsDTO.Start_Time != null && operateLogParamsDTO.Start_Time !=DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= operateLogParamsDTO.Start_Time);
                }
                if (operateLogParamsDTO.End_Time != null && operateLogParamsDTO.End_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= operateLogParamsDTO.End_Time);
                }

                var result = await items.ToListAsync();
                return await MiniExcelUtil.ExportAsync("OperateLogInfomation", result);           
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询操作日志信息
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Operate_Log>> GetListAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO)
        {
            try
            {
                var items = _db.Operate_Logs.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.User_Name))
                {
                    items = items.Where(m => m.User_Name.StartsWith(operateLogParamsDTO.User_Name));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.User_Code))
                {
                    items = items.Where(m => m.User_Code.StartsWith(operateLogParamsDTO.User_Code));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Method_Name))
                {
                    items = items.Where(m => m.Method_Name.StartsWith(operateLogParamsDTO.Method_Name));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Title))
                {
                    items = items.Where(m => m.Title.StartsWith(operateLogParamsDTO.Title));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Ip_Address))
                {
                    items = items.Where(m => m.Ip_Address.StartsWith(operateLogParamsDTO.Ip_Address));
                }
                if (operateLogParamsDTO.Operate_Status != null && operateLogParamsDTO.Operate_Status > 0)
                {
                    items = items.Where(m => m.Operate_Status == operateLogParamsDTO.Operate_Status);
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Operate_Type))
                {
                    items = items.Where(m => m.Operate_Type.Equals(operateLogParamsDTO.Operate_Type));
                }
                if (operateLogParamsDTO.Start_Time != null && operateLogParamsDTO.Start_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= operateLogParamsDTO.Start_Time);
                }
                if (operateLogParamsDTO.End_Time != null && operateLogParamsDTO.End_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= operateLogParamsDTO.End_Time);
                }
                var result = await items.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id获取操作日志信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Operate_Log> GetOperateLogByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The operateLog id parameter is empty");
                }
                var operate_Log = await _db.Operate_Logs.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (operate_Log == null)
                {
                    throw new Exception($"No information found for operateLog,id is {id}");
                }
                return operate_Log;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 分页查询操作日志信息
        /// </summary>
        /// <param name="operateLogParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Operate_Log>> GetPaginationAsync([FromQuery] OperateLogParamsDTO operateLogParamsDTO)
        {
            try
            {
                var items = _db.Operate_Logs.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.User_Name))
                {
                    items = items.Where(m => m.User_Name.StartsWith(operateLogParamsDTO.User_Name));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.User_Code))
                {
                    items = items.Where(m => m.User_Code.StartsWith(operateLogParamsDTO.User_Code));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Method_Name))
                {
                    items = items.Where(m => m.Method_Name.StartsWith(operateLogParamsDTO.Method_Name));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Title))
                {
                    items = items.Where(m => m.Title.StartsWith(operateLogParamsDTO.Title));
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Ip_Address))
                {
                    items = items.Where(m => m.Ip_Address.StartsWith(operateLogParamsDTO.Ip_Address));
                }
                if (operateLogParamsDTO.Operate_Status != null && operateLogParamsDTO.Operate_Status > 0)
                {
                    items = items.Where(m => m.Operate_Status == operateLogParamsDTO.Operate_Status);
                }
                if (!string.IsNullOrWhiteSpace(operateLogParamsDTO.Operate_Type))
                {
                    items = items.Where(m => m.Operate_Type.Equals(operateLogParamsDTO.Operate_Type));
                }
                if (operateLogParamsDTO.Start_Time != null && operateLogParamsDTO.Start_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= operateLogParamsDTO.Start_Time);
                }
                if (operateLogParamsDTO.End_Time != null && operateLogParamsDTO.End_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= operateLogParamsDTO.End_Time);
                }
                return await PaginationService.PaginateAsync(items, operateLogParamsDTO.PageIndex,operateLogParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
