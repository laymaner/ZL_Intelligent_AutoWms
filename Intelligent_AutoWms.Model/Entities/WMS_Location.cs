using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 存储位置
    /// </summary>
    [Table("WMS_Location")]
    public class WMS_Location
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 货位名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 货位编码
        /// </summary>
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 货架id
        /// </summary>
        [Column("shelf_id")]
        public long Shelf_Id { get; set; }

        /// <summary>
        /// 巷道
        /// </summary>
        [Column("lanway")]
        public int Lanway { get; set; }

        /// <summary>
        /// 排
        /// </summary>
        [Column("location_row")]
        public int Location_Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        [Column("location_column")]
        public int Location_Column { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        [Column("location_layer")]
        public int Location_Layer { get; set; }

        /// <summary>
        /// 1:货位空闲 2:入库锁定 3:出库锁定 4:占用中
        /// </summary>
        [Column("step")]
        public int Step { get; set; }

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
