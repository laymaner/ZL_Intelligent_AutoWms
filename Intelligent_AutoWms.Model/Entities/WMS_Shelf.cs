using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 货架
    /// </summary>
    [Table("WMS_Shelf")]
    public class WMS_Shelf
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 货架名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 库区id
        /// </summary>
        [Column("area_id")]
        public long Area_Id { get; set; }

        /// <summary>
        /// 巷道数量
        /// </summary>
        [Column("lanway")]
        public int Lanway { get; set; }

        /// <summary>
        /// 排数
        /// </summary>
        [Column("shelf_rows")]
        public int Shelf_Rows { get; set; }

        /// <summary>
        /// 列数
        /// </summary>
        [Column("shelf_columns")]
        public int Shelf_Columns { get; set; }

        /// <summary>
        /// 层数
        /// </summary>
        [Column("shelf_layers")]
        public int Shelf_Layers { get; set; }

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
