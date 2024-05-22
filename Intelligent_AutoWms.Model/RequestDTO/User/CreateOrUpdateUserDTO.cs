namespace Intelligent_AutoWms.Model.RequestDTO.User
{
    /// <summary>
    /// 创建或更新用户信息
    /// </summary>
    public class CreateOrUpdateUserDTO
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string? Birth { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// 居住地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
