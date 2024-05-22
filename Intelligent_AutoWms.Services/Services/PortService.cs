using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Port;
using Intelligent_AutoWms.Model.RequestDTO.Port;
using Intelligent_AutoWms.Model.ResponseDTO.Port;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class PortService : IPortService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<PortService> _log;

        public PortService(Intelligent_AutoWms_DbContext db,ILogger<PortService> logger)
        {
            _db = db;
            _log = logger;
        }

        /// <summary>
        /// 创建出入口
        /// </summary>
        /// <param name="createPortDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateAsync([FromBody] CreateOrUpdatePortDTO createPortDTO, long currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createPortDTO.Code))
                {
                    throw new Exception("The port code parameter is empty");
                }
                if (string.IsNullOrWhiteSpace(createPortDTO.Name))
                {
                    throw new Exception("The port name parameter is empty");
                }
                if (createPortDTO.Type <= 0)
                {
                    throw new Exception("The port  type is empty");
                }
                if (createPortDTO.Warehouse_Id == null || createPortDTO.Warehouse_Id <= 0)
                {
                    throw new Exception("The port warehouseId  is empty");
                }
                var warehouse = await _db.WareHouses.Where(m => m.Id == createPortDTO.Warehouse_Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (warehouse == null)
                {
                    throw new Exception($"No information found for wareHouse,wareHouseId is {createPortDTO.Warehouse_Id}");
                }
                if (await IsExistAsync(createPortDTO.Code))
                {
                    throw new Exception("The port already exists");
                }
                WMS_Port port = new WMS_Port();
                port.Code = createPortDTO.Code;
                port.Name = createPortDTO.Name;
                port.Type = createPortDTO.Type;
                port.Status = (int)DataStatusEnum.Normal;
                port.Warehouse_Id = (long)createPortDTO.Warehouse_Id;
                port.Remark = createPortDTO.Remark;
                port.First_Lanway = createPortDTO.First_Lanway;
                port.Second_Lanway = createPortDTO.Second_Lanway;
                port.Third_Lanway = createPortDTO.Third_Lanway;
                port.Forth_Lanway = createPortDTO.Forth_Lanway;
                port.Fifth_Lanway = createPortDTO.Fifth_Lanway;
                port.Sixth_Lanway = createPortDTO.Sixth_Lanway;
                port.Create_Time = DateTime.Now;
                port.Creator = currentUserId;
                var result = await _db.Ports.AddAsync(port);
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
        /// 根据ids删除出入口信息
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
                            throw new Exception("The port id parameter is empty");
                        }
                        var port = await _db.Ports.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (port == null)
                        {
                            throw new Exception($"No information found for port,id is {id}");
                        }
                        if (await _db.Receipt_Orders.AnyAsync(m => m.Port_Id == id && m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step ==(int)ReceiptOrderStatusEnum.WaitingForStorage))
                        {
                            throw new Exception("The port is in use and cannot be deleted");
                        }
                        if (await _db.Delivery_Orders.AnyAsync(m => m.Port_Id == id && m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound))
                        {
                            throw new Exception("The port is in use and cannot be deleted");
                        }
                        if (await _db.Inventories.AnyAsync(m => m.Port_Id == id && m.Status == (int)DataStatusEnum.Normal))
                        {
                            throw new Exception("The port is in use and cannot be deleted");
                        }
                        if (await _db.WMS_Tasks.AnyAsync(m => m.Port_Id == id && m.Status == (int)DataStatusEnum.Normal))
                        {
                            throw new Exception("The port is in use and cannot be deleted");
                        }
                        port.Status = (int)DataStatusEnum.Delete;
                        port.Update_Time = DateTime.Now;
                        port.Updator = currentUserId;
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
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            try
            {
                List<PortDownloadTemplate> list = new List<PortDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("Port_Download_Template", list);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            try
            {
                List<PortExportTemplate> list = new List<PortExportTemplate>();
                var items = _db.Ports.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(portParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(portParamsDTO.Name));
                }
                if (portParamsDTO.Type != null && portParamsDTO.Type > 0)
                {
                    items = items.Where(m => m.Type.Equals(portParamsDTO.Type));
                }
                //获取所有仓库id 
                var warehouseIds = await items.Select(m => m.Warehouse_Id).Distinct().ToListAsync();
                if (warehouseIds != null && warehouseIds.Count > 0)
                {
                    var warehouseItems = await _db.WareHouses.Where(m => warehouseIds.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                    if (warehouseItems != null && warehouseItems.Count == warehouseIds.Count)
                    {
                        var data = await items.Join(warehouseItems, i => i.Warehouse_Id, o => o.Id, (i, o) => new { i, o }).Select(m => new PortExportTemplate
                        {
                            Id = m.i.Id,
                            Name = m.i.Name,
                            Code = m.i.Code,
                            Warehouse_Code = m.o.Code,
                            Type = m.i.Type,
                            First_Lanway = m.i.First_Lanway,
                            Second_Lanway = m.i.Second_Lanway,
                            Third_Lanway = m.i.Third_Lanway,
                            Forth_Lanway = m.i.Forth_Lanway,
                            Fifth_Lanway = m.i.Fifth_Lanway,
                            Sixth_Lanway = m.i.Sixth_Lanway,
                            Remark = m.i.Remark,
                            Create_Time = m.i.Create_Time
                        }).ToListAsync();
                        list = data;
                    }
                    else
                    {
                        throw new Exception("There is an issue with the warehouse status and it does not match");
                    }
                }
                return await MiniExcelUtil.ExportAsync("PortInfomation", list);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询出入口信息
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Port>> GetListAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            try
            {
                var items = _db.Ports.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(portParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(portParamsDTO.Name));
                }
                if (portParamsDTO.Type != null && portParamsDTO.Type > 0)
                {
                    items = items.Where(m => m.Type.Equals(portParamsDTO.Type));
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
        /// 查询出入口信息分页
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Port>> GetPaginationAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            try
            {
                var items = _db.Ports.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(portParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(portParamsDTO.Name));
                }
                if (portParamsDTO.Type != null && portParamsDTO.Type > 0)
                {
                    items = items.Where(m => m.Type.Equals(portParamsDTO.Type));
                }
                return await PaginationService.PaginateAsync(items,portParamsDTO.PageIndex,portParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据code获取出入口信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Port> GetPortByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The port code parameter is empty");
                }
                var port = await _db.Ports.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (port == null)
                {
                    throw new Exception($"No information found for port,code is {code}");
                }
                return port;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes获取出入口信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Port>> GetPortByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_Port>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.Ports.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 根据id获取出入口信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Port> GetPortByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The port id parameter is empty");
                }
                var port = await _db.Ports.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (port == null)
                {
                    throw new Exception($"No information found for port,id is {id}");
                }
                return port;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids获取出入口信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Port>> GetPortByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_Port>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.Ports.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 获取出入口选项集
        /// </summary>
        /// <param name="portParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<PortOptions>> GetPortOptionsAsync([FromQuery] PortParamsDTO portParamsDTO)
        {
            try
            {
                List<PortOptions> portOptions = new List<PortOptions>();
                var items = _db.Ports.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(portParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(portParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(portParamsDTO.Name));
                }
                if (portParamsDTO.Type != null && portParamsDTO.Type > 0)
                {
                    items = items.Where(m => m.Type.Equals(portParamsDTO.Type));
                }
                var result = await items.ToListAsync();
                portOptions = result.Adapt<List<PortOptions>>();
                return portOptions;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ImportAsync(string path, long currentUserId)
        {
            try
            {
                var result = MiniExcelUtil.Import<PortDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断出入口编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported port code");
                }
                //判断出入口名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported port name");
                }
                //判断库区编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Warehouse_Code)))
                {
                    throw new Exception("There is a null value in the imported port WarehouseCode");
                }
                //判断出入口类型有没有空值
                if (result.Any(m => m.Type <=0))
                {
                    throw new Exception("There is a null value in the imported port type");
                }
                //判断货架编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("port code duplication");
                }
                var portCodeList = result.Select(m => m.Code).ToList();
                var portItems = await _db.Ports.Where(m => portCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (portItems != null && portItems.Count > 0)
                {
                    throw new Exception("port code already exists");
                }
                //获取所有仓库编码
                var warehouseCodes = result.Select(m => m.Warehouse_Code).Distinct().ToList();
                var warehouseItems = await _db.WareHouses.Where(m => warehouseCodes.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                if (warehouseItems != null && warehouseItems.Count == warehouseCodes.Count)
                {
                    var data = result.Join(warehouseItems, i => i.Warehouse_Code, o => o.Code, (i, o) => new { i, o }).Select(m => new WMS_Port
                    {
                        Name = m.i.Name,
                        Code = m.i.Code,
                        Type = m.i.Type,
                        Warehouse_Id = m.o.Id,
                        First_Lanway = m.i.First_Lanway,
                        Second_Lanway = m.i.Second_Lanway,
                        Third_Lanway = m.i.Third_Lanway,
                        Forth_Lanway = m.i.Forth_Lanway,
                        Fifth_Lanway = m.i.Fifth_Lanway,
                        Sixth_Lanway = m.i.Sixth_Lanway,
                        Status = (int)DataStatusEnum.Normal,
                        Remark = m.i.Remark,
                        Create_Time = DateTime.Now,
                        Creator = currentUserId,
                    });
                    await _db.BulkInsertAsync(data);
                    await _db.SaveChangesAsync();
                    return "Import Port successful";
                }
                else
                {
                    throw new Exception("There is an issue with the warehouse status and it does not match");
                }


            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据出入口编码判断该出入口是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> IsExistAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The port code parameter is empty");
                }
                var port = await _db.Ports.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (port == null)
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
        /// 更新出入口信息
        /// </summary>
        /// <param name="updatePortDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> UpdateAsync([FromBody] CreateOrUpdatePortDTO updatePortDTO, long currentUserId)
        {
            try
            {
                if (updatePortDTO.Id <= 0)
                {
                    throw new Exception("Port ID does not exist");
                }
                var port = await _db.Ports.Where(m => m.Id == updatePortDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (port == null)
                {
                    throw new Exception($"No information found for port,portId is {updatePortDTO.Id}");
                }
                if (await _db.Receipt_Orders.AnyAsync(m => m.Port_Id == updatePortDTO.Id && m.Status == (int)DataStatusEnum.Normal && m.Receipt_Step == (int)ReceiptOrderStatusEnum.WaitingForStorage))
                {
                    throw new Exception("The port is in use and cannot be modify");
                }
                if (await _db.Delivery_Orders.AnyAsync(m => m.Port_Id == updatePortDTO.Id && m.Status == (int)DataStatusEnum.Normal && m.Delivery_Step == (int)DeliveryOrderStatusEnum.WaitingForOutbound))
                {
                    throw new Exception("The port is in use and cannot be modify");
                }
                if (await _db.Inventories.AnyAsync(m => m.Port_Id == updatePortDTO.Id && m.Status == (int)DataStatusEnum.Normal))
                {
                    throw new Exception("The port is in use and cannot be modify");
                }
                if (await _db.WMS_Tasks.AnyAsync(m => m.Port_Id == updatePortDTO.Id && m.Status == (int)DataStatusEnum.Normal))
                {
                    throw new Exception("The port is in use and cannot be modify");
                }
                if (!string.IsNullOrWhiteSpace(updatePortDTO.Code))
                {
                    if (await IsExistAsync(updatePortDTO.Code))
                    {
                        throw new Exception($"Port code {updatePortDTO.Code} already exists, duplicate creation is not allowed");
                    }
                    port.Code = updatePortDTO.Code;
                }
                if (!string.IsNullOrWhiteSpace(updatePortDTO.Name))
                {
                    port.Name = updatePortDTO.Name;
                }
                if (updatePortDTO.Type > 0)
                {
                    port.Type = updatePortDTO.Type;
                }
                if (updatePortDTO.First_Lanway > 0)
                {
                    port.First_Lanway = updatePortDTO.First_Lanway;
                }
                if (updatePortDTO.Second_Lanway > 0)
                {
                    port.Second_Lanway = updatePortDTO.Second_Lanway;
                }
                if (updatePortDTO.Third_Lanway > 0)
                {
                    port.Third_Lanway = updatePortDTO.Third_Lanway;
                }
                if (updatePortDTO.Forth_Lanway > 0)
                {
                    port.Forth_Lanway = updatePortDTO.Forth_Lanway;
                }
                if (updatePortDTO.Fifth_Lanway > 0)
                {
                    port.Fifth_Lanway = updatePortDTO.Fifth_Lanway;
                }
                if (updatePortDTO.Sixth_Lanway > 0)
                {
                    port.Sixth_Lanway = updatePortDTO.Sixth_Lanway;
                }
                port.Update_Time = DateTime.Now;
                port.Updator = currentUserId;
                port.Remark = string.IsNullOrWhiteSpace(updatePortDTO.Remark) ? port.Remark : updatePortDTO.Remark;
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
