using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Role;
using Intelligent_AutoWms.Model.ResponseDTO.Role;
using Intelligent_AutoWms.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 角色
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoleController : ApiControllerBase
    {
        private readonly IRoleService _iroleService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iroleService"></param>
        public RoleController(IRoleService iroleService)
        {
            _iroleService = iroleService;
        }

        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Roles>>> GetRolesAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            var result = await _iroleService.GetRolesAsync(roleParamsDto);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据id获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Roles>> GetRoleInfoByIdAsync(long id)
        {
            var result = await _iroleService.GetRoleInfoByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据编码获取角色信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<WMS_Roles>> GetRoleInfoByCodeAsync(string code)
        {
            var result = await _iroleService.GetRoleInfoByCodeAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取用户数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Roles>>> GetRoleByIdsAsync(string ids)
        {
            var result = await _iroleService.GetRoleByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取用户数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Roles>>> GetUserByCodesAsync(string codes)
        {
            var result = await _iroleService.GetRoleByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断角色是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string code)
        {
            var result = await _iroleService.IsExistAsync(code);
            return SuccessResult(result);
        }

        /// <summary>
        /// 分页查询角色信息
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Roles>>> GetRolePaginationAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            var result = await _iroleService.GetRolePaginationAsync(roleParamsDto);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="createOrUpdateRoleDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateRoleAsync([FromBody] CreateOrUpdateRoleDTO createOrUpdateRoleDTO)
        {
            var currentUserId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var result = await _iroleService.CreateRoleAsync(createOrUpdateRoleDTO, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="createOrUpdateRoleDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> UpdateRoleAsync([FromBody] CreateOrUpdateRoleDTO createOrUpdateRoleDTO)
        {
            var currentUserId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var result = await _iroleService.UpdateRoleAsync(createOrUpdateRoleDTO, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelRoleAsync(string ids)
        {
            var currentUserId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var result = await _iroleService.DelRoleAsync(ids, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _iroleService.DownloadTemplateAsync();
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            return await _iroleService.ExportAsync(roleParamsDto);
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> ImportAsync(string path)
        {
            var currentUserId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var result = await _iroleService.ImportAsync(path, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 导入----excel导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> ImportExcelAsync()
        {
            var fileForm = Request.Form.Files.FirstOrDefault();
            var result = await _iroleService.ImportExcelAsync(fileForm, long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取角色选项集
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<RoleOptions>>> GetRoleOptionsAsync([FromQuery] RoleParamsDTO roleParamsDto)
        {
            var result = await _iroleService.GetRoleOptionsAsync(roleParamsDto);
            return SuccessResult(result);
        }

    }
}
