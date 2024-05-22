using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 库区
    /// </summary>
    [Table("WMS_Area")]
    public class WMS_Area
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 库区名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 仓库id
        /// </summary>
        [Column("warehouse_id")]
        public long Warehouse_Id { get; set; }

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
