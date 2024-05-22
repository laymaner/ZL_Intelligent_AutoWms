namespace Intelligent_AutoWms.Model.RequestDTO.User
{
    /// <summary>
    /// 用户登陆参数
    /// </summary>
    public class UserLoginDTO
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
    }
}
