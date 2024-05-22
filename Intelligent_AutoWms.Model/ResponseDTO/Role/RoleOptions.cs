namespace Intelligent_AutoWms.Model.ResponseDTO.Role
{
    /// <summary>
    /// 角色选项
    /// </summary>
    public class RoleOptions
    {

        /// <summary>
        /// 角色id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色编码
        /// </summary>
        public string Code { get; set; }

        public RoleOptions(long id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }
    }
}
