using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.WareHouse;
using Intelligent_AutoWms.Model.RequestDTO.Warehouse;
using Intelligent_AutoWms.Model.RequestDTO.WareHouse;
using Intelligent_AutoWms.Model.ResponseDTO.WareHouse;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class WareHouseService : IWareHouseService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<WareHouseService> _log;

        public WareHouseService(Intelligent_AutoWms_DbContext db, ILogger<WareHouseService> logger)
        {
            _db = db;
            _log = logger;
        }

        /// <summary>
        /// 获取仓库选项集
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public async Task<List<WareHouseOptions>> GetWareHouseOptionsAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            try
            {
                List<WareHouseOptions> wareHouseOptions = new List<WareHouseOptions>();
                var items = _db.WareHouses.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(wareHouseParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(wareHouseParamsDTO.Name));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.WareHouseType))
                {
                    items = items.Where(m => m.Type.StartsWith(wareHouseParamsDTO.WareHouseType));
                }
                var result =await items.ToListAsync();
                wareHouseOptions = result.Adapt<List<WareHouseOptions>>();
                return wareHouseOptions;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新仓库信息
        /// </summary>
        /// <param name="updateWareHouseDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> UpdateAsync([FromBody] CreateOrUpdateWareHouseDTO updateWareHouseDTO, long currentUserId)
        {
            try
            {
                if (updateWareHouseDTO.Id ==null || updateWareHouseDTO.Id <= 0 )
                {
                    throw new Exception("The warehouse id parameter is empty");
                }
                var wareHouse = await _db.WareHouses.Where(m => m.Id == updateWareHouseDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (wareHouse == null)
                {
                    throw new Exception($"No information found for wareHouse,wareHouseId is {updateWareHouseDTO.Id}");
                }
                if (!string.IsNullOrWhiteSpace(updateWareHouseDTO.Code))
                {
                    if (await IsExistAsync(updateWareHouseDTO.Code))
                    {
                        throw new Exception($"WareHouse code {updateWareHouseDTO.Code} already exists, duplicate creation is not allowed");
                    }
                    wareHouse.Code = updateWareHouseDTO.Code;
                }
                wareHouse.Name = string.IsNullOrWhiteSpace(updateWareHouseDTO.Name) ? wareHouse.Name : updateWareHouseDTO.Name;
                wareHouse.Type = string.IsNullOrWhiteSpace(updateWareHouseDTO.Type) ? wareHouse.Type : updateWareHouseDTO.Type;
                wareHouse.Remark = string.IsNullOrWhiteSpace(updateWareHouseDTO.Remark) ? wareHouse.Remark : updateWareHouseDTO.Remark;
                wareHouse.Updator = currentUserId;
                wareHouse.Update_Time = DateTime.Now;
                await _db.SaveChangesAsync();
                return wareHouse.Id;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 创建仓库
        /// </summary>
        /// <param name="createWareHouseDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> CreateAsync([FromBody] CreateOrUpdateWareHouseDTO createWareHouseDTO, long currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createWareHouseDTO.Code))
                {
                    throw new Exception("The warehouse code parameter is empty");
                }
                if (string.IsNullOrWhiteSpace(createWareHouseDTO.Name))
                {
                    throw new Exception("The warehouse name parameter is empty");
                }
                if (string.IsNullOrWhiteSpace(createWareHouseDTO.Type))
                {
                    throw new Exception("The warehouse name parameter is empty");
                }
                if (await IsExistAsync(createWareHouseDTO.Code))
                {
                    throw new Exception("The warehouse already exists");
                }
                WMS_WareHouse wareHouse = new WMS_WareHouse();
                wareHouse.Code = createWareHouseDTO.Code;
                wareHouse.Name = createWareHouseDTO.Name;
                wareHouse.Create_Time = DateTime.Now;
                wareHouse.Creator = currentUserId;
                wareHouse.Type = createWareHouseDTO.Type;
                wareHouse.Remark = createWareHouseDTO.Remark;
                wareHouse.Status = (int)DataStatusEnum.Normal;
                var result = await _db.WareHouses.AddAsync(wareHouse);
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
        /// 根据仓库ids删除仓库信息 
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
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
                            throw new Exception("The warehouse id parameter is empty");
                        }
                        var wareHouse = await _db.WareHouses.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (wareHouse == null)
                        {
                            throw new Exception($"No information found for Warehouse,id is {id}");
                        }
                        if (await _db.Areas.AnyAsync(m => m.Warehouse_Id == id && m.Status == (int)DataStatusEnum.Normal))
                        {
                            throw new Exception("The warehouse is in use and cannot be deleted");
                        }
                        wareHouse.Status = (int)DataStatusEnum.Delete;
                        wareHouse.Update_Time = DateTime.Now;
                        wareHouse.Updator = currentUserId;
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
        /// 下载仓库模板
        /// </summary>
        /// <returns></returns>
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            try
            {
                List<WareHouseDownloadTemplate> list = new List<WareHouseDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("WareHouse_Download_Template", list);
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
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public async Task<FileStreamResult> ExportAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            try
            {
                var items = _db.WareHouses.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(wareHouseParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(wareHouseParamsDTO.Name));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.WareHouseType))
                {
                    items = items.Where(m => m.Type.StartsWith(wareHouseParamsDTO.WareHouseType));
                }
                var result = items.Adapt<List<WareHouseExportTemplate>>();
                return await MiniExcelUtil.ExportAsync("WareHouseInfomation", result);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询仓库信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<WMS_WareHouse>> GetListAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            try
            {
                var items = _db.WareHouses.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(wareHouseParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(wareHouseParamsDTO.Name));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.WareHouseType))
                {
                    items = items.Where(m => m.Type.StartsWith(wareHouseParamsDTO.WareHouseType));
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
        /// 查询仓库信息分页
        /// </summary>
        /// <param name="wareHouseParamsDTO"></param>
        /// <returns></returns>
        public async Task<BasePagination<WMS_WareHouse>> GetPaginationAsync([FromQuery] WareHouseParamsDTO wareHouseParamsDTO)
        {
            try
            {
                var items = _db.WareHouses.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Code))
                {
                    items = items.Where(m => m.Code.StartsWith(wareHouseParamsDTO.Code));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.Name))
                {
                    items = items.Where(m => m.Name.StartsWith(wareHouseParamsDTO.Name));
                }
                if (!string.IsNullOrWhiteSpace(wareHouseParamsDTO.WareHouseType))
                {
                    items = items.Where(m => m.Type.StartsWith(wareHouseParamsDTO.WareHouseType));
                }
                return await PaginationService.PaginateAsync(items, wareHouseParamsDTO.PageIndex, wareHouseParamsDTO.PageSize);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据仓库编码查询仓库信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_WareHouse> GetWareHouseByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("The warehouse code parameter is empty");
                }
                var wareHouse = await _db.WareHouses.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (wareHouse == null)
                {
                    throw new Exception($"No information found for Warehouse,code is {code}");
                }
                return wareHouse;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes集合获取用户数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public async Task<List<WMS_WareHouse>> GetWareHouseByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_WareHouse>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.WareHouses.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 根据仓库id查询仓库信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<WMS_WareHouse> GetWareHouseByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("The warehouse id parameter is empty");
                }
                var wareHouse = await _db.WareHouses.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (wareHouse == null)
                {
                    throw new Exception($"No information found for Warehouse,id is {id}");
                }
                return wareHouse;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据ids集合获取仓库数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<WMS_WareHouse>> GetWareHouseByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_WareHouse>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.WareHouses.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
                var result = MiniExcelUtil.Import<WareHouseDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断仓库编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported warehouse code");
                }
                //判断仓库名称有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported warehouse name");
                }
                //判断仓库类型有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Type)))
                {
                    throw new Exception("There is a null value in the imported warehouse type");
                }
                //判断仓库编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("warehouse code duplication");
                }
                //判断仓库是否存在
                var wareCodeList = result.Select(m => m.Code).ToList();
                var wareHouseItems = await _db.WareHouses.Where(m => wareCodeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (wareHouseItems != null && wareHouseItems.Count > 0)
                {
                    throw new Exception("warehouse code already exists");
                }

                var data = result.Select(m => new WMS_WareHouse
                {
                    Name = m.Name,
                    Code = m.Code,
                    Type = m.Type,
                    Status = (int)DataStatusEnum.Normal,
                    Creator = currentUserId,
                    Remark = m.Remark,
                    Create_Time = DateTime.Now,

                });
                await _db.BulkInsertAsync(data);
                await _db.SaveChangesAsync();
                return "Import Warehouse successful";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据code判断仓库是否存在
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
                    throw new Exception("The warehouse code parameter is empty");
                }
                var wareHouse = await _db.WareHouses.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (wareHouse == null)
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
    }
}
