using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.User;
using Intelligent_AutoWms.Model.ResponseDTO.User;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IUserService
    {
        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public Task<List<WMS_Users>> GetUserAsync([FromQuery] UserParamsDTO userParams);

        /// <summary>
        ///  根据用户id查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<UserInfo> GetUserByIdAsync(long id);

        /// <summary>
        ///  根据用户id查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<UserInfo> GetUserByCodeAsync(string userCode);

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public Task<bool> IsExistAsync(string userCode);

        /// <summary>
        /// 分页查询用户信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_Users>> GetUserPaginationAsync([FromQuery] UserParamsDTO userParams);

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="createOrUpdateUserDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<long> CreateUserAsync([FromBody] CreateOrUpdateUserDTO createOrUpdateUserDTO, string currentUserId);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<long> DelUserAsync(string ids, string currentUserId);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="createOrUpdateUserDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> UpdateUserAsync([FromBody] CreateOrUpdateUserDTO createOrUpdateUserDTO, string currentUserId);

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public Task<TokenInfo> LoginAsync([FromBody] UserLoginDTO userLogin);

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <param name="newPassword"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ReSetPasswordAsync(string userCode, string password, string newPassword, string currentUserId);

        /// <summary>
        /// 获取jwt
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<TokenInfo> GetJwtDataAsync(string userCode);

        /// <summary>
        /// 下载Excel模板
        /// </summary>
        /// <returns></returns>
        public Task<FileStreamResult> DownloadTemplateAsync();

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public Task<FileStreamResult> ExportAsync([FromQuery] UserParamsDTO userParams);

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="path"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<string> ImportAsync(string path, string currentUserId);

        /// <summary>
        /// 根据ids集合获取用户数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Task<List<WMS_Users>> GetUserByIdsAsync(string ids);

        /// <summary>
        /// 根据codes集合获取用户数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public Task<List<WMS_Users>> GetUserByCodesAsync(string codes);

        /// <summary>
        /// 根据jwt获取用户信息
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public Task<JwtUserInfo> GetUserInfoFromJwtAsync(string token);

        /// <summary>
        /// 获取用户选项集
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        public Task<List<UserOptions>> GetUserOptionsAsync([FromQuery] UserParamsDTO userParams);

    }
}
