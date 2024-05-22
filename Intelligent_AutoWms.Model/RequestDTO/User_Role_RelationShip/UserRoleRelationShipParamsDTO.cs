using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.User_Role_RelationShip
{
    /// <summary>
    /// 查询角色用户关系参数实体
    /// </summary>
    public class UserRoleRelationShipParamsDTO : BasicQuery
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long? RoleId { get; set; }

    }
}
