using Intelligent_AutoWms.Extensions.Attri;
using Intelligent_AutoWms.IServices.IServices;
using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.User_Role_RelationShip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Intelligent_AutoWms.WebApi.Controllers
{
    /// <summary>
    /// 用户关系
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class User_Role_RelationShipController : ApiControllerBase
    {
        private readonly IUser_Role_RelationShipService _iuser_Role_RelationShipService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user_Role_RelationShipService"></param>
        public User_Role_RelationShipController(IUser_Role_RelationShipService user_Role_RelationShipService)
        {
            _iuser_Role_RelationShipService = user_Role_RelationShipService;
        }

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Transation]
        public async Task<ApiResult<long>> CreateAsync(long userId, long roleId)
        {
            var currentUserId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var result = await _iuser_Role_RelationShipService.CreateAsync(userId, roleId, currentUserId);
            return SuccessResult(result);
        }

        /// <summary>
        /// 查询用户角色关系
        /// </summary>
        /// <param name="userRoleRelationShipParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<List<WMS_User_Role_RelationShips>>> GetListAsync([FromQuery] UserRoleRelationShipParamsDTO userRoleRelationShipParamsDTO)
        {
            var result = await _iuser_Role_RelationShipService.GetListAsync(userRoleRelationShipParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 分页查询用户角色关系
        /// </summary>
        /// <param name="userRoleRelationShipParamsDTO"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult<BasePagination<WMS_User_Role_RelationShips>>> GetPaginationAsync([FromQuery] UserRoleRelationShipParamsDTO userRoleRelationShipParamsDTO)
        {
            var result = await _iuser_Role_RelationShipService.GetPaginationAsync(userRoleRelationShipParamsDTO);
            return SuccessResult(result);
        }

        /// <summary>
        /// 删除用户角色关系
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [Transation]
        public async Task<ApiResult<long>> DelAsync(string ids)
        {
            var currentUserId = long.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var result = await _iuser_Role_RelationShipService.DelAsync(ids, currentUserId);
            return SuccessResult(result);
        }
    }
}
