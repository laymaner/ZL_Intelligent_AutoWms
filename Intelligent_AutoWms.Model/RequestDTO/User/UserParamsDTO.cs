using Intelligent_AutoWms.Model.BaseModel;

namespace Intelligent_AutoWms.Model.RequestDTO.User
{
    /// <summary>
    /// 查询用户参数实体
    /// </summary>
    public class UserParamsDTO : BasicQuery
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }

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

    }
}
