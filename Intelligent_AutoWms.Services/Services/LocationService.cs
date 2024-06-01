using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Location;
using Intelligent_AutoWms.Model.RequestDTO.Location;
using Intelligent_AutoWms.Model.ResponseDTO.Location;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class LocationService : ILocationService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<LocationService> _log;
        private readonly IShelfService _shelfService;

        public LocationService(Intelligent_AutoWms_DbContext db, ILogger<LocationService> logger, IShelfService shelfService)
        {
            _db = db;
            _log = logger;
            _shelfService = shelfService;
        }

        /// <summary>
        /// 创建货位 前端注意校验code = 排+列+层
        /// </summary>
        /// <param name="createLocationDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> CreateAsync([FromBody] CreateOrUpdateLocationDTO createLocationDTO, long currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createLocationDTO.Code))
                {
                    throw new Exception("The location code parameter is empty");
                }
                if (string.IsNullOrWhiteSpace(createLocationDTO.Name))
                {
                    throw new Exception("The location name parameter is empty");
                }
                if (createLocationDTO.Shelf_Id == null || createLocationDTO.Shelf_Id <= 0)
                {
                    throw new Exception("The location shelfId parameter is empty");
                }
                var shelf = await _shelfService.GetShelfByIdAsync((long)createLocationDTO.Shelf_Id);
                if (shelf == null)
                {
                    throw new Exception("The shelfId does not exist");
                }
                //判断 巷道 排列层与货架比对
                if (createLocationDTO.Lanway == null || createLocationDTO.Lanway <=0 || createLocationDTO.Lanway != shelf.Lanway)
                {
                    throw new Exception("Lanway Not in compliance with regulations");
                }
                if (createLocationDTO.Location_Row == null || createLocationDTO.Location_Row <= 0 || createLocationDTO.Location_Row != shelf.Shelf_Rows)
                {
                    throw new Exception("Location_Row Not in compliance with regulations");
                }
                if (createLocationDTO.Location_Column == null || createLocationDTO.Location_Column <= 0 || createLocationDTO.Location_Column > shelf.Shelf_Columns)
                {
                    throw new Exception("Location_Column Not in compliance with regulations");
                }
                if (createLocationDTO.Location_Layer == null || createLocationDTO.Location_Layer <= 0 || createLocationDTO.Location_Layer > shelf.Shelf_Layers)
                {
                    throw new Exception("Location_Layer Not in compliance with regulations");
                }
                //校验code = 排+列+层
                if (!JudeDataVerification(createLocationDTO.Code,(int)createLocationDTO.Location_Row, (int)createLocationDTO.Location_Column, (int)createLocationDTO.Location_Layer))
                {
                    throw new Exception("The shelf code does not match the arrangement layer");
                }
                if (await IsExistAsync(createLocationDTO.Code))
                {
                    throw new Exception("The location already exists");
                }
                WMS_Location location = new WMS_Location();
                location.Code = createLocationDTO.Code;
                location.Name = createLocationDTO.Name;
                location.Shelf_Id = (long)createLocationDTO.Shelf_Id;
                location.Lanway = (int)createLocationDTO.Lanway;
                location.Location_Row = (int)createLocationDTO.Location_Row;
                location.Location_Column = (int)createLocationDTO.Location_Column;
                location.Location_Layer = (int)createLocationDTO.Location_Layer;
                location.Create_Time = DateTime.Now;
                location.Creator = currentUserId;
                location.Step = (int)LocationStatusEnum.Idle;
                location.Remark = createLocationDTO.Remark;
                location.Status = (int)DataStatusEnum.Normal;
                var result = await _db.Locations.AddAsync(location);
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
        /// 根据ids删除货位信息
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
                            throw new Exception("The location id parameter is empty");
                        }
                        var location = await _db.Locations.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (location == null)
                        {
                            throw new Exception($"No information found for location,id is {id}");
                        }
                        //只有空闲的货位才能被删除
                        if (location.Step != (int)LocationStatusEnum.Idle)
                        {
                            throw new Exception("The shelf is in use and cannot be deleted");
                        }

                        location.Status = (int)DataStatusEnum.Delete;
                        location.Update_Time = DateTime.Now;
                        location.Updator = currentUserId;
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
                List<LocationDownloadTemplate> list = new List<LocationDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("Location_Download_Template", list);
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
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            try
            {
                List<LocationExportTemplate> list = new List<LocationExportTemplate>();
                var items = _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(locationParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(locationParamsDTO.Name));
                }
                //获取所有货架id 
                var shelfIds = await items.Select(m => m.Shelf_Id).Distinct().ToListAsync();
                if (shelfIds != null && shelfIds.Count > 0)
                {
                    var shelfItems = await _db.Shelves.Where(m => shelfIds.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name }).ToListAsync();
                    if (shelfItems != null && shelfItems.Count == shelfIds.Count)
                    {
                        var data = await items.Join(shelfItems, i => i.Shelf_Id, o => o.Id, (i, o) => new { i, o }).Select(m => new LocationExportTemplate
                        {
                            Id = m.i.Id,
                            Name = m.i.Name,
                            Code = m.i.Code,
                            Shelf_Code = m.o.Code,
                            Location_Row = m.i.Location_Row,
                            Location_Column = m.i.Location_Column,
                            Location_Layer = m.i.Location_Layer,
                            LocationStatus = EnumUtil.GetEnumDescription<LocationStatusEnum>((LocationStatusEnum)m.i.Step),
                            Remark = m.i.Remark,
                            Create_Time = m.i.Create_Time
                        }).ToListAsync();
                        list = data;
                    }
                    else
                    {
                        throw new Exception("There is an issue with the shelf status and it does not match");
                    }
                }
                return await MiniExcelUtil.ExportAsync("LocationInfomation", list);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询货位信息
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Location>> GetListAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            try
            {
                var items = _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(locationParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(locationParamsDTO.Name));
                }
                if (locationParamsDTO.Step != null && locationParamsDTO.Step > 0)
                {
                    items = items.Where(m => m.Step == locationParamsDTO.Step);
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
        /// 根据货位编码获取货位信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Location> GetLocationByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The location code parameter is empty");
                }
                var location = await _db.Locations.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
                {
                    throw new Exception($"No information found for location,code is {code}");
                }
                return location;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes获取货位信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Location>> GetLocationByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_Location>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.Locations.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 根据货位id查询货位信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Location> GetLocationByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The location id parameter is empty");
                }
                var location = await _db.Locations.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
                {
                    throw new Exception($"No information found for location,id is {id}");
                }
                return location;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据集合ids查询货位信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<WMS_Location>> GetLocationByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_Location>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.Locations.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 获取货位选项集
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<LocationOptions>> GetLocationOptionsAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            try
            {
                List<LocationOptions> locationOptions = new List<LocationOptions>();
                var items = _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(locationParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(locationParamsDTO.Name));
                }
                if (locationParamsDTO.Step != null && locationParamsDTO.Step > 0)
                {
                    items = items.Where(m => m.Step == locationParamsDTO.Step);
                }
                var result = await items.ToListAsync();
                locationOptions = result.Adapt<List<LocationOptions>>();
                return locationOptions;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询货位分页信息
        /// </summary>
        /// <param name="locationParamsDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<BasePagination<WMS_Location>> GetPaginationAsync([FromQuery] LocationParamsDTO locationParamsDTO)
        {
            try
            {
                var items = _db.Locations.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(locationParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(locationParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(locationParamsDTO.Name));
                }
                if (locationParamsDTO.Step != null && locationParamsDTO.Step > 0)
                {
                    items = items.Where(m => m.Step == locationParamsDTO.Step);
                }
                return await PaginationService.PaginateAsync(items, locationParamsDTO.PageIndex, locationParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 导入 注意code = 排+列+层！！！
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ImportAsync(string path, long currentUserId)
        {
            try
            {
                var result = MiniExcelUtil.Import<LocationDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断货位编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported location code");
                }
                //判断货位名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported location name");
                }
                //判断货架编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Shelf_Code)))
                {
                    throw new Exception("There is a null value in the imported location ShelfCode");
                }
                //判断货位编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("location code duplication");
                }
                //判断货位是否存在
                var locationCodeList = result.Select(m => m.Code).ToList();
                var locationItems = await _db.Locations.Where(m => locationCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (locationItems != null && locationItems.Count > 0)
                {
                    throw new Exception("location code already exists");
                }

                //判断货架是否存在
                var shelfCodeList = result.Select(m => m.Shelf_Code).Distinct().ToList();
                var shelfitems = await _db.Shelves.Where(m => shelfCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).Select(x => new { x.Id, x.Code, x.Name, x.Lanway,x.Shelf_Rows,x.Shelf_Columns,x.Shelf_Layers}).ToListAsync();
                if (shelfitems != null && shelfitems.Count == shelfCodeList.Count)
                {
                    // 表连接 判断 货位的巷道、排、列、层 是否符合
                    var data = result.Join(shelfitems, i => i.Shelf_Code, o => o.Code, (i, o) => new { i, o }).Select(m => new 
                    {
                        Name = m.i.Name,
                        Code = m.i.Code,
                        Status = (int)DataStatusEnum.Normal,
                        Shelf_Id = m.o.Id,
                        Lanway = m.i.Lanway,
                        Location_Row = m.i.Location_Row,
                        Location_Column = m.i.Location_Column,
                        Location_Layer = m.i.Location_Layer,
                        Step = (int)LocationStatusEnum.Idle,
                        Creator = currentUserId,
                        Remark = m.i.Remark,
                        Create_Time = DateTime.Now,
                        Shelf_Lanway =m.o.Lanway,
                        Shelf_Rows = m.o.Shelf_Rows,
                        Shelf_Columns = m.o.Shelf_Columns,
                        Shelf_Layers = m.o.Shelf_Layers,
                    });
                    var jugeResult = data.Where(m => m.Lanway != m.Shelf_Lanway || m.Location_Row != m.Shelf_Rows || m.Location_Column > m.Shelf_Columns || m.Location_Layer > m.Shelf_Layers).ToList();
                    if (jugeResult.Count > 0)
                    {
                        throw new Exception("There is non-compliance with regulations in Lanway,Location_Row,Location_Column,Location_Layer");
                    }
                    var items =data.Adapt<List<WMS_Location>>();
                    await _db.BulkInsertAsync(items);
                    await _db.SaveChangesAsync();
                    return "Import location successful";
                }
                else
                {
                    throw new Exception("There is an issue with the location status and it does not match");
                }
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据货位编码判断 该货位是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsExistAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The location code parameter is empty");
                }
                var location = await _db.Locations.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
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
        /// 根据货位编码判断 该货位是否空闲
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> IsIdleAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The location code parameter is empty");
                }
                var location = await _db.Locations.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
                {
                    throw new Exception($"No information found for location,code is {code}");
                }
                else 
                {
                    if (location.Step == (int)LocationStatusEnum.Idle)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据货位编码判断 该货位是否锁定
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> IsLockAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The location code parameter is empty");
                }
                var location = await _db.Locations.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
                {
                    throw new Exception($"No information found for location,code is {code}");
                }
                else
                {
                    if (location.Step == (int)LocationStatusEnum.Warehousing_Lock || location.Step == (int)LocationStatusEnum.Outbound_Lock)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据货位编码判断 该货位是否占用
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> IsOccupyAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The location code parameter is empty");
                }
                var location = await _db.Locations.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
                {
                    throw new Exception($"No information found for location,code is {code}");
                }
                else
                {
                    if (location.Step == (int)LocationStatusEnum.Occupying )
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据入口编码推荐最佳货位
        /// </summary>
        /// <param name="portCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_Location> RecommendedStorageLocationAsync(string portCode)
        {
            try
            {
                var port = await _db.Ports.Where(m => m.Code.Equals(portCode) &&  m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (port == null) 
                {
                    throw new Exception($"No information found for port,code is {portCode}");

                }
                if (port.First_Lanway != 0)
                {
                    var location = await _db.Locations.Where(m => m.Lanway == port.First_Lanway && m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).OrderBy(n => n.Location_Row).ThenBy(x => x.Location_Column).ThenBy(y => y.Location_Layer).FirstOrDefaultAsync();
                    if (location != null)
                    {
                        return location;
                    }
                }
                if (port.Second_Lanway != 0)
                {
                    var location = await _db.Locations.Where(m => m.Lanway == port.Second_Lanway && m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).OrderBy(n => n.Location_Row).ThenBy(x => x.Location_Column).ThenBy(y =>y.Location_Layer).FirstOrDefaultAsync();
                    if (location != null)
                    {
                        return location;
                    }
                }
                if (port.Third_Lanway != 0)
                {
                    var location = await _db.Locations.Where(m => m.Lanway == port.Third_Lanway && m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).OrderBy(n => n.Location_Row).ThenBy(x => x.Location_Column).ThenBy(y => y.Location_Layer).FirstOrDefaultAsync();
                    if (location != null)
                    {
                        return location;
                    }
                }
                if (port.Forth_Lanway != 0)
                {
                    var location = await _db.Locations.Where(m => m.Lanway == port.Forth_Lanway && m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).OrderBy(n => n.Location_Row).ThenBy(x => x.Location_Column).ThenBy(y => y.Location_Layer).FirstOrDefaultAsync();
                    if (location != null)
                    {
                        return location;
                    }
                }
                if (port.Fifth_Lanway != 0)
                {
                    var location = await _db.Locations.Where(m => m.Lanway == port.Fifth_Lanway && m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).OrderBy(n => n.Location_Row).ThenBy(x => x.Location_Column).ThenBy(y => y.Location_Layer).FirstOrDefaultAsync();
                    if (location != null)
                    {
                        return location;
                    }
                }
                if (port.Sixth_Lanway != 0)
                {
                    var location = await _db.Locations.Where(m => m.Lanway == port.Sixth_Lanway && m.Status == (int)DataStatusEnum.Normal && m.Step == (int)LocationStatusEnum.Idle).OrderBy(n => n.Location_Row).ThenBy(x => x.Location_Column).ThenBy(y => y.Location_Layer).FirstOrDefaultAsync();
                    if (location != null)
                    {
                        return location;
                    }
                }
                throw new Exception("Recommended Storage Location Failed");
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 修改货位信息 前端注意校验code = 排+列+层
        /// </summary>
        /// <param name="updateLocationDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<long> UpdateAsync([FromBody] CreateOrUpdateLocationDTO updateLocationDTO, long currentUserId)
        {
            try
            {
                if (updateLocationDTO.Id <= 0)
                {
                    throw new Exception("Location ID does not exist");
                }
                var location = await _db.Locations.Where(m => m.Id == updateLocationDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (location == null)
                {
                    throw new Exception($"No information found for location,locationId is {updateLocationDTO.Id}");
                }
                if (location.Step != (int)LocationStatusEnum.Idle)
                {
                    throw new Exception("Not allowed to modify in non idle state");
                }
                if (!string.IsNullOrWhiteSpace(updateLocationDTO.Code))
                {
                    if (!updateLocationDTO.Code.Equals(location.Code))
                    {
                        if (await IsExistAsync(updateLocationDTO.Code))
                        {
                            throw new Exception($"Location code {updateLocationDTO.Code} already exists, duplicate creation is not allowed");
                        }
                    }                
                    location.Code = updateLocationDTO.Code;
                }
                if (!string.IsNullOrWhiteSpace(updateLocationDTO.Name))
                {
                    location.Name = updateLocationDTO.Name;
                }
                if (updateLocationDTO.Shelf_Id != null && updateLocationDTO.Shelf_Id > 0)
                {
                    var shelf = await _shelfService.GetShelfByIdAsync((long)updateLocationDTO.Shelf_Id);
                    location.Shelf_Id = (long)updateLocationDTO.Shelf_Id;
                    if (updateLocationDTO.Lanway != null && updateLocationDTO.Lanway > 0)
                    {
                        if (updateLocationDTO.Lanway != shelf.Lanway)
                        {
                            throw new Exception("The location lanway does not match the shelf lanway");
                        }
                        location.Lanway = (int)updateLocationDTO.Lanway;
                    }
                    if (updateLocationDTO.Location_Row != null && updateLocationDTO.Location_Row > 0)
                    {
                        if (updateLocationDTO.Location_Row != shelf.Shelf_Rows)
                        {
                            throw new Exception("The number of storage locations exceeds the number of shelves");
                        }
                        location.Location_Row = (int)updateLocationDTO.Location_Row;
                    }
                    if (updateLocationDTO.Location_Column != null && updateLocationDTO.Location_Column > 0)
                    {
                        if (updateLocationDTO.Location_Column > shelf.Shelf_Columns)
                        {
                            throw new Exception("The number of storage space columns exceeds the number of shelf columns");
                        }
                        location.Location_Column = (int)updateLocationDTO.Location_Column;
                    }
                    if (updateLocationDTO.Location_Layer != null && updateLocationDTO.Location_Layer > 0)
                    {
                        if (updateLocationDTO.Location_Layer > shelf.Shelf_Layers)
                        {
                            throw new Exception("The number of storage space columns exceeds the number of shelf layers");
                        }
                        location.Location_Layer = (int)updateLocationDTO.Location_Layer;
                    }
                }
                //校验code = 排+列+层
                if (!string.IsNullOrEmpty(updateLocationDTO.Code) && updateLocationDTO.Location_Row != null && updateLocationDTO.Location_Row > 0 && updateLocationDTO.Location_Column != null && updateLocationDTO.Location_Column > 0 && updateLocationDTO.Location_Layer != null && updateLocationDTO.Location_Layer > 0)
                {
                    if (!JudeDataVerification(updateLocationDTO.Code, (int)updateLocationDTO.Location_Row, (int)updateLocationDTO.Location_Column, (int)updateLocationDTO.Location_Layer))
                    {
                        throw new Exception("The shelf code does not match the arrangement layer");
                    }
                }

                location.Remark = string.IsNullOrWhiteSpace(updateLocationDTO.Remark) ? location.Remark : updateLocationDTO.Remark;
                location.Update_Time = DateTime.Now;
                location.Updator = currentUserId;
                return await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 校验货架编码是否和排列层匹配
        /// </summary>
        /// <param name="code"></param>
        /// <param name="rowNum"></param>
        /// <param name="columnNum"></param>
        /// <param name="layerNum"></param>
        /// <returns></returns>
        public bool JudeDataVerification(string code,int rowNum,int columnNum,int layerNum)
        {
            string merge = "";
            if (rowNum < 10)
            {
                merge = rowNum.ToString().PadLeft(2, '0');
            }
            else
            {
                merge = rowNum.ToString();
            }

            if (columnNum < 10)
            {
                merge += columnNum.ToString().PadLeft(2, '0');
            }
            else
            {
                merge += columnNum.ToString();
            }

            if (layerNum < 10)
            {
                merge += layerNum.ToString().PadLeft(2, '0');
            }
            else
            {
                merge += layerNum.ToString();
            }
            if (merge.Length == code.Length && merge.Equals(code))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 生成货位模板
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<FileStreamResult> Test(int rows,int columns,int layers)
        {
            try
            {
                List<LocationDownloadTemplate> list = new List<LocationDownloadTemplate>();
                for (int i =1;i<= rows; i++)
                {
                    for(int j =1;j<= columns; j++)
                    {
                        for (int k = 1; k <= layers; k++)
                        {
                            LocationDownloadTemplate template = new LocationDownloadTemplate();
                            template.Name = i.ToString().PadLeft(2, '0') + j.ToString().PadLeft(2, '0') + k.ToString().PadLeft(2, '0');
                            template.Code = i.ToString().PadLeft(2, '0') + j.ToString().PadLeft(2, '0') + k.ToString().PadLeft(2, '0');                          
                            if (i >=10)
                            {
                                template.Shelf_Code = "S0" + i;
                            }
                            else
                            {
                                template.Shelf_Code = "S00" + i;
                            }
                            if (i % 2 == 0)
                            {
                                template.Lanway = i / 2;
                            }
                            else
                            {
                                template.Lanway = i % 2;
                            }
                            template.Location_Row = i;
                            template.Location_Column = j;
                            template.Location_Layer = k;
                            template.Remark = "EXCEL导入基础参数";
                            list.Add(template);
                        }
                    }
                }
                return await MiniExcelUtil.ExportAsync("Location_Download_Template", list);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
