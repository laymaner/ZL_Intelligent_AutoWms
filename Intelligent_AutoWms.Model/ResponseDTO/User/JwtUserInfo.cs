namespace Intelligent_AutoWms.Model.ResponseDTO.User
{
    /// <summary>
    /// 根据jwt token获取用户信息
    /// </summary>
    public class JwtUserInfo
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 角色编码集合
        /// </summary>
        public string[] Roles { get; set; }
    }
}
