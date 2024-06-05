using Intelligent_AutoWms.Model.ResponseDTO.Role;

namespace Intelligent_AutoWms.Model.ResponseDTO.User
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// 性别(M:男生 W：女生)
        /// </summary>
        public string Gender { get; set; }

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
        /// 状态 1：正常 2：注销 3.禁用
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Create_Time { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long Creator { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? Update_Time { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public long? Updator { get; set; }

        /// <summary>
        /// 用户角色权限
        /// </summary>
        public List<RoleOptions> Roles { get; set; } = new List<RoleOptions>();
    }
}
