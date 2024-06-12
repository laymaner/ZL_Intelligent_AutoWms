using Intelligent_AutoWms.Model.BaseModel;
using Intelligent_AutoWms.Model.Entities;
using Intelligent_AutoWms.Model.RequestDTO.User_Role_RelationShip;
using Microsoft.AspNetCore.Mvc;

namespace Intelligent_AutoWms.IServices.IServices
{
    public interface IUser_Role_RelationShipService
    {
        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="createRelationShipDTO"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<long> CreateAsync([FromBody] CreateRelationShipDTO createRelationShipDTO, long currentUserId);

        /// <summary>
        /// 查询用户角色关系
        /// </summary>
        /// <param name="userRoleRelationShipParamsDTO"></param>
        /// <returns></returns>
        public Task<List<WMS_User_Role_RelationShips>> GetListAsync([FromQuery] UserRoleRelationShipParamsDTO userRoleRelationShipParamsDTO);

        /// <summary>
        /// 分页查询用户角色关系
        /// </summary>
        /// <param name="userRoleRelationShipParamsDTO"></param>
        /// <returns></returns>
        public Task<BasePagination<WMS_User_Role_RelationShips>> GetPaginationAsync([FromQuery] UserRoleRelationShipParamsDTO userRoleRelationShipParamsDTO);

        /// <summary>
        /// 删除用户角色关系
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public Task<long> DelAsync(string ids, long currentUserId);



    }
}
