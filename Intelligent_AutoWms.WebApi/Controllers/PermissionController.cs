using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.RequestDTO.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 库区
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class PermissionController : ApiControllerBase
    {
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permissionService"></param>
        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// 创建角色权限
        /// </summary>
        /// <param name="createPermisssionDTO"></param>
        /// <returns></returns>
        public async Task<ApiResult<long>> CreateAsync([FromBody] CreatePermisssionDTO createPermisssionDTO)
        {
            var result = await _permissionService.CreateAsync(createPermisssionDTO, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据多个角色获取角色统一权限
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public async Task<ApiResult<string[]>> GetRolePermissionsByRoleIdsAsync(List<long> Ids)
        {
            var result = await _permissionService.GetRolePermissionsByRoleIdsAsync(Ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据角色获取角色统一权限
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<ApiResult<string[]>> GetRolePermissionsByRoleIdAsync(long Id)
        {
            var result = await _permissionService.GetRolePermissionsByRoleIdAsync(Id);
            return SuccessResult(result);
        }
    }
}
