using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Task;
using Intelligent_AutoWms.Model.RequestDTO.Inventory;
using Intelligent_AutoWms.Model.RequestDTO.Task;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class TaskService : ITaskService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<TaskService> _log;
        private readonly IInventoryService _inventoryService;
        private readonly IReceiptOrderService _receiptOrderService;
        private readonly IDeliveryOrderService _deliveryOrderService;
        private readonly ILocationService _locationService;

        public TaskService(Intelligent_AutoWms_DbContext db, ILogger<TaskService> log, IInventoryService inventoryService, IReceiptOrderService receiptOrderService, IDeliveryOrderService deliveryOrderService, ILocationService locationService)
        {
            _db = db;
            _log = log;
            _inventoryService = inventoryService;
            _receiptOrderService = receiptOrderService;
            _deliveryOrderService = deliveryOrderService;
            _locationService = locationService;
        }

        /// <summary>
        /// 根据ids删除任务
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> DelAsync(string ids,long currentUserId)
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
                            throw new Exception("The task id parameter is empty");
                        }
                        var task = await _db.WMS_Tasks.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (task == null)
                        {
                            throw new Exception($"No information found for task,id is {id}");
                        }


                        task.Status = (int)DataStatusEnum.Delete;
                        task.Update_Time = DateTime.Now;
                        task.Updator = currentUserId;
                        task.Remark = "删除任务";
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
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] TaskParamsDTO taskParamsDTO)
        {
            try
            {
                var items = _db.WMS_Tasks.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Task_No))
                {
                    items = items.Where(m => m.Task_No.Equals(taskParamsDTO.Task_No));
                }
                if (taskParamsDTO.Task_Mode > 0)
                {
                    items = items.Where(m => m.Task_Mode == taskParamsDTO.Task_Mode);
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(taskParamsDTO.Material_Code));
                }
                if (taskParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == taskParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(taskParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(taskParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(taskParamsDTO.Order_No));
                }
                if (taskParamsDTO.Status > 0)
                {
                    items = items.Where(m => m.Status == taskParamsDTO.Status);
                }
                if (taskParamsDTO.Create_Time_Strat != null && taskParamsDTO.Create_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= taskParamsDTO.Create_Time_Strat);
                }
                if (taskParamsDTO.Create_Time_End != null && taskParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= taskParamsDTO.Create_Time_End);
                }
                if (taskParamsDTO.Task_Time_Strat != null && taskParamsDTO.Task_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Task_End_Time >= taskParamsDTO.Task_Time_Strat);
                }
                if (taskParamsDTO.Task_Time_End != null && taskParamsDTO.Task_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Task_End_Time <= taskParamsDTO.Task_Time_End);
                }
                var result = items.Adapt<List<TaskExportTemplate>>();
                return await MiniExcelUtil.ExportAsync("TaskInfomation", result);

            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids结束任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> FinishTask(string ids, long currentUserId)
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
                            throw new Exception("The task id parameter is empty");
                        }
                        var task = await _db.WMS_Tasks.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (task == null)
                        {
                            throw new Exception($"No information found for task,id is {id}");
                        }


                        if (task.Task_Mode == (int)TaskModeEnum.NormalReceipt || task.Task_Mode == (int)TaskModeEnum.AcrossReceipt)
                        {
                            //入库单结束
                            var receipt_Orders = await _receiptOrderService.GetReceiptOrderByCodeAsync(task.Order_No);
                            if (receipt_Orders.Receipt_Step != (int)ReceiptOrderStatusEnum.WaitingForStorage)
                            {
                                throw new Exception("Inventory status error");
                            }
                            //修改入库单状态
                            receipt_Orders.Receipt_Step = (int)ReceiptOrderStatusEnum.Received;
                            receipt_Orders.Receipt_Time = DateTime.Now;
                            receipt_Orders.Update_Time = DateTime.Now;
                            receipt_Orders.Updator = currentUserId;

                            //创建库存
                            CreateInventoryDTO createInventoryDTO = new CreateInventoryDTO();
                            createInventoryDTO.Material_Code = receipt_Orders.Material_Code;
                            createInventoryDTO.Material_Type = receipt_Orders.Material_Type;
                            createInventoryDTO.Location_Code = receipt_Orders.Location_Code;
                            createInventoryDTO.Order_No = receipt_Orders.Order_No;
                            createInventoryDTO.Order_Type = receipt_Orders.Order_Type;
                            createInventoryDTO.Port_Code = receipt_Orders.Port_Code;
                            await _inventoryService.CreateAsync(createInventoryDTO, currentUserId);


                        }
                        else
                        {
                            //出库单结束
                            var delivery_Orders = await _deliveryOrderService.GetDeliveryOrderByCodeAsync(task.Order_No);
                            var inventory = await _db.Inventories.Where(m => m.Material_Code.Equals(delivery_Orders.Material_Code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                            var location = await _locationService.GetLocationByCodeAsync(delivery_Orders.Location_Code);
                            if (delivery_Orders.Delivery_Step != (int)DeliveryOrderStatusEnum.WaitingForOutbound)
                            {
                                throw new Exception("Error in the status of the outbound order");
                            }
                            if (inventory == null)
                            {
                                throw new Exception($"No information found for inventory,Material_Code is {delivery_Orders.Material_Code}");
                            }
                            //修改出库单状态
                            delivery_Orders.Delivery_Step = (int)DeliveryOrderStatusEnum.Outbound;
                            delivery_Orders.Delivery_Time = DateTime.Now;
                            delivery_Orders.Update_Time = DateTime.Now;
                            delivery_Orders.Updator = currentUserId;
                            //删除库存
                            inventory.Status = (int)DataStatusEnum.Delete;
                            inventory.Update_Time = DateTime.Now;
                            inventory.Updator = currentUserId;
                            //修改存储货位状态
                            location.Step = (int)LocationStatusEnum.Idle;
                            location.Update_Time = DateTime.Now;
                            location.Updator = currentUserId;

                        }
                        task.Task_Execute_Flag = 1;
                        task.Status = (int)DataStatusEnum.Delete;
                        task.Update_Time = DateTime.Now;
                        task.Task_End_Time = DateTime.Now;
                        task.Updator = currentUserId;
                        task.Remark = "结束任务";
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
        /// 查询任务信息
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Task>> GetListAsync(TaskParamsDTO taskParamsDTO)
        {
            try
            {
                var items = _db.WMS_Tasks.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Task_No))
                {
                    items = items.Where(m => m.Task_No.Equals(taskParamsDTO.Task_No));
                }
                if (taskParamsDTO.Task_Mode > 0)
                {
                    items = items.Where(m => m.Task_Mode == taskParamsDTO.Task_Mode);
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(taskParamsDTO.Material_Code));
                }
                if (taskParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == taskParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(taskParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(taskParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(taskParamsDTO.Order_No));
                }
                if (taskParamsDTO.Status > 0)
                {
                    items = items.Where(m => m.Status == taskParamsDTO.Status);
                }
                if (taskParamsDTO.Create_Time_Strat != null && taskParamsDTO.Create_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= taskParamsDTO.Create_Time_Strat);
                }
                if (taskParamsDTO.Create_Time_End != null && taskParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= taskParamsDTO.Create_Time_End);
                }
                if (taskParamsDTO.Task_Time_Strat != null && taskParamsDTO.Task_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Task_End_Time >= taskParamsDTO.Task_Time_Strat);
                }
                if (taskParamsDTO.Task_Time_End != null && taskParamsDTO.Task_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Task_End_Time <= taskParamsDTO.Task_Time_End);
                }
                return await items.ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询任务信息分页
        /// </summary>
        /// <param name="taskParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Task>> GetPaginationAsync([FromQuery] TaskParamsDTO taskParamsDTO)
        {
            try
            {
                var items = _db.WMS_Tasks.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Task_No))
                {
                    items = items.Where(m => m.Task_No.Equals(taskParamsDTO.Task_No));
                }
                if (taskParamsDTO.Task_Mode > 0)
                {
                    items = items.Where(m => m.Task_Mode == taskParamsDTO.Task_Mode);
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(taskParamsDTO.Material_Code));
                }
                if (taskParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == taskParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(taskParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(taskParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(taskParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(taskParamsDTO.Order_No));
                }
                if (taskParamsDTO.Status > 0)
                {
                    items = items.Where(m => m.Status == taskParamsDTO.Status);
                }
                if (taskParamsDTO.Create_Time_Strat != null && taskParamsDTO.Create_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= taskParamsDTO.Create_Time_Strat);
                }
                if (taskParamsDTO.Create_Time_End != null && taskParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= taskParamsDTO.Create_Time_End);
                }
                if (taskParamsDTO.Task_Time_Strat != null && taskParamsDTO.Task_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Task_End_Time >= taskParamsDTO.Task_Time_Strat);
                }
                if (taskParamsDTO.Task_Time_End != null && taskParamsDTO.Task_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Task_End_Time <= taskParamsDTO.Task_Time_End);
                }
                return await PaginationService.PaginateAsync(items, taskParamsDTO.PageIndex, taskParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 根据订单编码查询任务信息
        /// </summary>
        /// <param name="orderNos"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Task>> GetTaskByCodesAsync(string orderNos)
        {
            try 
            {
                var list = new List<WMS_Task>();
                if (!string.IsNullOrWhiteSpace(orderNos))
                {
                    var codeList = orderNos.Split(',').ToList();
                    list = await _db.WMS_Tasks.Where(m => codeList.Contains(m.Order_No) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                }
                return list;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id查询任务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Task> GetTaskByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The task id parameter is empty");
                }
                var task = await _db.WMS_Tasks.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (task == null)
                {
                    throw new Exception($"No information found for task,id is {id}");
                }
                return task;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
