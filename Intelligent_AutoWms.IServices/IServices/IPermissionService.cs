using Intelligent_AutoWms.Model.RequestDTO.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IPermissionService
    {
        /// <summary>
        /// 创建角色权限
        /// </summary>
        /// <param name="createPermisssionDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreatePermisssionDTO createPermisssionDTO, long currentUserId);

        /// <summary>
        /// 根据多个角色获取角色统一权限
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public Task<string[]> GetRolePermissionsByRoleIdsAsync(List<long> Ids);

        /// <summary>
        /// 根据角色获取角色统一权限
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Task<string[]> GetRolePermissionsByRoleIdAsync(long Id);
    }
}
