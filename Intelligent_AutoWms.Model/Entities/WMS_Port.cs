using System.ComponentModel.DataAnnotations.Schema;

namespace Intelligent_AutoWms.Model.Entities
{
    /// <summary>
    /// 出入口
    /// </summary>
    [Table("WMS_Port")]
    public class WMS_Port
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 出入口名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// 出入口编码
        /// </summary>
        [Column("code")]
        public string Code { get; set; }

        /// <summary>
        /// 类型 1：入口 2：出口
        /// </summary>
        [Column("type")]
        public int Type { get; set; }

        /// <summary>
        /// 仓库id
        /// </summary>
        [Column("warehouse_id")]
        public long Warehouse_Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("first_lanway")]
        public int First_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("second_lanway")]
        public int Second_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("third_lanway")]
        public int Third_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("forth_lanway")]
        public int Forth_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("fifth_lanway")]
        public int Fifth_Lanway { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("sixth_lanway")]
        public int Sixth_Lanway { get; set; }

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
