namespace Intelligent_AutoWms.Model.RequestDTO.Role
{
    /// <summary>
    /// 创建或更新角色信息
    /// </summary>
    public class CreateOrUpdateRoleDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
