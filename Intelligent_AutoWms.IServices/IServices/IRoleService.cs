using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.Role;
using Intelligent_AutoWms.Model.ResponseDTO.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IRoleService
    {
        /// <summary>
        /// 查询角色信息
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public Task<List<WMS_Roles>> GetRolesAsync([FromQuery] RoleParamsDTO roleParamsDto);

        /// <summary>
        /// 根据id获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<WMS_Roles> GetRoleInfoByIdAsync(long id);

        /// <summary>
        /// 根据编码获取角色信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<WMS_Roles> GetRoleInfoByCodeAsync(string code);

        /// <summary>
        /// 判断角色是否存在
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string code);

        /// <summary>
        /// 分页查询角色信息
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Roles>> GetRolePaginationAsync([FromQuery] RoleParamsDTO roleParamsDto);

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="createOrUpdateRoleDTO"></param>
        /// <returns></returns>
        public Task<long> CreateRoleAsync([FromBody] CreateOrUpdateRoleDTO createOrUpdateRoleDTO, long currentUserId);

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="createOrUpdateRoleDTO"></param>
        /// <returns></returns>
        public Task<string> UpdateRoleAsync([FromBody] CreateOrUpdateRoleDTO createOrUpdateRoleDTO, long currentUserId);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<long> DelRoleAsync(string ids, long currentUserId);

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        public Task<FileStreamResult> DownloadTemplateAsync();

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] RoleParamsDTO roleParamsDto);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ImportAsync(string path, long currentUserId);

        /// <summary>
        /// 导入----excel导入
        /// </summary>
        /// <param name="fileForm"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ImportExcelAsync(IFormFile fileForm, long currentUserId);

        /// <summary>
        /// 根据ids集合获取角色数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_Roles>> GetRoleByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取角色数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_Roles>> GetRoleByCodesAsync(string codes);

        /// <summary>
        /// 获取角色选项集
        /// </summary>
        /// <param name="roleParamsDto"></param>
        /// <returns></returns>
        public Task<List<RoleOptions>> GetRoleOptionsAsync([FromQuery] RoleParamsDTO roleParamsDto);
    }
}
