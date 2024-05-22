using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.DeliveryOrder;
using Intelligent_AutoWms.Model.RequestDTO.DeliveryOrder;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<DeliveryOrderService> _log;
        private readonly IPortService _portService;
        private readonly ILocationService _locationService;

        public DeliveryOrderService(Intelligent_AutoWms_DbContext db, ILogger<DeliveryOrderService> log, IPortService portService, ILocationService locationService)
        {
            _db = db;
            _log = log;
            _portService = portService;
            _locationService = locationService;
        }

        /// <summary>
        /// 创建出库单
        /// </summary>
        /// <param name="createDeliveryOrderDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateAsync([FromBody] CreateDeliveryOrderDTO createDeliveryOrderDTO, long currentUserId)
        {
            try
            {
                if (createDeliveryOrderDTO.ids == null || createDeliveryOrderDTO.ids.Count <= 0)
                {
                    throw new Exception("The ids  parameter cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createDeliveryOrderDTO.Port_Code))
                {
                    throw new Exception("The port code parameter cannot be empty");
                }
                var inventories= await _db.Inventories.Where(m => createDeliveryOrderDTO.ids.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (inventories == null || inventories.Count <= 0)
                {
                    throw new Exception("Inventory details not found based on ID query");
                }
                if (createDeliveryOrderDTO.ids.Count != inventories.Count)
                {
                    throw new Exception("The quantity of generated outbound orders does not match");
                }
                if (inventories.Any(m => m.Is_Lock =="Y"))
                {
                    throw new Exception("Inventory details are in a locked state");
                }
                var port = await _portService.GetPortByCodeAsync(createDeliveryOrderDTO.Port_Code);
                List<WMS_Delivery_Orders> list = new List<WMS_Delivery_Orders>();
                foreach (var item in inventories)
                {
                    //创建出库单
                    WMS_Delivery_Orders delivery_Orders = new WMS_Delivery_Orders();
                    delivery_Orders.Order_No = GenerateOrderNoUtil.Gener("CKD")+port.Code;//生成唯一订单号流水
                    delivery_Orders.Order_Type = "ZJCKD";
                    delivery_Orders.Material_Code = item.Material_Code;
                    delivery_Orders.Material_Type = item.Material_Type;
                    delivery_Orders.Location_Id = item.Location_Id;
                    delivery_Orders.Location_Code = item.Location_Code;
                    delivery_Orders.Port_Id = port.Id;
                    delivery_Orders.Port_Code = port.Code;
                    delivery_Orders.Status = (int)DataStatusEnum.Normal;
                    delivery_Orders.Delivery_Step = (int)DeliveryOrderStatusEnum.WaitingForOutbound;
                    delivery_Orders.Create_Time = DateTime.Now;
                    delivery_Orders.Creator = currentUserId;

                    //创建出库单 锁定库存
                    item.Is_Lock = "Y";
                    item.Updator = currentUserId;
                    item.Update_Time = DateTime.Now;

                    list.Add(delivery_Orders);
                }
                List<string> orders = list.Select(m => m.Order_No).ToList();
                //批量创建出库单
                await _db.BulkInsertAsync(list);
                await _db.SaveChangesAsync();
                //批量创建出库任务
                return await CreateTaskAsync(list, currentUserId);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids删库出库单信息
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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
                            throw new Exception("The delivery_Orders id parameter is empty");
                        }
                        var delivery_Orders = await _db.Delivery_Orders.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (delivery_Orders == null)
                        {
                            throw new Exception($"No information found for delivery_Orders,id is {id}");
                        }
                        //若该出库单已生成出库任务 且出库任务未执行结束 不允许删除
                        var task = await _db.WMS_Tasks.Where(m => m.Order_No.Equals(delivery_Orders.Order_No) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (task != null)
                        {
                            throw new Exception("The inventory order has already been generated with a task and cannot be deleted");
                        }
                        //获取被锁定的库存 解锁库存状态
                        var  inventory= await _db.Inventories.Where(m => m.Material_Code.Equals(delivery_Orders.Material_Code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (inventory == null || inventory.Is_Lock.Equals("N"))
                        {
                            throw new Exception("Inventory does not exist or inventory status is problematic");
                        }

                        inventory.Is_Lock = "N";
                        inventory.Update_Time = DateTime.Now;
                        inventory.Updator = currentUserId;

                        delivery_Orders.Status = (int)DataStatusEnum.Delete;
                        delivery_Orders.Update_Time = DateTime.Now;
                        delivery_Orders.Updator = currentUserId;
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
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO)
        {
            try
            {
                var items = _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO .Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(deliveryOrderParamsDTO .Material_Code));
                }
                if (deliveryOrderParamsDTO .Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == deliveryOrderParamsDTO .Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO .Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(deliveryOrderParamsDTO .Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO .Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(deliveryOrderParamsDTO .Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO .Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(deliveryOrderParamsDTO .Order_No));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO .Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(deliveryOrderParamsDTO .Order_Type));
                }
                if (deliveryOrderParamsDTO .Create_Time_Strat != null && deliveryOrderParamsDTO .Create_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= deliveryOrderParamsDTO .Create_Time_Strat);
                }
                if (deliveryOrderParamsDTO .Create_Time_End != null && deliveryOrderParamsDTO .Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= deliveryOrderParamsDTO .Create_Time_End);
                }
                if (deliveryOrderParamsDTO .Delivery_Time_Strat != null && deliveryOrderParamsDTO .Delivery_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Delivery_Time >= deliveryOrderParamsDTO .Delivery_Time_Strat);
                }
                if (deliveryOrderParamsDTO .Delivery_Time_End != null && deliveryOrderParamsDTO .Delivery_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Delivery_Time <= deliveryOrderParamsDTO .Delivery_Time_End);
                }
                if (deliveryOrderParamsDTO .Delivery_Step > 0)
                {
                    items = items.Where(m => m.Delivery_Step == deliveryOrderParamsDTO .Delivery_Step);
                }
                var result = items.Adapt<List<DeliveryOrderExportTemplate>>();
                return await MiniExcelUtil.ExportAsync("DeliveryOrderInfomation", result);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据出库单编码查询出库单信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<WMS_Delivery_Orders> GetDeliveryOrderByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The delivery_Orders code parameter is empty");
                }
                var delivery_Orders = await _db.Delivery_Orders.Where(m => m.Order_No.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (delivery_Orders == null)
                {
                    throw new Exception($"No information found for delivery_Orders,code is {code}");
                }
                return delivery_Orders;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id查询出库单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<WMS_Delivery_Orders> GetDeliveryOrderByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The delivery_Orders id parameter is empty");
                }
                var delivery_Orders = await _db.Delivery_Orders.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (delivery_Orders == null)
                {
                    throw new Exception($"No information found for delivery_Orders,id is {id}");
                }
                return delivery_Orders;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询出库单信息
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<WMS_Delivery_Orders>> GetListAsync(DeliveryOrderParamsDTO deliveryOrderParamsDTO)
        {
            try
            {
                var items = _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(deliveryOrderParamsDTO.Material_Code));
                }
                if (deliveryOrderParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == deliveryOrderParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(deliveryOrderParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(deliveryOrderParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(deliveryOrderParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(deliveryOrderParamsDTO.Order_Type));
                }
                if (deliveryOrderParamsDTO.Create_Time_Strat != null && deliveryOrderParamsDTO.Create_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= deliveryOrderParamsDTO.Create_Time_Strat);
                }
                if (deliveryOrderParamsDTO.Create_Time_End != null && deliveryOrderParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= deliveryOrderParamsDTO.Create_Time_End);
                }
                if (deliveryOrderParamsDTO.Delivery_Time_Strat != null && deliveryOrderParamsDTO.Delivery_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Delivery_Time >= deliveryOrderParamsDTO.Delivery_Time_Strat);
                }
                if (deliveryOrderParamsDTO.Delivery_Time_End != null && deliveryOrderParamsDTO.Delivery_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Delivery_Time <= deliveryOrderParamsDTO.Delivery_Time_End);
                }
                if (deliveryOrderParamsDTO.Delivery_Step > 0)
                {
                    items = items.Where(m => m.Delivery_Step == deliveryOrderParamsDTO.Delivery_Step);
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
        /// 分页查询出库单信息
        /// </summary>
        /// <param name="deliveryOrderParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<BasePagination<WMS_Delivery_Orders>> GetPaginationAsync([FromQuery] DeliveryOrderParamsDTO deliveryOrderParamsDTO)
        {
            try
            {
                var items = _db.Delivery_Orders.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(deliveryOrderParamsDTO.Material_Code));
                }
                if (deliveryOrderParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == deliveryOrderParamsDTO.Material_Type);
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(deliveryOrderParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(deliveryOrderParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(deliveryOrderParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(deliveryOrderParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(deliveryOrderParamsDTO.Order_Type));
                }
                if (deliveryOrderParamsDTO.Create_Time_Strat != null && deliveryOrderParamsDTO.Create_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= deliveryOrderParamsDTO.Create_Time_Strat);
                }
                if (deliveryOrderParamsDTO.Create_Time_End != null && deliveryOrderParamsDTO.Create_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= deliveryOrderParamsDTO.Create_Time_End);
                }
                if (deliveryOrderParamsDTO.Delivery_Time_Strat != null && deliveryOrderParamsDTO.Delivery_Time_Strat != DateTime.MinValue)
                {
                    items = items.Where(m => m.Delivery_Time >= deliveryOrderParamsDTO.Delivery_Time_Strat);
                }
                if (deliveryOrderParamsDTO.Delivery_Time_End != null && deliveryOrderParamsDTO.Delivery_Time_End != DateTime.MinValue)
                {
                    items = items.Where(m => m.Delivery_Time <= deliveryOrderParamsDTO.Delivery_Time_End);
                }
                if (deliveryOrderParamsDTO.Delivery_Step > 0)
                {
                    items = items.Where(m => m.Delivery_Step == deliveryOrderParamsDTO.Delivery_Step);
                }
                return await PaginationService.PaginateAsync(items, deliveryOrderParamsDTO.PageIndex, deliveryOrderParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id判断出库单是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsExistAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The delivery_Orders id parameter is empty");
                }
                var delivery_Orders = await _db.Delivery_Orders.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (delivery_Orders == null)
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
        public async Task<string> RegenerateTaskAsync(List<long> ids, long currentUserId)
        {
            try
            {
                if(ids ==null || ids.Count <=0)
                {
                    throw new Exception("The ids  parameter cannot be empty");
                }  
                var delivery_Orders = await _db.Delivery_Orders.Where(m => ids.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (delivery_Orders == null || delivery_Orders.Count < ids.Count)
                {
                    throw new Exception("Query results do not match");
                }
                if (delivery_Orders.Any(m => m.Delivery_Step == (int)DeliveryOrderStatusEnum.Outbound))
                {
                    throw new Exception("There are outbound tasks that have been completed");
                }
                //判断该出库单是否存在出库任务
                var orders = delivery_Orders.Select(m => m.Order_No).ToList();
                var tasks = await _db.WMS_Tasks.Where(m => orders.Contains(m.Order_No) && m.Status == (int)DataStatusEnum.Normal).CountAsync();
                if (tasks > 0)
                {
                    throw new Exception("There is an outbound task, resending is not allowed");
                }
                await CreateTaskAsync(delivery_Orders, currentUserId);
                return "Regenerate Tasks Success";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据出库单生成出库任务
        /// </summary>
        /// <param name="delivery_Orders"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateTaskAsync(List<WMS_Delivery_Orders> delivery_Orders, long currentUserId)
        {
            try
            {
                if (delivery_Orders == null || delivery_Orders.Count <= 0)
                {
                    throw new Exception("The delivery_Orders  parameter cannot be empty");
                }
                List<WMS_Task> tasks = new List<WMS_Task>();
                var taskCount = await _db.WMS_Tasks.CountAsync();
                foreach (var delivery_Order in delivery_Orders)
                {
                    WMS_Task task = new WMS_Task();
                    task.Task_Priority = 9;
                    var port = await _portService.GetPortByIdAsync(delivery_Order.Port_Id);
                    var location = await _locationService.GetLocationByIdAsync(delivery_Order.Location_Id);
                    // 判断出库口巷道是否在存储位置上的本巷道 不在则穿越出库模式
                    if (port.First_Lanway != location.Lanway)
                    {
                        task.Task_Mode = (int)TaskModeEnum.AcrossDelivery;
                    }
                    else
                    {
                        task.Task_Mode = (int)TaskModeEnum.NormalDelivery;
                    }
                    task.Task_Lanway = location.Lanway;
                    task.Source_Position_X = location.Location_Row;
                    task.Source_Position_Y = location.Location_Column;
                    task.Source_Position_Z = location.Location_Layer;
                    task.Location_Id = location.Id;
                    task.Location_Code = location.Code;
                    task.Material_Code = delivery_Order.Material_Code;
                    task.Material_Type = delivery_Order.Material_Type;
                    task.Order_No = delivery_Order.Order_No;
                    task.Port_Id = port.Id;
                    task.Port_Code = port.Code;
                    task.Status = (int)DataStatusEnum.Normal;
                    task.Create_Time = DateTime.Now;
                    task.Creator = currentUserId;
                    if (delivery_Orders.IndexOf(delivery_Order) + taskCount > Int16.MaxValue)
                    {
                        task.Task_No = ((delivery_Orders.IndexOf(delivery_Order) + 1 + taskCount) % Int16.MaxValue).ToString();
                        task.Task_No_Backup = task.Task_No;

                    }
                    else
                    {
                        task.Task_No = (delivery_Orders.IndexOf(delivery_Order) + 1 + taskCount).ToString();
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
