namespace Intelligent_AutoWms.Model.RequestDTO.User_Role_RelationShip
{
    public class CreateRelationShipDTO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public List<long> RoleIds { get; set;} = new List<long>();
    }
}
