using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 用户角色关系
    /// </summary>
    [Table("WMS_User_Role_RelationShips")]
    public class WMS_User_Role_RelationShips
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [Column("user_id")]
        public long User_Id { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        [Column("role_id")]
        public long Role_Id { get; set; }

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
