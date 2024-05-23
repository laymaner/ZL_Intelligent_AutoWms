using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.User;
using Intelligent_AutoWms.Model.ResponseDTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _iuserService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public UserController(IUserService service)
        {
            _iuserService = service;
        }

        /// <summary>
        /// 查询用户信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Users>>> GetUserAsync([FromQuery] UserParamsDTO userParams)
        {
            var result = await _iuserService.GetUserAsync(userParams);
            return SuccessResult(result);
        }

        /// <summary>
        ///  根据用户id查询用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<UserInfo>> GetUserByIdAsync(long id)
        {
            var result = await _iuserService.GetUserByIdAsync(id);
            return SuccessResult(result);
        }

        /// <summary>
        ///  根据用户code查询用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<UserInfo>> GetUserByCodeAsync(string userCode)
        {
            var result = await _iuserService.GetUserByCodeAsync(userCode);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据ids集合获取用户数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Users>>> GetUserByIdsAsync(string ids)
        {
            var result = await _iuserService.GetUserByIdsAsync(ids);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据codes集合获取用户数据
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_Users>>> GetUserByCodesAsync(string codes)
        {
            var result = await _iuserService.GetUserByCodesAsync(codes);
            return SuccessResult(result);
        }

        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<bool>> IsExistAsync(string userCode)
        {
            var result = await _iuserService.IsExistAsync(userCode);
            return SuccessResult(result);
        }

        /// <summary>
        /// 分页查询用户信息
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_Users>>> GetUserPaginationAsync([FromQuery] UserParamsDTO userParams)
        {
            var result = await _iuserService.GetUserPaginationAsync(userParams);
            return SuccessResult(result);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="createOrUpdateUserDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateUserAsync([FromBody] CreateOrUpdateUserDTO createOrUpdateUserDTO)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _iuserService.CreateUserAsync(createOrUpdateUserDTO, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelUserAsync(string ids)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _iuserService.DelUserAsync(ids, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="createOrUpdateUserDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> UpdateUserAsync([FromBody] CreateOrUpdateUserDTO createOrUpdateUserDTO)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _iuserService.UpdateUserAsync(createOrUpdateUserDTO, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Transation]
        public async Task<ApiResult<TokenInfo>> LoginAsync([FromBody] UserLoginDTO userLogin)
        {
            var result = await _iuserService.LoginAsync(userLogin);
            return SuccessResult(result);
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<string>> ReSetPasswordAsync(string userCode, string password, string newPassword)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _iuserService.ReSetPasswordAsync(userCode, password, newPassword, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取jwt
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResult<TokenInfo>> GetJwtDataAsync(string userCode)
        {
            var result = await _iuserService.GetJwtDataAsync(userCode);
            return SuccessResult(result);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> ExportAsync([FromQuery] UserParamsDTO userParams)
        {
            return await _iuserService.ExportAsync(userParams);
        }

        /// <summary>
        /// 下载excel模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> DownloadTemplateAsync()
        {
            return await _iuserService.DownloadTemplateAsync();
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
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _iuserService.ImportAsync(path, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 根据jwt获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<JwtUserInfo>> GetUserInfoFromJwtAsync(string token)
        {
            var result = await _iuserService.GetUserInfoFromJwtAsync(token);
            return SuccessResult(result);
        }

        /// <summary>
        /// 获取用户选项集
        /// </summary>
        /// <param name="userParams"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<UserOptions>>> GetUserOptionsAsync([FromQuery] UserParamsDTO userParams)
        {
            var result = await _iuserService.GetUserOptionsAsync(userParams);
            return SuccessResult(result);
        }
    }
}
