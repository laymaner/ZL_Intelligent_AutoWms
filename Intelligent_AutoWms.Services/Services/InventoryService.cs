using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Inventory;
using Intelligent_AutoWms.Model.RequestDTO.Inventory;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class InventoryService:IInventoryService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<InventoryService> _log;
        private readonly ILocationService _locationService;
        private readonly IPortService _portService;
        private readonly IReceiptOrderService _receiptOrderService;

        public InventoryService(Intelligent_AutoWms_DbContext db, ILogger<InventoryService> log, ILocationService locationService,IPortService portService,IReceiptOrderService receiptOrderService)
        {
            _db = db; 
            _log = log;
            _locationService = locationService;
            _portService = portService;
            _receiptOrderService = receiptOrderService;
        }

        /// <summary>
        /// 创建库存明细
        /// </summary>
        /// <param name="createInventoryDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<long> CreateAsync([FromBody] CreateInventoryDTO createInventoryDTO, long currentUserId)
        {
            try
            {
                //判断参数是否符合
                if (string.IsNullOrWhiteSpace(createInventoryDTO.Material_Code))
                {
                    throw new Exception("The material code parameter cannot be empty");
                }
                if (createInventoryDTO.Material_Type <= 0)
                {
                    throw new Exception("The material type parameter cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createInventoryDTO.Location_Code))
                {
                    throw new Exception("The location code parameter cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createInventoryDTO.Port_Code))
                {
                    throw new Exception("The port code parameter cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createInventoryDTO.Order_No))
                {
                    throw new Exception("The order code parameter cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createInventoryDTO.Order_Type))
                {
                    throw new Exception("The order type parameter cannot be empty");
                }
                //判断物料 货位 入口 订单
                var inventory = await _db.Inventories.Where(m => m.Material_Code.Equals(createInventoryDTO.Material_Code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (inventory != null)
                {
                    throw new Exception("The goods are already in stock and cannot be added repeatedly");
                }
                var location =await _locationService.GetLocationByCodeAsync(createInventoryDTO.Location_Code);
                var port = await _portService.GetPortByCodeAsync(createInventoryDTO.Port_Code);
                await _receiptOrderService.GetReceiptOrderByCodeAsync(createInventoryDTO.Order_No);
                WMS_Inventory materialStock = new WMS_Inventory();
                materialStock.Order_No = createInventoryDTO.Order_No;
                materialStock.Order_Type = createInventoryDTO.Order_Type;
                materialStock.Location_Id = location.Id;
                materialStock.Location_Code = createInventoryDTO.Location_Code;
                materialStock.Port_Id = port.Id;
                materialStock.Port_Code = createInventoryDTO.Port_Code;
                materialStock.Is_Lock = "N";
                materialStock.Create_Time = DateTime.Now;
                materialStock.Creator = currentUserId;
                materialStock.Status = (int)DataStatusEnum.Normal;
                materialStock.Remark = createInventoryDTO.Remark;
                materialStock.Material_Code = createInventoryDTO.Material_Code;
                materialStock.Material_Type = createInventoryDTO.Material_Type;

                //创建库存明细 将货位存储位置改为占用状态
                location.Step = (int)LocationStatusEnum.Occupying;
                location.Update_Time = DateTime.Now;
                location.Updator = currentUserId;
                var result = await _db.Inventories.AddAsync(materialStock);
                await _db.SaveChangesAsync();
                return result.Entity.Id;
            }
            catch (Exception ex) 
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids删除库存明细信息
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
                            throw new Exception("The Inventory id parameter is empty");
                        }
                        var inventory = await _db.Inventories.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (inventory == null)
                        {
                            throw new Exception($"No information found for inventory,id is {id}");
                        }
                        if (inventory.Is_Lock.Equals("Y"))
                        {
                            throw new Exception("This inventory is locked and cannot be deleted");
                        }
                        var loaction = await _locationService.GetLocationByCodeAsync(inventory.Location_Code);

                        inventory.Status = (int)DataStatusEnum.Delete;
                        inventory.Update_Time = DateTime.Now;
                        inventory.Updator = currentUserId;

                        //删除库存明细 将货位存储位置改为空闲状态
                        loaction.Step = (int)LocationStatusEnum.Idle;
                        loaction.Update_Time = DateTime.Now;
                        loaction.Updator = currentUserId;
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
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO)
        {
            try
            {
                var items = _db.Inventories.AsNoTracking();
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(inventoryParamsDTO.Material_Code));
                }
                if (inventoryParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == inventoryParamsDTO.Material_Type);
                }
                if (inventoryParamsDTO.Status > 0)
                {
                    items = items.Where(m => m.Status == inventoryParamsDTO.Status);
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(inventoryParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(inventoryParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(inventoryParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(inventoryParamsDTO.Order_Type));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Is_Lock))
                {
                    items = items.Where(m => m.Is_Lock.Equals(inventoryParamsDTO.Is_Lock));
                }
                if (inventoryParamsDTO.Start_Time != null && inventoryParamsDTO.Start_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= inventoryParamsDTO.Start_Time);
                }
                if (inventoryParamsDTO.End_Time != null && inventoryParamsDTO.End_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= inventoryParamsDTO.End_Time);
                }
                var result = items.Adapt<List<InventoryExportTemplate>>();
                return await MiniExcelUtil.ExportAsync("InventoryInfomation",result);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id获取库存明细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Inventory> GetInventoryByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The inventory id parameter is empty");
                }
                var inventory = await _db.Inventories.Where(m => m.Id == id).SingleOrDefaultAsync();
                if (inventory == null)
                {
                    throw new Exception($"No information found for inventory,id is {id}");
                }
                return inventory;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取库存明细信息
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Inventory>> GetListAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO)
        {
            try
            {
                var items = _db.Inventories.AsNoTracking();
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(inventoryParamsDTO.Material_Code));
                }
                if (inventoryParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == inventoryParamsDTO.Material_Type);
                }
                if (inventoryParamsDTO.Status > 0)
                {
                    items = items.Where(m => m.Status == inventoryParamsDTO.Status);
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(inventoryParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(inventoryParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(inventoryParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(inventoryParamsDTO.Order_Type));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Is_Lock))
                {
                    items = items.Where(m => m.Is_Lock.Equals(inventoryParamsDTO.Is_Lock));
                }
                if (inventoryParamsDTO.Start_Time != null && inventoryParamsDTO.Start_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= inventoryParamsDTO.Start_Time);
                }
                if (inventoryParamsDTO.End_Time != null && inventoryParamsDTO.End_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= inventoryParamsDTO.End_Time);
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
        /// 分页查询库存明细信息
        /// </summary>
        /// <param name="inventoryParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Inventory>> GetPaginationAsync([FromQuery] InventoryParamsDTO inventoryParamsDTO)
        {
            try
            {
                var items = _db.Inventories.OrderByDescending(n => n.Id).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Material_Code))
                {
                    items = items.Where(m => m.Material_Code.StartsWith(inventoryParamsDTO.Material_Code));
                }
                if (inventoryParamsDTO.Material_Type > 0)
                {
                    items = items.Where(m => m.Material_Type == inventoryParamsDTO.Material_Type);
                }
                if (inventoryParamsDTO.Status > 0)
                {
                    items = items.Where(m => m.Status == inventoryParamsDTO.Status);
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Location_Code))
                {
                    items = items.Where(m => m.Location_Code.StartsWith(inventoryParamsDTO.Location_Code));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Port_Code))
                {
                    items = items.Where(m => m.Port_Code.StartsWith(inventoryParamsDTO.Port_Code));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Order_No))
                {
                    items = items.Where(m => m.Order_No.StartsWith(inventoryParamsDTO.Order_No));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Order_Type))
                {
                    items = items.Where(m => m.Order_Type.Equals(inventoryParamsDTO.Order_Type));
                }
                if (!string.IsNullOrWhiteSpace(inventoryParamsDTO.Is_Lock))
                {
                    items = items.Where(m => m.Is_Lock.Equals(inventoryParamsDTO.Is_Lock));
                }
                if (inventoryParamsDTO.Start_Time != null && inventoryParamsDTO.Start_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time >= inventoryParamsDTO.Start_Time);
                }
                if (inventoryParamsDTO.End_Time != null && inventoryParamsDTO.End_Time != DateTime.MinValue)
                {
                    items = items.Where(m => m.Create_Time <= inventoryParamsDTO.End_Time);
                }
                return await PaginationService.PaginateAsync(items,inventoryParamsDTO.PageIndex,inventoryParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id判断该库存是否存在
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
                    throw new Exception("The inventory id parameter is empty");
                }
                var inventory = await _db.Inventories.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (inventory == null)
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
        /// 根据ids锁定库存信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> LockInventoryAsync(List<long> ids,long currentUserId)
        {
            try
            {
                if (ids == null || ids.Count <= 0)
                {
                    throw new Exception("The inventory ids parameter is empty");
                }
                foreach (var id in ids)
                {
                    if (id <= 0)
                    {
                        throw new Exception("The Inventory id parameter is empty");
                    }
                    var inventory = await _db.Inventories.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                    if (inventory == null)
                    {
                        throw new Exception($"No information found for inventory,id is {id}");
                    }
                    if (inventory.Is_Lock.Equals("Y"))
                    {
                        throw new Exception("The inventory has been locked, no need to lock it again");
                    }
                    inventory.Is_Lock = "Y";
                    inventory.Update_Time = DateTime.Now;
                    inventory.Updator = currentUserId;
                }
                await _db.SaveChangesAsync();
                return "Locked Success";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids解锁库存信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> UnLockInventoryAsync(List<long> ids, long currentUserId)
        {
            try
            {
                if (ids == null || ids.Count <= 0)
                {
                    throw new Exception("The inventory ids parameter is empty");
                }
                foreach (var id in ids)
                {
                    if (id <= 0)
                    {
                        throw new Exception("The Inventory id parameter is empty");
                    }
                    var inventory = await _db.Inventories.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                    if (inventory == null)
                    {
                        throw new Exception($"No information found for inventory,id is {id}");
                    }
                    if (inventory.Is_Lock.Equals("N"))
                    {
                        throw new Exception("This inventory is not locked and does not need to be unlocked");
                    }
                    var flag = _db.Delivery_Orders.Any(m => m.Material_Code == inventory.Material_Code && m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound);
                    if (flag)
                    {
                        throw new Exception("The inventory has generated an outbound order and cannot be unlocked");
                    }
                    inventory.Is_Lock = "N";
                    inventory.Update_Time = DateTime.Now;
                    inventory.Updator = currentUserId;
                }
                await _db.SaveChangesAsync();
                return "UnLocked Success";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
