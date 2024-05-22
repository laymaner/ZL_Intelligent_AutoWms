using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.Role
{
    /// <summary>
    /// 查询角色参数实体
    /// </summary>
    public class RoleParamsDTO : BasicQuery
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 角色姓名
        /// </summary>
        public string? Name { get; set; }
    }
}
