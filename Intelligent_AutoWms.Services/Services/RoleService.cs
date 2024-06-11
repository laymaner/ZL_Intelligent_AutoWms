using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.Common.Utils;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.ImExportTemplate.Role;
using Intelligent_AutoWms.Model.RequestDTO.Role;
using Intelligent_AutoWms.Model.ResponseDTO.Role;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniExcelLibs;
using System.Data;

namespace Intelligent_AutoWms.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<RoleService> _log;

        public RoleService(Intelligent_AutoWms_DbContext dbContext, ILogger<RoleService> logger)
        {
            _db = dbContext;
            _log = logger;
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="createOrUpdateRoleDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> CreateRoleAsync([FromBody] CreateOrUpdateRoleDTO createOrUpdateRoleDTO, long currentUserId)
        {
            try 
            {
                WMS_Roles role = new WMS_Roles();
                if (!string.IsNullOrWhiteSpace(createOrUpdateRoleDTO.Code))
                {
                    if (await IsExistAsync(createOrUpdateRoleDTO.Code))
                    {
                        throw new Exception($"Role code {createOrUpdateRoleDTO.Code} already exists, duplicate creation is not allowed");
                    }
                    role.Code = createOrUpdateRoleDTO.Code;
                }
                else
                {
                    throw new Exception("Role code cannot be empty");
                }
                if (string.IsNullOrWhiteSpace(createOrUpdateRoleDTO.Name))
                {
                    throw new Exception("Role name cannot be empty");
                }
                role.Name = createOrUpdateRoleDTO.Name;
                role.Description = createOrUpdateRoleDTO.Description;
                role.Remark = createOrUpdateRoleDTO.Remark;
                role.Status = (int)DataStatusEnum.Normal;
                role.Creator = currentUserId;
                role.Create_Time = DateTime.Now;
                var result = await _db.Roles.AddAsync(role);
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
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<long> DelRoleAsync(string ids, long currentUserId)
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
                            throw new Exception($"Role ID:{id} does not exist");
                        }
                        var role = await _db.Roles.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                        if (role == null)
                        {
                            throw new Exception($"No information found for role,roleId is {id}");
                        }
                        if (role.Code.Equals("admin"))
                        {
                            throw new Exception("Administrator role does not allow deletion");
                        }
                        var items = await _db._User_Role_RelationShips.Where(m => m.Role_Id == role.Id && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                        if (items != null && items.Count > 0)
                        {
                            foreach (var item in items)
                            {
                                item.Status = (int)DataStatusEnum.Delete;
                                item.Update_Time = DateTime.Now;
                                item.Updator = currentUserId;
                            }
                        }
                        role.Status = (int)DataStatusEnum.Delete;
                        role.Update_Time = DateTime.Now;
                        role.Updator = currentUserId;
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
        /// 下载角色模板
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            try
            {
                List<RoleDownloadTemplate> list = new List<RoleDownloadTemplate>();
                return await MiniExcelUtil.ExportAsync("Role_Download_Template", list);
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
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<FileStreamResult> ExportAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            try
            {
                var result = _db.Roles.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking().Select(m => new RoleExportTemplate
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Remark = m.Remark,
                    Description = m.Description,
                    Create_Time = m.Create_Time,
                });
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(roleParamsDto.Name));
                }
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Code))
                {
                    result = result.Where(m => m.Name.StartsWith(roleParamsDto.Code));
                }

                return await MiniExcelUtil.ExportAsync("RoleInfomation", result);
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据编码获取角色信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<WMS_Roles> GetRoleInfoByCodeAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("Query parameter code is empty");
                }
                var role = await _db.Roles.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (role == null)
                {
                    throw new Exception($"No information found for Role,code is {code}");
                }
                return role;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据id获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<WMS_Roles> GetRoleInfoByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new Exception("Query parameter RoleId is empty");
                }
                var role = await _db.Roles.Where(m => m.Id == id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (role == null)
                {
                    throw new Exception($"No information found for Role,id is {id}");
                }
                return role;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public async Task<List<WMS_Roles>> GetRolesAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            try
            {
                var result = _db.Roles.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(roleParamsDto.Name));
                }
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(roleParamsDto.Code));
                }
                return await result.ToListAsync();
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 分页查询角色信息
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public async Task<BasePagination<WMS_Roles>> GetRolePaginationAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            try
            {
                var result = _db.Roles.Where(m => m.Status == (int)DataStatusEnum.Normal).OrderByDescending(n => n.Id).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(roleParamsDto.Name));
                }
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(roleParamsDto.Code));
                }
                return await PaginationService.PaginateAsync(result, roleParamsDto.PageIndex, roleParamsDto.PageSize);
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
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string> ImportAsync(string path, long currentUserId)
        {
            try
            {
                var result = MiniExcelUtil.Import<RoleDownloadTemplate>(path).ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断角色编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported role code");
                }
                //判断角色姓名有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported role name");
                }
                //判断角色编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("Role code duplication");
                }

                var codeList = result.Select(m => m.Code).ToList();
                var items = await _db.Roles.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (items != null && items.Count > 0)
                {
                    throw new Exception("Role code already exists");
                }

                var data = result.Select(m => new WMS_Roles
                {
                    Name = m.Name,
                    Code = m.Code,
                    Description = m.Description,
                    Status = (int)DataStatusEnum.Normal,
                    Creator = currentUserId,
                    Remark = m.Remark,
                    Create_Time = DateTime.Now,
                });
                await _db.BulkInsertAsync(data);
                await _db.SaveChangesAsync();
                return "Import Roles successful";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 判断角色是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool> IsExistAsync(string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new Exception("Query parameter code is empty");
                }
                var role = await _db.Roles.Where(m => m.Code.Equals(code) && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (role != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="createOrUpdateRoleDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public async Task<string> UpdateRoleAsync([FromBody] CreateOrUpdateRoleDTO createOrUpdateRoleDTO, long currentUserId)
        {
            try
            {
                if (createOrUpdateRoleDTO.Id <= 0)
                {
                    throw new Exception("Role ID does not exist");
                }
                var role = await _db.Roles.Where(m => m.Id == createOrUpdateRoleDTO.Id && m.Status == (int)DataStatusEnum.Normal).SingleOrDefaultAsync();
                if (role == null)
                {
                    throw new Exception($"No information found for role,roleId is {createOrUpdateRoleDTO.Id}");
                }
                if (!string.IsNullOrWhiteSpace(createOrUpdateRoleDTO.Code))
                {
                    if (!createOrUpdateRoleDTO.Code.Equals(role.Code))
                    {
                        if (await IsExistAsync(createOrUpdateRoleDTO.Code))
                        {
                            throw new Exception($"Role code {createOrUpdateRoleDTO.Code} already exists, duplicate creation is not allowed");
                        }
                    }
                    role.Code = createOrUpdateRoleDTO.Code;
                }

                //基础信息更新
                role.Name = string.IsNullOrWhiteSpace(createOrUpdateRoleDTO.Name) ? role.Name : createOrUpdateRoleDTO.Name;
                role.Description = string.IsNullOrWhiteSpace(createOrUpdateRoleDTO.Description) ? role.Description : createOrUpdateRoleDTO.Description;
                role.Remark = string.IsNullOrWhiteSpace(createOrUpdateRoleDTO.Remark) ? role.Remark : createOrUpdateRoleDTO.Remark;
                role.Updator = currentUserId;
                role.Update_Time = DateTime.Now;
                await _db.SaveChangesAsync();
                return "Successfully updated role information";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据codes集合获取角色数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<WMS_Roles>> GetRoleByIdsAsync(string ids)
        {
            try
            {
                var list = new List<WMS_Roles>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    var items = ids.Split(',').ToList();
                    List<long> idList = items.Select(s => long.Parse(s)).ToList();
                    list = await _db.Roles.Where(m => idList.Contains(m.Id) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 根据codes集合获取角色数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public async Task<List<WMS_Roles>> GetRoleByCodesAsync(string codes)
        {
            try
            {
                var list = new List<WMS_Roles>();
                if (!string.IsNullOrWhiteSpace(codes))
                {
                    var codeList = codes.Split(',').ToList();
                    list = await _db.Roles.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
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
        /// 获取角色选项集
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public async Task<List<RoleOptions>> GetRoleOptionsAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            try
            {
                List<RoleOptions> roleOptions = new List<RoleOptions>();
                var result = _db.Roles.Where(m => m.Status == (int)DataStatusEnum.Normal).AsNoTracking();
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Name))
                {
                    result = result.Where(m => m.Name.StartsWith(roleParamsDto.Name));
                }
                if (!String.IsNullOrWhiteSpace(roleParamsDto.Code))
                {
                    result = result.Where(m => m.Code.StartsWith(roleParamsDto.Code));
                }
                var item = await result.ToListAsync();
                roleOptions = item.Adapt<List<RoleOptions>>();
                return roleOptions;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 导入----excel导入
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ImportExcelAsync(IFormFile fileForm, long currentUserId)
        {
            try
            {
                if (!fileForm.FileName.Contains("Role_Download_Template"))
                {
                    throw new Exception("Please select the correct template to import");
                }
                var stream = fileForm.OpenReadStream();
                var result = stream.Query<RoleDownloadTemplate>().ToList();
                if (result == null || result.Count <= 0)
                {
                    throw new Exception("Import data is empty");
                }
                //判断角色编码有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Code)))
                {
                    throw new Exception("There is a null value in the imported role code");
                }
                //判断角色姓名有没有空值
                if (result.Any(m => string.IsNullOrWhiteSpace(m.Name)))
                {
                    throw new Exception("There is a null value in the imported role name");
                }
                //判断角色编码是否有重复
                if (result.GroupBy(m => m.Code).Any(group => group.Count() > 1))
                {
                    throw new Exception("Role code duplication");
                }

                var codeList = result.Select(m => m.Code).ToList();
                var items = await _db.Roles.Where(m => codeList.Contains(m.Code) && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                if (items != null && items.Count > 0)
                {
                    throw new Exception("Role code already exists");
                }

                var data = result.Select(m => new WMS_Roles
                {
                    Name = m.Name,
                    Code = m.Code,
                    Description = m.Description,
                    Status = (int)DataStatusEnum.Normal,
                    Creator = currentUserId,
                    Remark = m.Remark,
                    Create_Time = DateTime.Now,
                });
                await _db.BulkInsertAsync(data);
                await _db.SaveChangesAsync();
                return "Import Roles successful";
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
