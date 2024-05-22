using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table("WMS_Users")]
    public class WMS_Users
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Column("age")]
        public int? Age { get; set; }

        /// <summary>
        /// 性别(M:男生 W：女生)
        /// </summary>
        [Column("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Column("birth")]
        public string? Birth { get; set; }

        /// <summary>
        /// 电子邮件
        /// </summary>
        [Column("email")]
        public string? Email { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [Column("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// 居住地址
        /// </summary>
        [Column("address")]
        public string? Address { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Column("password")]
        public string Password { get; set; }

        /// <summary>
        /// 自增jwt令牌
        /// </summary>
        [Column("jwt_version")]
        public long? Jwt_Version { get; set; }

        /// <summary>
        /// 状态 1：正常 2：注销 3.禁用
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("remark")]
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("create_time")]
        public DateTime Create_Time { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("creator")]
        public long Creator { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("update_time")]
        public DateTime? Update_Time { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        [Column("updator")]
        public long? Updator { get; set; }


    }
}
