using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Shelf;
using Intelligent_AutoWms.Model.RequestDTO.Shelf;
using Intelligent_AutoWms.Model.ResponseDTO.Shelf;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;

namespace Intelligent_AutoWms.Services.Services
{
    public class ShelfService : IShelfService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<ShelfService> _log;
        private readonly IAreaService _areaService;

        public ShelfService(Intelligent_AutoWms_DbContext db, ILogger<ShelfService> logger, IAreaService areaService)
        {
            _db = db;
            _log = logger;
            _areaService = areaService;
        }

        /// <summary>
        /// 创建货架
        /// </summary>
        /// <param name="createShelfDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateAsync([FromBody] CreateOrUpdateShelfDTO createShelfDTO, long currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createShelfDTO.Code))
                {
                    throw new Exception("The shelf code parameter is empty");
                }
                if (string.IsNullOrWhiteSpace(createShelfDTO.Name))
                {
                    throw new Exception("The shelf code parameter is empty");
                }
                if (createShelfDTO.Area_Id == null || createShelfDTO.Area_Id <= 0 )
                {
                    throw new Exception("The shelf areaId parameter is empty");
                }
                if (createShelfDTO.Lanway == null || createShelfDTO.Lanway <= 0)
                {
                    throw new Exception("Lanway Not in compliance with regulations");
                }
                if (createShelfDTO.Shelf_Rows == null || createShelfDTO.Shelf_Rows <= 0)
                {
                    throw new Exception("Shelf_Rows Not in compliance with regulations");
                }
                if (createShelfDTO.Shelf_Columns == null || createShelfDTO.Shelf_Columns <= 0)
                {
                    throw new Exception("Shelf_Columns Not in compliance with regulations");
                }
                if (createShelfDTO.Shelf_Layers == null || createShelfDTO.Shelf_Layers <= 0)
                {
                    throw new Exception("Shelf_Layers Not in compliance with regulations");
                }
                if (await IsExistAsync(createShelfDTO.Code))
                {
                    throw new Exception("The shelf already exists");
                }
                var area = await _areaService.GetAreaByIdAsync((long)createShelfDTO.Area_Id);
                if (area == null)
                {
                    throw new Exception("The AreaId does not exist");
                }
                WMS_Shelf shelf = new WMS_Shelf();
                shelf.Code = createShelfDTO.Code;
                shelf.Name = createShelfDTO.Name;
                shelf.Area_Id = (long)createShelfDTO.Area_Id;
                shelf.Lanway = (int)createShelfDTO.Lanway;
                shelf.Shelf_Rows = (int)createShelfDTO.Shelf_Rows;
                shelf.Shelf_Columns = (int)createShelfDTO.Shelf_Columns;
                shelf.Shelf_Layers = (int)createShelfDTO.Shelf_Layers;
                shelf.Remark = createShelfDTO.Remark;
                shelf.Creator = currentUserId;
                shelf.Create_Time = DateTime.Now;
                shelf.Status = (int)DataStatusEnum.Normal;
                var result = await _db.Shelves.AddAsync(shelf);
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
        /// 根据ids删除货架信息
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
                            throw new Exception("The shelf id parameter is empty");
                        }
                        var shelf = await _db.Shelves.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (shelf == null)
                        {
                            throw new Exception($"No information found for shelf,id is {id}");
                        }
                        if (await _db.Locations.AnyAsync(m => m.Shelf_Id == id && m.Status == (int)DataStatusEnum.Normal))
                        {
                            throw new Exception("The shelf is in use and cannot be deleted");
                        }
                        shelf.Status = (int)DataStatusEnum.Delete;
                        shelf.Update_Time = DateTime.Now;
                        shelf.Updator = currentUserId;
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
                List<ShelfDownloadTemplate> list = new List<ShelfDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("Shelf_Download_Template", list);
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
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            try
            {
                List<ShelfExportTemplate> list = new List<ShelfExportTemplate>();
                var items = _db.Shelves.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(shelfParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(shelfParamsDTO.Name));
                }
                if (shelfParamsDTO.AreaId != null && shelfParamsDTO.AreaId >0)
                {
                    items = items.Where(m => m.Area_Id == shelfParamsDTO.AreaId);
                }
                //获取所有库区id 
                var areaIds = await items.Select(m => m.Area_Id).Distinct().ToListAsync();
                if (areaIds != null && areaIds.Count > 0)
                {
                    var areaItems = await _db.Areas.Where(m => areaIds.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                    if (areaItems != null && areaItems.Count == areaIds.Count)
                    {
                        var data = await items.Join(areaItems, i => i.Area_Id, o => o.Id, (i, o) => new { i, o }).Select(m => new ShelfExportTemplate
                        {
                            Id = m.i.Id,
                            Name = m.i.Name,
                            Code = m.i.Code,
                            Area_Code = m.o.Code,
                            Lanway = m.i.Lanway,
                            Shelf_Rows = m.i.Shelf_Rows,
                            Shelf_Columns = m.i.Shelf_Columns,
                            Shelf_Layers = m.i.Shelf_Layers,
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
                return await MiniExcelUtil.ExportAsync("ShelfInfomation", list);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取货架信息
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Shelf>> GetListAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            try
            {
                var items = _db.Shelves.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(shelfParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(shelfParamsDTO.Name));
                }
                if (shelfParamsDTO.AreaId != null && shelfParamsDTO.AreaId > 0)
                {
                    items = items.Where(m => m.Area_Id == shelfParamsDTO.AreaId);
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
        /// 获取货架信息分页
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Shelf>> GetPaginationAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            try
            {
                var items = _db.Shelves.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(shelfParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(shelfParamsDTO.Name));
                }
                if (shelfParamsDTO.AreaId != null && shelfParamsDTO.AreaId > 0)
                {
                    items = items.Where(m => m.Area_Id == shelfParamsDTO.AreaId);
                }
                return await PaginationService.PaginateAsync(items,shelfParamsDTO.PageIndex,shelfParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据货架编码获取货架信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Shelf> GetShelfByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The shelf code parameter is empty");
                }
                var shelf = await _db.Shelves.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (shelf == null)
                {
                    throw new Exception($"No information found for shelf,code is {code}");
                }
                return shelf;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes获取货架信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Shelf>> GetShelfByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_Shelf>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.Shelves.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 根据货架id获取货架信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Shelf> GetShelfByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The shelf id parameter is empty");
                }
                var shelf = await _db.Shelves.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (shelf == null)
                {
                    throw new Exception($"No information found for shelf,id is {id}");
                }
                return shelf;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids获取货架信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Shelf>> GetShelfByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_Shelf>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.Shelves.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 获取货架选项集
        /// </summary>
        /// <param name="shelfParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<ShelfOptions>> GetShelfOptionsAsync([FromQuery] ShelfParamsDTO shelfParamsDTO)
        {
            try
            {
                List<ShelfOptions> shelfOptions = new List<ShelfOptions>();
                var items = _db.Shelves.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(shelfParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(shelfParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(shelfParamsDTO.Name));
                }
                if (shelfParamsDTO.AreaId != null && shelfParamsDTO.AreaId > 0)
                {
                    items = items.Where(m => m.Area_Id == shelfParamsDTO.AreaId);
                }
                var result =await items.ToListAsync();
                shelfOptions = result.Adapt<List<ShelfOptions>>();
                return shelfOptions;
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
                var result = MiniExcelUtil.Import<ShelfDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断货架编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported shelf code");
                }
                //判断货架名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported shelf name");
                }
                //判断库区编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Area_Code)))
                {
                    throw new Exception("There is a null value in the imported shelf AreaCode");
                }
                if (result.Any(m => m.Lanway == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                if (result.Any(m => m.Shelf_Rows == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                if (result.Any(m => m.Shelf_Columns == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                if (result.Any(m => m.Shelf_Layers == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                //判断货架编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("shelf code duplication");
                }
                //判断货架是否存在
                var shelfCodeList = result.Select(m => m.Code).ToList();
                var shelfItems = await _db.Shelves.Where(m => shelfCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (shelfItems != null && shelfItems.Count > 0)
                {
                    throw new Exception("shelf code already exists");
                }
                //获取所有库区id 
                var areaCodeList = result.Select(m => m.Area_Code).Distinct().ToList();
                var areaitems = await _db.Areas.Where(m => areaCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                if (areaitems != null && areaitems.Count == areaCodeList.Count)
                {
                    var data = result.Join(areaitems, i => i.Area_Code, o => o.Code, (i, o) => new { i, o }).Select(m => new WMS_Shelf
                    {
                        Name = m.i.Name,
                        Code = m.i.Code,
                        Area_Id = m.o.Id,
                        Lanway = m.i.Lanway,
                        Shelf_Rows = m.i.Shelf_Rows,
                        Shelf_Columns = m.i.Shelf_Columns,
                        Shelf_Layers = m.i.Shelf_Layers,
                        Status = (int)DataStatusEnum.Normal,
                        Remark = m.i.Remark,
                        Create_Time = DateTime.Now,
                        Creator = currentUserId,
                    });
                    await _db.BulkInsertAsync(data);
                    await _db.SaveChangesAsync();
                    return "Import Shelf successful";
                }
                else
                {
                    throw new Exception("There is an issue with the shelf status and it does not match");
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导入---excel导入
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ImportExcelAsync(IFormFile fileForm, long currentUserId)
        {
            try
            {
                if (!fileForm.FileName.Contains("Shelf_Download_Template"))
                {
                    throw new Exception("Please select the correct template to import");
                }
                var stream = fileForm.OpenReadStream();
                var result = stream.Query<ShelfDownloadTemplate>().ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断货架编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported shelf code");
                }
                //判断货架名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported shelf name");
                }
                //判断库区编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Area_Code)))
                {
                    throw new Exception("There is a null value in the imported shelf AreaCode");
                }
                if (result.Any(m => m.Lanway == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                if (result.Any(m => m.Shelf_Rows == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                if (result.Any(m => m.Shelf_Columns == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                if (result.Any(m => m.Shelf_Layers == 0))
                {
                    throw new Exception("Lanway,Shelf_Rows,Shelf_Columns,Shelf_Layers Not in compliance with regulations");
                }
                //判断货架编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("shelf code duplication");
                }
                //判断货架是否存在
                var shelfCodeList = result.Select(m => m.Code).ToList();
                var shelfItems = await _db.Shelves.Where(m => shelfCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (shelfItems != null && shelfItems.Count > 0)
                {
                    throw new Exception("shelf code already exists");
                }
                //获取所有库区id 
                var areaCodeList = result.Select(m => m.Area_Code).Distinct().ToList();
                var areaitems = await _db.Areas.Where(m => areaCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                if (areaitems != null && areaitems.Count == areaCodeList.Count)
                {
                    var data = result.Join(areaitems, i => i.Area_Code, o => o.Code, (i, o) => new { i, o }).Select(m => new WMS_Shelf
                    {
                        Name = m.i.Name,
                        Code = m.i.Code,
                        Area_Id = m.o.Id,
                        Lanway = m.i.Lanway,
                        Shelf_Rows = m.i.Shelf_Rows,
                        Shelf_Columns = m.i.Shelf_Columns,
                        Shelf_Layers = m.i.Shelf_Layers,
                        Status = (int)DataStatusEnum.Normal,
                        Remark = m.i.Remark,
                        Create_Time = DateTime.Now,
                        Creator = currentUserId,
                    });
                    await _db.BulkInsertAsync(data);
                    await _db.SaveChangesAsync();
                    return "Import Shelf successful";
                }
                else
                {
                    throw new Exception("There is an issue with the shelf status and it does not match");
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据货架编码判断货架是否存在
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
                    throw new Exception("The shelf code parameter is empty");
                }
                var shelf = await _db.Shelves.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (shelf == null)
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
        /// 修改货架信息
        /// </summary>
        /// <param name="updateShelfDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> UpdateAsync([FromBody] CreateOrUpdateShelfDTO updateShelfDTO, long currentUserId)
        {
            try
            {
                if (updateShelfDTO.Id <= 0)
                {
                    throw new Exception("Shelf ID does not exist");
                }
                var shelf = await _db.Shelves.Where(m => m.Id == updateShelfDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (shelf == null)
                {
                    throw new Exception($"No information found for shelf,shelfId is {updateShelfDTO.Id}");
                }
                if (!string.IsNullOrWhiteSpace(updateShelfDTO.Code))
                {
                    if (!updateShelfDTO.Code.Equals(shelf.Code))
                    {
                        if (await IsExistAsync(updateShelfDTO.Code))
                        {
                            throw new Exception($"Shelf code {updateShelfDTO.Code} already exists, duplicate creation is not allowed");
                        }
                    }
                    shelf.Code = updateShelfDTO.Code;
                }
                if (!string.IsNullOrWhiteSpace(updateShelfDTO.Name))
                {
                    shelf.Name = updateShelfDTO.Name;
                }
                if (updateShelfDTO.Area_Id != null && updateShelfDTO.Area_Id > 0)
                {
                    var area = await _areaService.GetAreaByIdAsync((long)updateShelfDTO.Area_Id);
                    shelf.Area_Id = (long)updateShelfDTO.Area_Id;
                }
                var locations = await _db.Locations.Where(m => m.Shelf_Id == updateShelfDTO.Id && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                //当货架未被货位引用 无需判断修改值与引用值大小
                if (locations != null && locations.Count > 0)
                {
                    //当巷道已被引用时 不让修改
                    // 判断巷道 
                    if (updateShelfDTO.Lanway != null && updateShelfDTO.Lanway > 0)
                    {
                        throw new Exception("Referenced and not allowed to be modified");
                    }
                    // 判断排
                    if (updateShelfDTO.Shelf_Rows != null && updateShelfDTO.Shelf_Rows > 0)
                    {
                        var l_Row = locations.Max(m => m.Location_Row);
                        if (updateShelfDTO.Shelf_Rows < l_Row)
                        {
                            throw new Exception("The modified value cannot be less than the actual reference value");
                        }
                        shelf.Shelf_Rows = (int)updateShelfDTO.Shelf_Rows;
                    }
                    // 判断列
                    if (updateShelfDTO.Shelf_Columns != null && updateShelfDTO.Shelf_Columns > 0)
                    {
                        var l_Column = locations.Max(m => m.Location_Column);
                        if (updateShelfDTO.Shelf_Columns < l_Column)
                        {
                            throw new Exception("The modified value cannot be less than the actual reference value");
                        }
                        shelf.Shelf_Columns = (int)updateShelfDTO.Shelf_Columns;
                    }
                    // 判断层
                    if (updateShelfDTO.Shelf_Layers != null && updateShelfDTO.Shelf_Layers > 0)
                    {
                        var l_Layer = locations.Max(m => m.Location_Layer);
                        if (updateShelfDTO.Shelf_Layers < l_Layer)
                        {
                            throw new Exception("The modified value cannot be less than the actual reference value");
                        }
                        shelf.Shelf_Layers = (int)updateShelfDTO.Shelf_Layers;
                    }
                }
                else
                {
                    if (updateShelfDTO.Lanway != null && updateShelfDTO.Lanway > 0)
                    {
                        shelf.Lanway = (int)updateShelfDTO.Lanway;
                    }
                    if (updateShelfDTO.Shelf_Rows != null && updateShelfDTO.Shelf_Rows > 0)
                    {
                        shelf.Shelf_Rows = (int)updateShelfDTO.Shelf_Rows;
                    }
                    if (updateShelfDTO.Shelf_Columns != null && updateShelfDTO.Shelf_Columns > 0)
                    {
                        shelf.Shelf_Columns = (int)updateShelfDTO.Shelf_Columns;
                    }
                    if (updateShelfDTO.Shelf_Layers != null && updateShelfDTO.Shelf_Layers > 0)
                    {
                        shelf.Shelf_Layers = (int)updateShelfDTO.Shelf_Layers;
                    }
                }
                shelf.Remark = string.IsNullOrWhiteSpace(updateShelfDTO.Remark) ? shelf.Remark : updateShelfDTO.Remark;
                shelf.Update_Time = DateTime.Now;
                shelf.Updator = currentUserId;
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
