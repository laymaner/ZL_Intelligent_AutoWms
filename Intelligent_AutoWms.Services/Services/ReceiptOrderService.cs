using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.ReceiptOrder;
using Intelligent_AutoWms.Model.RequestDTO.ReceiptOrder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Intelligent_AutoWms.Services.Services
{
    public class ReceiptOrderService : IReceiptOrderService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<ReceiptOrderService> _log;
        private readonly IPortService _portService;
        private readonly ILocationService _locationService;
        private readonly AsyncLock _mutex = new AsyncLock();

        public ReceiptOrderService(Intelligent_AutoWms_DbContext db, ILogger<ReceiptOrderService> log, IPortService portService, ILocationService locationService)
        {
            _db = db;
            _log = log;
            _portService = portService;
            _locationService = locationService;
        }

        /// <summary>
        /// 创建入库单  
        /// </summary>
        /// <param name="createReceiptOrderDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateAsync([FromBody] CreateReceiptOrderDTO createReceiptOrderDTO, long currentUserId)
        {
            try
            {
                using (await _mutex.LockAsync())
                {
                    if (string.IsNullOrWhiteSpace(createReceiptOrderDTO.Material_Code))
                    {
                        throw new Exception("The material code parameter cannot be empty");
                    }
                    if (createReceiptOrderDTO.Material_Type <= 0)
                    {
                        throw new Exception("The material type parameter cannot be empty");
                    }
                    if (string.IsNullOrWhiteSpace(createReceiptOrderDTO.Port_Code))
                    {
                        throw new Exception("The port code parameter cannot be empty");
                    }
                    var port = await _portService.GetPortByCodeAsync(createReceiptOrderDTO.Port_Code);
                    if (port.Type != (int)PortTypeEnum.Entrance)
                    {
                        throw new Exception("Entry error");
                    }
                    var receipt_Orders = await _db.Receipt_Orders.Where(m => m.Material_Code.Equals(createReceiptOrderDTO.Material_Code) && m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage).SingleOrDefaultAsync();
                    if (receipt_Orders != null)
                    {
                        throw new Exception("The material has already created a receipt, please do not create it again");
                    }
                    List<WMS_Receipt_Orders> list = new List<WMS_Receipt_Orders>();
                    var location = await _locationService.RecommendedStorageLocationAsync(port.Code);
                    WMS_Receipt_Orders receipt_Order = new WMS_Receipt_Orders();
                    receipt_Order.Order_No = GenerateOrderNoUtil.Gener("RKD") + location.Code; //生成唯一订单号流水
                    receipt_Order.Order_Type = "ZJRKD"; //自建单 build by yourself
                    receipt_Order.Material_Code = createReceiptOrderDTO.Material_Code;
                    receipt_Order.Material_Type = createReceiptOrderDTO.Material_Type;
                    receipt_Order.Location_Id = location.Id;
                    receipt_Order.Location_Code = location.Code;
                    receipt_Order.Port_Id = port.Id;
                    receipt_Order.Port_Code = port.Code;
                    receipt_Order.Receipt_Step = (int)ReceiptOrderStatusEnum.WaitingForStorage;
                    receipt_Order.Status = (int)DataStatusEnum.Normal;
                    receipt_Order.Create_Time = DateTime.Now;
                    receipt_Order.Creator = currentUserId;

                    list.Add(receipt_Order);
                    List<string> orders = list.Select(m => m.Order_No).ToList();

                    //将货位存储地址 修改为入库锁定
                    location.Step = (int)LocationStatusEnum.Warehousing_Lock;
                    location.Updator = currentUserId;
                    location.Update_Time = DateTime.Now;
                    var result = await _db.Receipt_Orders.AddAsync(receipt_Order);
                    await _db.SaveChangesAsync();

                    //根据入库单创建入库任务
                    await CreateTaskAsync(list, currentUserId);
                    return result.Entity.Id;
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids删除入库单信息
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
                            throw new Exception("The receipt_Orders id parameter is empty");
                        }
                        var receipt_Orders = await _db.Receipt_Orders.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (receipt_Orders == null)
                        {
                            throw new Exception($"No information found for receipt_Orders,id is {id}");
                        }
                        //若该入库单已生成入库任务 且入库任务未执行结束 不允许删除
                        var task = await _db.WMS_Tasks.Where(m => m.Order_No.Equals(receipt_Orders.Order_No) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (task != null)
                        {
                            throw new Exception("The inventory order has already been generated with a task and cannot be deleted");
                        }

                        var loaction = await _locationService.GetLocationByCodeAsync(receipt_Orders.Location_Code);

                        //删除入库单 将货位存储位置改为空闲状态
                        loaction.Step = (int)LocationStatusEnum.Idle;
                        loaction.Update_Time = DateTime.Now;
                        loaction.Updator = currentUserId;

                        receipt_Orders.Status = (int)DataStatusEnum.Delete;
                        receipt_Orders.Update_Time = DateTime.Now;
                        receipt_Orders.Updator = currentUserId;
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
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO)
        {
            try
            {
                var items = _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(receiptOrderParamsDTO.Material_Code));
                }
                if (receiptOrderParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == receiptOrderParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(receiptOrderParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(receiptOrderParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(receiptOrderParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(receiptOrderParamsDTO.Order_Type));
                }
                if (receiptOrderParamsDTO.Create_Time_Start != null && receiptOrderParamsDTO.Create_Time_Start != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= receiptOrderParamsDTO.Create_Time_Start);
                }
                if (receiptOrderParamsDTO.Create_Time_End != null && receiptOrderParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= receiptOrderParamsDTO.Create_Time_End);
                }
                if (receiptOrderParamsDTO.Receipt_Time_Start != null && receiptOrderParamsDTO.Receipt_Time_Start != DateTime.MinValue)
                {
                    items = items.Where(m => m.Receipt_Time >= receiptOrderParamsDTO.Receipt_Time_Start);
                }
                if (receiptOrderParamsDTO.Receipt_Time_End != null && receiptOrderParamsDTO.Receipt_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Receipt_Time <= receiptOrderParamsDTO.Receipt_Time_End);
                }
                if (receiptOrderParamsDTO.Receipt_Step > 0)
                {
                    items = items.Where(m => m.Receipt_Step == receiptOrderParamsDTO.Receipt_Step);
                }
                var result = items.Adapt<List<ReceiptOrderExportTemplate>>();
                return await MiniExcelUtil.ExportAsync("ReceiptOrderInfomation", result);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询入库单信息
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Receipt_Orders>> GetListAsync(ReceiptOrderParamsDTO receiptOrderParamsDTO)
        {
            try
            {
                var items = _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(receiptOrderParamsDTO.Material_Code));
                }
                if (receiptOrderParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == receiptOrderParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(receiptOrderParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(receiptOrderParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(receiptOrderParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(receiptOrderParamsDTO.Order_Type));
                }
                if (receiptOrderParamsDTO.Create_Time_Start != null && receiptOrderParamsDTO.Create_Time_Start != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= receiptOrderParamsDTO.Create_Time_Start);
                }
                if (receiptOrderParamsDTO.Create_Time_End != null && receiptOrderParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= receiptOrderParamsDTO.Create_Time_End);
                }
                if (receiptOrderParamsDTO.Receipt_Time_Start != null && receiptOrderParamsDTO.Receipt_Time_Start != DateTime.MinValue)
                {
                    items = items.Where(m => m.Receipt_Time >= receiptOrderParamsDTO.Receipt_Time_Start);
                }
                if (receiptOrderParamsDTO.Receipt_Time_End != null && receiptOrderParamsDTO.Receipt_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Receipt_Time <= receiptOrderParamsDTO.Receipt_Time_End);
                }
                if (receiptOrderParamsDTO.Receipt_Step > 0)
                {
                    items = items.Where(m => m.Receipt_Step == receiptOrderParamsDTO.Receipt_Step);
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
        /// 分页查询入库单信息
        /// </summary>
        /// <param name="receiptOrderParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Receipt_Orders>> GetPaginationAsync([FromQuery] ReceiptOrderParamsDTO receiptOrderParamsDTO)
        {
            try
            {
                var items = _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal).OrderByDescending(n => n.Id).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(receiptOrderParamsDTO.Material_Code));
                }
                if (receiptOrderParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == receiptOrderParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(receiptOrderParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(receiptOrderParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(receiptOrderParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(receiptOrderParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(receiptOrderParamsDTO.Order_Type));
                }
                if (receiptOrderParamsDTO.Create_Time_Start != null && receiptOrderParamsDTO.Create_Time_Start != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= receiptOrderParamsDTO.Create_Time_Start);
                }
                if (receiptOrderParamsDTO.Create_Time_End != null && receiptOrderParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= receiptOrderParamsDTO.Create_Time_End);
                }
                if (receiptOrderParamsDTO.Receipt_Time_Start != null && receiptOrderParamsDTO.Receipt_Time_Start != DateTime.MinValue)
                {
                    items = items.Where(m => m.Receipt_Time >= receiptOrderParamsDTO.Receipt_Time_Start);
                }
                if (receiptOrderParamsDTO.Receipt_Time_End != null && receiptOrderParamsDTO.Receipt_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Receipt_Time <= receiptOrderParamsDTO.Receipt_Time_End);
                }
                if (receiptOrderParamsDTO.Receipt_Step > 0)
                {
                    items = items.Where(m => m.Receipt_Step == receiptOrderParamsDTO.Receipt_Step);
                }
                return await PaginationService.PaginateAsync(items,receiptOrderParamsDTO.PageIndex,receiptOrderParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据入库单订单编码获取入库单信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Receipt_Orders> GetReceiptOrderByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The receipt_Orders code parameter is empty");
                }
                var receipt_Orders = await _db.Receipt_Orders.Where(m => m.Order_No.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (receipt_Orders == null)
                {
                    throw new Exception($"No information found for receipt_Orders,code is {code}");
                }
                return receipt_Orders;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id获取入库单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Receipt_Orders> GetReceiptOrderByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The receipt_Orders id parameter is empty");
                }
                var receipt_Orders = await _db.Receipt_Orders.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (receipt_Orders == null)
                {
                    throw new Exception($"No information found for receipt_Orders,id is {id}");
                }
                return receipt_Orders;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id判断该入库单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> IsExistAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The receipt_Orders id parameter is empty");
                }
                var receipt_Orders = await _db.Receipt_Orders.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (receipt_Orders == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
               return false;
            }
        }

        /// <summary>
        /// 根据ids重新生成入库任务
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> RegenerateTaskByIdsAsync(List<long> ids,long currentUserId)
        {
            try
            {
                using (await _mutex.LockAsync())
                {
                    if (ids == null || ids.Count <= 0)
                    {
                        throw new Exception("The ids  parameter cannot be empty");
                    }
                    var receipt_Orders = await _db.Receipt_Orders.Where(m => ids.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                    if (receipt_Orders == null || receipt_Orders.Count < ids.Count)
                    {
                        throw new Exception("Query results do not match");
                    }
                    if (receipt_Orders.Any(m => m.Receipt_Step == (int)ReceiptOrderStatusEnum.Received))
                    {
                        throw new Exception("There are Received tasks that have been completed");
                    }
                    //判断该入库单是否存在入库任务
                    var orders = receipt_Orders.Select(m => m.Order_No).ToList();
                    var tasks = await _db.WMS_Tasks.Where(m => orders.Contains(m.Order_No) && m.Status == (int)DataStatusEnum.Normal).CountAsync();
                    if (tasks > 0)
                    {
                        throw new Exception("There is an Received task, resending is not allowed");
                    }
                    await CreateTaskAsync(receipt_Orders, currentUserId);
                    return "Regenerate Tasks Success";
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 一键重新生成入库任务
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> RegenerateTaskAsync(long currentUserId)
        {
            try
            {
                using (await _mutex.LockAsync())
                {
                    var items = await _db.Receipt_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage).ToListAsync();
                    if (items != null && items.Count > 0)
                    {
                        var orderNoItems = items.Select(m => m.Order_No).Distinct().ToList();
                        var taskItem = _db.WMS_Tasks.Where(m => orderNoItems.Contains(m.Order_No) && m.Status == (int)DataStatusEnum.Delete).Select( n => n.Order_No).Distinct().ToList();
                        if (taskItem != null && taskItem.Count > 0)
                        {
                            var regenerateItems = items.Where(m => taskItem.Contains(m.Order_No)).ToList();
                            await CreateTaskAsync(regenerateItems, currentUserId);
                            return "Regenerate Tasks Success";
                        }
                        else
                        {
                            throw new Exception("There is no data that needs to be re-issued");
                        }
                    }
                    else 
                    {
                        throw new Exception("There is no Received order that is eligible for re-issuance");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据入库单生成入库任务
        /// </summary>
        /// <param name="receipt_Orders"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateTaskAsync(List<WMS_Receipt_Orders> receipt_Orders,long currentUserId)
        {
            try
            {
                if (receipt_Orders == null || receipt_Orders.Count <= 0)
                {
                    throw new Exception("The receipt_Orders  parameter cannot be empty");
                }
                List<WMS_Task> tasks = new List<WMS_Task>();
                var  taskCount =await _db.WMS_Tasks.CountAsync();
                foreach (var receipt_Order in receipt_Orders)
                {
                    WMS_Task task = new WMS_Task();
                    task.Task_Priority = 9;
                    var port = await _portService.GetPortByIdAsync(receipt_Order.Port_Id);
                    var location = await _locationService.GetLocationByIdAsync(receipt_Order.Location_Id);
                    // 判断出库口巷道是否在存储位置上的本巷道 不在则穿越出库模式
                    if (port.First_Lanway != location.Lanway)
                    {
                        task.Task_Mode = (int)TaskModeEnum.AcrossReceipt;
                    }
                    else
                    {
                        task.Task_Mode = (int)TaskModeEnum.NormalReceipt;
                    }
                    task.Task_Lanway = location.Lanway;
                    task.Destination_Position_X = location.Location_Row;
                    task.Destination_Position_Y = location.Location_Column;
                    task.Destination_Position_Z = location.Location_Layer;
                    task.Location_Id = location.Id;
                    task.Location_Code = location.Code;
                    task.Material_Code = receipt_Order.Material_Code;
                    task.Material_Type = receipt_Order.Material_Type;
                    task.Order_No = receipt_Order.Order_No;
                    task.Port_Id = port.Id;
                    task.Port_Code = port.Code;
                    task.Status = (int)DataStatusEnum.Normal;
                    task.Create_Time = DateTime.Now;
                    task.Creator = currentUserId;
                    if (receipt_Orders.IndexOf(receipt_Order) + taskCount > Int16.MaxValue)
                    {
                        task.Task_No = ((receipt_Orders.IndexOf(receipt_Order) + 1 + taskCount) % Int16.MaxValue).ToString();
                        task.Task_No_Backup = task.Task_No;

                    }
                    else
                    {
                        task.Task_No = (receipt_Orders.IndexOf(receipt_Order) + 1 + taskCount).ToString();
                        task.Task_No_Backup = task.Task_No;
                    }
                    tasks.Add(task);
                }
                await _db.BulkInsertAsync(tasks);
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
