using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Area;
using Intelligent_AutoWms.Model.RequestDTO.Area;
using Intelligent_AutoWms.Model.ResponseDTO.Area;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;

namespace Intelligent_AutoWms.Services.Services
{
    public class AreaService : IAreaService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<AreaService> _log;
        private readonly IWareHouseService _iwarehouseService;

        public AreaService(Intelligent_AutoWms_DbContext db, ILogger<AreaService> logger, IWareHouseService wareHouseService)
        {
            _db = db;
            _log = logger;
            _iwarehouseService = wareHouseService;
        }

        /// <summary>
        /// 创建库区
        /// </summary>
        /// <param name="createAreaDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> CreateAsync([FromBody] CreateOrUpdateAreaDTO createAreaDTO, long currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createAreaDTO.Code))
                {
                    throw new Exception("The area code parameter is empty");
                }
                if (string.IsNullOrWhiteSpace(createAreaDTO.Name))
                {
                    throw new Exception("The area name parameter is empty");
                }
                if (createAreaDTO.Warehouse_Id == null || createAreaDTO.Warehouse_Id <= 0)
                {
                    throw new Exception("The warehouse id parameter is empty");
                }
                if (await IsExistAsync(createAreaDTO.Code))
                {
                    throw new Exception("The area already exists");
                }
                var warehouse = await _db.WareHouses.Where(m => m.Id == createAreaDTO.Warehouse_Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (warehouse == null)
                {
                    throw new Exception($"No information found for warehouse,id is {createAreaDTO.Warehouse_Id}");
                }
                WMS_Area area = new WMS_Area();
                area.Code = createAreaDTO.Code;
                area.Name = createAreaDTO.Name;
                area.Remark = createAreaDTO.Remark;
                area.Warehouse_Id = (long)createAreaDTO.Warehouse_Id;
                area.Creator = currentUserId;
                area.Status = (int)DataStatusEnum.Normal;
                area.Create_Time = DateTime.Now;
                var result = await _db.Areas.AddAsync(area);
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
        /// 根据ids删除库区
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
                            throw new Exception("The area id parameter is empty");
                        }
                        var area = await _db.Areas.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (area == null)
                        {
                            throw new Exception($"No information found for area,id is {id}");
                        }
                        if (await _db.Shelves.AnyAsync(m => m.Area_Id == id && m.Status == (int)DataStatusEnum.Normal))
                        {
                            throw new Exception("The area is in use and cannot be deleted");
                        }
                        area.Status = (int)DataStatusEnum.Delete;
                        area.Update_Time = DateTime.Now;
                        area.Updator = currentUserId;
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
                List<AreaDownloadTemplate> list = new List<AreaDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("Area_Download_Template", list);
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
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            try
            {
                List<AreaExportTemplate> list = new List<AreaExportTemplate>();
                var items = _db.Areas.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(areaParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(areaParamsDTO.Name));
                }
                if (areaParamsDTO.WarehouseId != null && areaParamsDTO.WarehouseId > 0)
                {
                    items = items.Where(m => m.Warehouse_Id == areaParamsDTO.WarehouseId);
                }
                //获取所有仓库id 
                var warehouseIds = await items.Select(m => m.Warehouse_Id).Distinct().ToListAsync();
                var result = await items.ToListAsync();
                if (warehouseIds != null && warehouseIds.Count > 0)
                {
                    var warehouseItems = await _db.WareHouses.Where(m => warehouseIds.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                    if (warehouseItems != null && warehouseItems.Count == warehouseIds.Count)
                    {
                        var data =result.Join(warehouseItems, i => i.Warehouse_Id, o => o.Id, (i, o) => new { i, o }).Select(m => new AreaExportTemplate
                        {
                            Id = m.i.Id,
                            Name = m.i.Name,
                            Code = m.i.Code,
                            WareHouse_Code = m.o.Code,
                            Remark = m.i.Remark,
                            Create_Time = m.i.Create_Time
                        }).ToList();
                        list = data;
                    }
                    else
                    {
                        throw new Exception("There is an issue with the warehouse status and it does not match");
                    }
                }
                return await MiniExcelUtil.ExportAsync("AreaInfomation", list);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据库区编码获取库区信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Area> GetAreaByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The area code parameter is empty");
                }
                var area = await _db.Areas.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (area == null)
                {
                    throw new Exception($"No information found for area,code is {code}");
                }
                return area;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes集合获取库区信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Area>> GetAreaByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_Area>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.Areas.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 根据库区id获取库区信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Area> GetAreaByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The area id parameter is empty");
                }
                var area = await _db.Areas.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (area == null)
                {
                    throw new Exception($"No information found for area,id is {id}");
                }
                return area;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids获取库区信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Area>> GetAreaByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_Area>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.Areas.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 获取库区选项集
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<AreaOptions>> GetAreaOptionsAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            try
            {
                List<AreaOptions> areaOptions = new List<AreaOptions>();
                var items = _db.Areas.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(areaParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(areaParamsDTO.Name));
                }
                if (areaParamsDTO.WarehouseId != null && areaParamsDTO.WarehouseId > 0)
                {
                    items = items.Where(m => m.Warehouse_Id == areaParamsDTO.WarehouseId);
                }
                var result = await items.ToListAsync();
                areaOptions = result.Adapt<List<AreaOptions>>();
                return areaOptions;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取库区信息
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Area>> GetListAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            try
            {
                var items = _db.Areas.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(areaParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(areaParamsDTO.Name));
                }
                if (areaParamsDTO.WarehouseId != null && areaParamsDTO.WarehouseId > 0)
                {
                    items = items.Where(m => m.Warehouse_Id == areaParamsDTO.WarehouseId);
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
        /// 获取库区分页信息
        /// </summary>
        /// <param name="areaParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Area>> GetPaginationAsync([FromQuery] AreaParamsDTO areaParamsDTO)
        {
            try
            {
                var items = _db.Areas.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(areaParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(areaParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(areaParamsDTO.Name));
                }
                if (areaParamsDTO.WarehouseId != null && areaParamsDTO.WarehouseId > 0)
                {
                    items = items.Where(m => m.Warehouse_Id == areaParamsDTO.WarehouseId);
                }
                return await PaginationService.PaginateAsync(items, areaParamsDTO.PageIndex, areaParamsDTO.PageSize);
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
                var result = MiniExcelUtil.Import<AreaDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断库区编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported area code");
                }
                //判断库区名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported area name");
                }
                //判断仓库编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.WareHouse_Code)))
                {
                    throw new Exception("There is a null value in the imported area wareHouseCode");
                }
                //判断库区编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("area code duplication");
                }
                //判断库区是否存在
                var areaCodeList = result.Select(m => m.Code).ToList();
                var areaItems = await _db.Areas.Where(m => areaCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (areaItems != null && areaItems.Count > 0)
                {
                    throw new Exception("area code already exists");
                }

                //获取所有仓库编码
                var warehouseCodes = result.Select(m => m.WareHouse_Code).Distinct().ToList();
                var warehouseItems = await _db.WareHouses.Where(m => warehouseCodes.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                if (warehouseItems != null && warehouseItems.Count == warehouseCodes.Count)
                {
                    var data = result.Join(warehouseItems, i => i.WareHouse_Code, o => o.Code, (i, o) => new { i, o }).Select(m => new WMS_Area
                    {
                        Name = m.i.Name,
                        Code = m.i.Code,
                        Warehouse_Id = m.o.Id,
                        Status = (int)DataStatusEnum.Normal,
                        Remark = m.i.Remark,
                        Create_Time = DateTime.Now,
                        Creator = currentUserId,
                    });
                    await _db.BulkInsertAsync(data);
                    await _db.SaveChangesAsync();
                    return "Import Area successful";
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
        /// 导入-----excel导入
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ImportExcelAsync(IFormFile fileForm, long currentUserId)
        {
            try
            {
                if (!fileForm.FileName.Contains("Area_Download_Template"))
                {
                    throw new Exception("Please select the correct template to import");
                }
                var stream = fileForm.OpenReadStream();
                var result = stream.Query<AreaDownloadTemplate>().ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断库区编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported area code");
                }
                //判断库区名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported area name");
                }
                //判断仓库编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.WareHouse_Code)))
                {
                    throw new Exception("There is a null value in the imported area wareHouseCode");
                }
                //判断库区编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("area code duplication");
                }
                //判断库区是否存在
                var areaCodeList = result.Select(m => m.Code).ToList();
                var areaItems = await _db.Areas.Where(m => areaCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (areaItems != null && areaItems.Count > 0)
                {
                    throw new Exception("area code already exists");
                }

                //获取所有仓库编码
                var warehouseCodes = result.Select(m => m.WareHouse_Code).Distinct().ToList();
                var warehouseItems = await _db.WareHouses.Where(m => warehouseCodes.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                if (warehouseItems != null && warehouseItems.Count == warehouseCodes.Count)
                {
                    var data = result.Join(warehouseItems, i => i.WareHouse_Code, o => o.Code, (i, o) => new { i, o }).Select(m => new WMS_Area
                    {
                        Name = m.i.Name,
                        Code = m.i.Code,
                        Warehouse_Id = m.o.Id,
                        Status = (int)DataStatusEnum.Normal,
                        Remark = m.i.Remark,
                        Create_Time = DateTime.Now,
                        Creator = currentUserId,
                    });
                    await _db.BulkInsertAsync(data);
                    await _db.SaveChangesAsync();
                    return "Import Area successful";
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
        /// 根据库区编码判断该库区是否存在
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
                    throw new Exception("The area code parameter is empty");
                }
                var area = await _db.Areas.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (area == null)
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
        ///  更新库区信息
        /// </summary>
        /// <param name="updateAreaDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> UpdateAsync([FromBody] CreateOrUpdateAreaDTO updateAreaDTO, long currentUserId)
        {
            try
            {
                if (updateAreaDTO.Id <= 0)
                {
                    throw new Exception("Area ID does not exist");
                }
                var area = await _db.Areas.Where(m => m.Id == updateAreaDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (area == null)
                {
                    throw new Exception($"No information found for area,areaId is {updateAreaDTO.Id}");
                }
                if (!string.IsNullOrWhiteSpace(updateAreaDTO.Code))
                {
                    if (!updateAreaDTO.Code.Equals(area.Code))
                    {
                        if (await IsExistAsync(updateAreaDTO.Code))
                        {
                            throw new Exception($"Area code {updateAreaDTO.Code} already exists, duplicate creation is not allowed");
                        }
                    }
                    area.Code = updateAreaDTO.Code;
                }
                if (updateAreaDTO.Warehouse_Id != null && updateAreaDTO.Warehouse_Id > 0)
                {
                    var warehouse = await _db.WareHouses.Where(m => m.Id == updateAreaDTO.Warehouse_Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                    if (warehouse != null)
                    {
                        area.Warehouse_Id = (long)updateAreaDTO.Warehouse_Id;
                    }
                }
                area.Name = string.IsNullOrWhiteSpace(updateAreaDTO.Name) ? area.Name : updateAreaDTO.Name;
                area.Remark = string.IsNullOrWhiteSpace(updateAreaDTO.Remark) ? area.Remark : updateAreaDTO.Remark;
                area.Updator = currentUserId;
                area.Update_Time = DateTime.Now;
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
