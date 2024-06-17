using Intelligent_AutoWms.Common.Enum;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Permission;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Intelligent_AutoWms.Services.Services
{
    public class PermissionService:IPermissionService
    {
        private readonly Intelligent_AutoWms_DbContext _db;
        private readonly ILogger<PermissionService> _log;
        private readonly IRoleService _iroleService;

        public PermissionService(Intelligent_AutoWms_DbContext db, ILogger<PermissionService> logger,IRoleService roleService)
        {
            _db = db;   
            _log = logger;
            _iroleService = roleService;
        }

        /// <summary>
        ///  创建角色权限
        /// </summary>
        /// <param name="createPermisssionDTO"></param>
        /// <returns></returns>
        public async Task<long> CreateAsync([FromBody] CreatePermisssionDTO createPermisssionDTO,long currentUserId)
        {
            try
            {
                if (createPermisssionDTO.Role_Id < 0 || createPermisssionDTO.Permission_Codes == null || createPermisssionDTO.Permission_Codes.Count <= 0)
                {
                    throw new Exception("Parameter error");
                }
                var role = await _iroleService.GetRoleInfoByIdAsync(createPermisssionDTO.Role_Id);
                var permission = await _db.WMS_Permissions.Where(m => m.Role_Id == createPermisssionDTO.Role_Id && m.Status == (int)DataStatusEnum.Normal).ToListAsync();
                List<WMS_Permission> list = new List<WMS_Permission>();
                if (permission == null || permission.Count <= 0)
                {
                    foreach (var item in createPermisssionDTO.Permission_Codes)
                    {
                        WMS_Permission param = new WMS_Permission();
                        param.Role_Id = role.Id;
                        param.Permission_Code = item;
                        param.Status = (int)DataStatusEnum.Normal;
                        param.Creator = currentUserId;
                        param.Create_Time = DateTime.Now;
                        list.Add(param);
                    }
                }
                else 
                {
                    foreach (var item in createPermisssionDTO.Permission_Codes)
                    {
                        if (!permission.Any(m => m.Permission_Code == item))
                        {
                            WMS_Permission param = new WMS_Permission();
                            param.Role_Id = role.Id;
                            param.Permission_Code = item;
                            param.Status = (int)DataStatusEnum.Normal;
                            param.Creator = currentUserId;
                            param.Create_Time = DateTime.Now;
                            list.Add(param);
                        }
                    }
                    foreach (var item in permission)
                    {
                        if (!createPermisssionDTO.Permission_Codes.Contains(item.Permission_Code))
                        {
                            item.Status = (int)DataStatusEnum.Delete;
                            item.Updator = currentUserId;
                            item.Update_Time = DateTime.Now;
                        }
                    }
                }
                await _db.WMS_Permissions.AddRangeAsync(list);
                return await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据角色获取角色统一权限
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<string[]> GetRolePermissionsByRoleIdAsync(long Id)
        {
            try
            {
                if (Id < 0)
                {
                    throw new Exception("Parameter error");
                }
                var permission = await _db.WMS_Permissions.Where(m => m.Role_Id == Id && m.Status == (int)DataStatusEnum.Normal).Select(n => n.Permission_Code).Distinct().ToArrayAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据多个角色获取角色统一权限
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public async Task<string[]> GetRolePermissionsByRoleIdsAsync(List<long> Ids)
        {
            try
            {
                if (Ids == null || Ids.Count <= 0)
                {
                    throw new Exception("Parameter error");
                }
                var permission = await _db.WMS_Permissions.Where(m => Ids.Contains(m.Role_Id) && m.Status == (int)DataStatusEnum.Normal).Select(n => n.Permission_Code).Distinct().ToArrayAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _log.LogDebug(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
